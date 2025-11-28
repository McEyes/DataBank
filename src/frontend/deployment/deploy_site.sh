#!/bin/bash
set -eo pipefail

# ==============================================================================
# 网站部署自动化脚本
# 功能：备份当前站点、清理目标目录、部署新站点
# 作者：AI Assistant
# 日期：2025-10-23
# ==============================================================================

# ------------------------------------------------------------------------------
# 配置参数
# ------------------------------------------------------------------------------
# 备份根目录（默认为当前目录）
BACKUP_ROOT="."

# 目标站点目录（需要部署的位置）
TARGET_DIR="/usr/local/app/websites/dataops/"

# 新站点源目录（包含要部署的新内容）
SOURCE_DIR="/home/3223901/src/Data Asset Management.NorthAsia.HUA/WebApp/DataOps.Web/"

# 日志文件名称
LOG_DIR="logs"
LOG_FILE="${LOG_DIR}/deployment_$(date +%Y%m%d%H%M%S).log"

# 时间戳格式
TIMESTAMP=$(date +%Y%m%d%H%M%S)

# 备份目录名称
BACKUP_DIR="${SOURCE_DIR}/../backup/DataOps.Web_${TIMESTAMP}"

# ------------------------------------------------------------------------------
# 颜色定义
# ------------------------------------------------------------------------------
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # 无颜色

# ------------------------------------------------------------------------------
# 函数定义
# ------------------------------------------------------------------------------

# 显示错误信息并退出
error_exit() {
    echo -e "${RED}[ERROR] $1${NC}" >&2
    exit 1
}

# 显示信息
info() {
    echo -e "${GREEN}[INFO] $1${NC}"
}

# 显示警告
warning() {
    echo -e "${YELLOW}[WARNING] $1${NC}"
}

# 显示用法
usage() {
    echo "网站部署自动化脚本"
    echo "用法: $0 [选项]"
    echo
    echo "选项:"
    echo "  -h, --help           显示此帮助信息"
    echo "  -b, --backup-root    指定备份根目录（默认为当前目录）"
    echo "  -t, --target-dir     指定目标站点目录"
    echo "  -s, --source-dir     指定新站点源目录"
    echo "  -y, --yes            自动确认所有操作（不提示用户确认）"
    echo
    echo "示例:"
    echo "  $0 -b /backup -t /var/www/site -s /home/user/new_site"
    echo "  $0 --yes  # 自动执行所有操作，不进行确认"
}

# 验证目录是否存在
validate_directory() {
    local dir_path="$1"
    local dir_name="$2"

    if [ ! -d "$dir_path" ]; then
        error_exit "${dir_name}目录不存在: ${dir_path}"
    fi

    if [ ! -r "$dir_path" ]; then
        error_exit "对${dir_name}目录没有读取权限: ${dir_path}"
    fi
}

# 创建备份
create_backup() {
    info "开始创建站点备份..."

    # 创建备份目录
    if ! mkdir -p "$BACKUP_DIR"; then
        # 尝试使用sudo创建
        info "普通用户无法创建备份目录，尝试使用sudo..."
        if ! sudo mkdir -p "$BACKUP_DIR"; then
            error_exit "无法创建备份目录: ${BACKUP_DIR}"
        fi
    fi

    info "备份目录: ${BACKUP_DIR}"

    # 执行备份
    info "正在复制站点文件到备份目录..."
    if ! cp -r "${TARGET_DIR}/." "${BACKUP_DIR}/"; then
        # 如果普通复制失败，尝试使用sudo
        info "普通用户复制失败，尝试使用sudo..."
        if ! sudo cp -r "${TARGET_DIR}/." "${BACKUP_DIR}/"; then
            error_exit "备份失败！请检查源目录和权限。"
        fi
    fi

    info "备份完成: ${BACKUP_DIR}"
}

# 清理目标目录
clean_target() {
    info "开始清理目标目录..."

    # 检查目标目录是否为空
    if [ -z "$(ls -A "${TARGET_DIR}")" ]; then
        warning "目标目录已经是空的，跳过清理步骤"
        return 0
    fi

    # 确认删除操作
    if [ "$AUTO_CONFIRM" != "true" ]; then
        read -p "确定要删除 ${TARGET_DIR} 目录下的所有文件吗？[y/N] " confirm
        if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
            error_exit "用户取消了删除操作"
        fi
    fi

    # 执行删除
    info "正在删除目标目录内容: ${TARGET_DIR}"
    if ! rm -rf "${TARGET_DIR:?}"/*; then
        # 如果普通删除失败，尝试使用sudo
        info "普通用户删除失败，尝试使用sudo..."
        if ! sudo rm -rf "${TARGET_DIR:?}"/*; then
            error_exit "清理目标目录失败！"
        fi
    fi

    info "目标目录清理完成"
}

# 部署新站点
deploy_new_site() {
    info "开始部署新站点..."

    # 验证源目录
    validate_directory "$SOURCE_DIR" "源"

    # 执行复制
    info "正在将新站点文件复制到目标目录..."
    if ! cp -r "${SOURCE_DIR}/." "${TARGET_DIR}/"; then
        # 如果普通复制失败，尝试使用sudo
        info "普通用户复制失败，尝试使用sudo..."
        if ! sudo cp -r "${SOURCE_DIR}/." "${TARGET_DIR}/"; then
            error_exit "部署失败！请检查源目录和权限。"
        fi
    fi

    info "新站点部署完成"
}

# 检查目录权限
check_directory_permissions() {
    local dir_path="$1"
    local dir_name="$2"
    local required_perms="$3"

    # 检查读权限
    if [ ! -r "$dir_path" ]; then
        error_exit "对${dir_name}目录没有读取权限: ${dir_path}"
    fi

    # 如果需要写权限
    if [[ "$required_perms" == *"w"* ]] && [ ! -w "$dir_path" ]; then
        # 检查是否可以通过sudo获取写权限
        if sudo -n true 2>/dev/null; then
            warning "${dir_name}目录没有写权限，但可以通过sudo获取"
        else
            error_exit "对${dir_name}目录没有写权限，且无法使用sudo: ${dir_path}"
        fi
    fi

    # 如果需要执行权限
    if [[ "$required_perms" == *"x"* ]] && [ ! -x "$dir_path" ]; then
        error_exit "对${dir_name}目录没有执行权限: ${dir_path}"
    fi
}

# 验证配置
validate_config() {
    info "正在验证配置..."

    # 验证目标目录（需要读写权限）
    validate_directory "$TARGET_DIR" "目标"
    check_directory_permissions "$TARGET_DIR" "目标" "rw"

    # 验证源目录（至少需要读权限）
    if [ "$AUTO_CONFIRM" != "true" ]; then
        validate_directory "$SOURCE_DIR" "源"
        check_directory_permissions "$SOURCE_DIR" "源" "r"
    fi

    # 验证备份目录（需要写权限）
    if [ ! -d "$BACKUP_ROOT" ]; then
        # 检查是否可以创建备份根目录
        if ! mkdir -p "$BACKUP_ROOT"; then
            # 尝试使用sudo创建
            if ! sudo mkdir -p "$BACKUP_ROOT"; then
                error_exit "无法创建备份根目录: ${BACKUP_ROOT}"
            fi
        fi
    fi
    check_directory_permissions "$BACKUP_ROOT" "备份根" "rw"

    # 检查sudo可用性
    if ! sudo -n true 2>/dev/null; then
        warning "sudo可能需要密码或无法使用，某些操作可能会失败"
    fi

    info "配置验证完成"
}

# 显示配置信息
show_config() {
    echo "========================================"
    echo "部署配置信息"
    echo "========================================"
    echo "备份根目录:   $BACKUP_ROOT"
    echo "备份目录:     $BACKUP_DIR"
    echo "目标站点目录: $TARGET_DIR"
    echo "新站点源目录: $SOURCE_DIR"
    echo "日志文件:     $LOG_FILE"
    echo "========================================"
    echo

    if [ ! -r "${LOG_DIR}" ]; then
       mkdir -p ${LOG_DIR}
    fi
}

# 主函数
main() {
    # 解析命令行参数
    AUTO_CONFIRM="false"

    while [[ $# -gt 0 ]]; do
        case "$1" in
            -h|--help)
                usage
                exit 0
                ;;
            -b|--backup-root)
                BACKUP_ROOT="$2"
                BACKUP_DIR="${BACKUP_ROOT}/backup/DataOps.Web_${TIMESTAMP}"
                shift 2
                ;;
            -t|--target-dir)
                TARGET_DIR="$2"
                shift 2
                ;;
            -s|--source-dir)
                SOURCE_DIR="$2"
                shift 2
                ;;
            -y|--yes)
                AUTO_CONFIRM="true"
                shift
                ;;
            *)
                error_exit "未知选项: $1"
                ;;
        esac
    done

    # 显示欢迎信息
    echo "========================================"
    echo "网站部署自动化脚本"
    echo "========================================"

    # 显示配置信息
    show_config

    # 确认开始执行
    if [ "$AUTO_CONFIRM" != "true" ]; then
        read -p "是否继续执行部署流程？[y/N] " confirm
        if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
            error_exit "用户取消了部署操作"
        fi
    fi

    # 开始执行部署流程
    info "开始执行部署流程..."

    # 验证配置
    validate_config

    # 创建备份
    create_backup

    # 清理目标目录
    clean_target

    # 部署新站点
    deploy_new_site

    # 部署完成
    info "========================================"
    info "部署流程全部完成！"
    info "备份位置: ${BACKUP_DIR}"
    info "目标站点: ${TARGET_DIR}"
    info "========================================"
}

# 执行主函数
main "$@" 2>&1 | tee -a "$LOG_FILE"
