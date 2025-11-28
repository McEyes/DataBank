#!/bin/bash
set -eo pipefail

# ==============================================================================
# 网站部署回滚脚本
# 功能：从备份恢复到之前的网站版本
# 作者：AI Assistant
# 日期：2025-10-23
# ==============================================================================

# ------------------------------------------------------------------------------
# 配置参数
# ------------------------------------------------------------------------------
# 备份根目录
BACKUP_ROOT="."

# 目标站点目录
TARGET_DIR="/usr/local/app/websites/dataops/"

# 日志文件名称
LOG_FILE="logs/rollback_$(date +%Y%m%d%H%M%S).log"

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
    echo "网站部署回滚脚本"
    echo "用法: $0 [选项] <备份目录或编号>"
    echo
    echo "选项:"
    echo "  -h, --help           显示此帮助信息"
    echo "  -b, --backup-root    指定备份根目录（默认为当前目录）"
    echo "  -t, --target-dir     指定目标站点目录"
    echo "  -l, --list           列出所有可用的备份"
    echo "  -y, --yes            自动确认所有操作（不提示用户确认）"
    echo
    echo "示例:"
    echo "  $0 -l                 # 列出所有备份"
    echo "  $0 1                  # 恢复到第1个备份（最新的）"
    echo "  $0 DataOps.Web_20251023120000  # 恢复到指定备份目录"
    echo "  $0 -b /backup -t /var/www/site 2  # 使用自定义路径恢复到第2个备份"
}

# 列出所有备份
list_backups() {
    info "正在查找备份目录..."
    
    # 查找所有备份目录
    backups=($(find "$BACKUP_ROOT" -maxdepth 1 -type d -name "DataOps.Web_*" | sort -r))
    
    if [ ${#backups[@]} -eq 0 ]; then
        error_exit "在 ${BACKUP_ROOT} 目录下未找到任何备份"
    fi
    
    echo "可用备份列表:"
    echo "========================================"
    
    for i in "${!backups[@]}"; do
        backup="${backups[$i]}"
        # 提取时间戳
        timestamp=$(basename "$backup" | sed 's/^DataOps\.Web_//')
        # 转换为可读日期
        readable_date=$(date -d "${timestamp:0:8} ${timestamp:8:2}:${timestamp:10:2}:${timestamp:12:2}" +"%Y-%m-%d %H:%M:%S" 2>/dev/null || echo "未知时间")
        
        echo "$((i+1)). $readable_date - $backup"
    done
    
    echo "========================================"
    echo "使用: $0 <编号> 来恢复到指定备份"
}

# 验证备份目录
validate_backup() {
    local backup_dir="$1"
    
    if [ ! -d "$backup_dir" ]; then
        error_exit "备份目录不存在: ${backup_dir}"
    fi
    
    # 检查备份目录是否包含网站文件
    if [ -z "$(ls -A "$backup_dir")" ]; then
        error_exit "备份目录为空: ${backup_dir}"
    fi
    
    info "备份目录验证通过: ${backup_dir}"
}

# 验证目标目录
validate_target() {
    local target_dir="$1"
    
    if [ ! -d "$target_dir" ]; then
        error_exit "目标目录不存在: ${target_dir}"
    fi
    
    if [ ! -w "$target_dir" ]; then
        error_exit "对目标目录没有写入权限: ${target_dir}"
    fi
    
    info "目标目录验证通过: ${target_dir}"
}

# 创建当前状态的临时备份
create_temp_backup() {
    local temp_backup_dir="${BACKUP_ROOT}/backup/DataOps.Web_rollback_$(date +%Y%m%d%H%M%S)"
    
    info "正在创建当前状态的临时备份..."
    
    if ! mkdir -p "$temp_backup_dir"; then
        # 尝试使用sudo创建
        info "普通用户无法创建临时备份目录，尝试使用sudo..."
        if ! sudo mkdir -p "$temp_backup_dir"; then
            warning "无法创建临时备份目录，跳过此步骤"
            return 1
        fi
    fi
    
    if ! cp -r "${TARGET_DIR}/." "$temp_backup_dir/"; then
        # 尝试使用sudo复制
        info "普通用户复制失败，尝试使用sudo..."
        if ! sudo cp -r "${TARGET_DIR}/." "$temp_backup_dir/"; then
            warning "临时备份创建失败，跳过此步骤"
            return 1
        fi
    fi
    
    info "临时备份创建完成: ${temp_backup_dir}"
    return 0
}

# 恢复备份
restore_backup() {
    local backup_dir="$1"
    
    info "开始恢复备份: ${backup_dir}"
    
    # 验证备份目录
    validate_backup "$backup_dir"
    
    # 验证目标目录
    validate_target "$TARGET_DIR"
    
    # 创建当前状态的临时备份
    create_temp_backup
    
    # 确认恢复操作
    if [ "$AUTO_CONFIRM" != "true" ]; then
        read -p "确定要将 ${TARGET_DIR} 恢复到 ${backup_dir} 的状态吗？[y/N] " confirm
        if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
            error_exit "用户取消了恢复操作"
        fi
    fi
    
    # 清理目标目录
    info "正在清理目标目录: ${TARGET_DIR}"
    if ! sudo rm -rf "${TARGET_DIR:?}"/*; then
        error_exit "清理目标目录失败！"
    fi
    
    # 恢复备份
    info "正在从备份恢复文件..."
    if ! cp -r "${backup_dir}/." "${TARGET_DIR}/"; then
        # 尝试使用sudo复制
        info "普通用户复制失败，尝试使用sudo..."
        if ! sudo cp -r "${backup_dir}/." "${TARGET_DIR}/"; then
            error_exit "恢复备份失败！"
        fi
    fi
    
    info "备份恢复完成"
}

# 主函数
main() {
    # 解析命令行参数
    AUTO_CONFIRM="false"
    LIST_BACKUPS="false"
    
    while [[ $# -gt 0 ]]; do
        case "$1" in
            -h|--help)
                usage
                exit 0
                ;;
            -b|--backup-root)
                BACKUP_ROOT="$2"
                shift 2
                ;;
            -t|--target-dir)
                TARGET_DIR="$2"
                shift 2
                ;;
            -l|--list)
                LIST_BACKUPS="true"
                shift
                ;;
            -y|--yes)
                AUTO_CONFIRM="true"
                shift
                ;;
            *)
                # 检查是否是备份编号或备份目录
                if [[ "$1" =~ ^[0-9]+$ ]]; then
                    BACKUP_NUMBER="$1"
                else
                    BACKUP_DIR="$1"
                fi
                shift
                ;;
        esac
    done
    
    # 显示欢迎信息
    echo "========================================"
    echo "网站部署回滚脚本"
    echo "========================================"
    
    # 如果只是列出备份
    if [ "$LIST_BACKUPS" = "true" ]; then
        list_backups
        exit 0
    fi
    
    # 如果没有指定备份
    if [ -z "$BACKUP_NUMBER" ] && [ -z "$BACKUP_DIR" ]; then
        echo "请指定要恢复的备份编号或备份目录"
        echo "使用 -l 选项列出所有可用备份"
        exit 1
    fi
    
    # 如果指定了备份编号
    if [ -n "$BACKUP_NUMBER" ]; then
        # 查找所有备份目录
        backups=($(find "$BACKUP_ROOT" -maxdepth 1 -type d -name "DataOps.Web_*" | sort -r))
        
        if [ ${#backups[@]} -eq 0 ]; then
            error_exit "在 ${BACKUP_ROOT} 目录下未找到任何备份"
        fi
        
        # 检查编号是否有效
        if [ "$BACKUP_NUMBER" -lt 1 ] || [ "$BACKUP_NUMBER" -gt ${#backups[@]} ]; then
            error_exit "无效的备份编号。可用编号范围: 1-${#backups[@]}"
        fi
        
        # 获取对应的备份目录
        BACKUP_DIR="${backups[$((BACKUP_NUMBER-1))]}"
    fi
    
    # 显示恢复信息
    echo "恢复配置信息"
    echo "========================================"
    echo "备份根目录:   $BACKUP_ROOT"
    echo "备份目录:     $BACKUP_DIR"
    echo "目标站点目录: $TARGET_DIR"
    echo "日志文件:     $LOG_FILE"
    echo "========================================"
    echo
    
    # 开始恢复流程
    restore_backup "$BACKUP_DIR"
    
    # 恢复完成
    info "========================================"
    info "回滚操作全部完成！"
    info "网站已恢复到: ${BACKUP_DIR}"
    info "目标站点: ${TARGET_DIR}"
    info "========================================"
}

# 执行主函数
main "$@" 2>&1 | tee -a "$LOG_FILE"
