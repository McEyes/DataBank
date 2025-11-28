#!/bin/bash
set -eo pipefail

# ==============================================================================
# Docker容器化应用部署自动化脚本
# 功能：版本管理、多项目构建、容器部署
# 作者：AI Assistant
# 日期：2025-10-23
# ==============================================================================

# ------------------------------------------------------------------------------
# 配置参数
# ------------------------------------------------------------------------------
# 工作目录
WORK_DIR="/home/3223901/src/Data Asset Management.NorthAsia.HUA/"

# 日志文件
LOG_FILE="docker_deployment_$(date +%Y%m%d%H%M%S).log"

# 项目配置
declare -A PROJECTS=(
    ["dataasset"]="Dockerfile.DataAsset"
    ["itportal.flow"]="Dockerfile.Flow"
    ["itportal.gateways"]="Dockerfile.Gateways"
    ["itportal.search"]="Dockerfile.Search"
    ["itportal.res"]="Dockerfile.Res"
    ["itportal.auth"]="Dockerfile.Auth"
    ["itportal.dashbord"]="Dockerfile.Dashbord"
)

# 容器运行配置
declare -A CONTAINER_CONFIGS=(
    ["dataasset"]="--name dataasset-api-prd -p 7001:6001 -v /home/3223901/temp/logs/dataasset.api:/app/logs"
    ["itportal.auth"]="--name dataasset-auth-prd -p 6000:6000 -p 6001:6001 -v /home/3223901/temp/logs/dataasset.auth:/app/logs"
    ["itportal.flow"]="--name dataasset-flow-prd -p 7005:6005 -v /home/3223901/temp/logs/dataasset.flow:/app/logs"
    ["itportal.search"]="--name dataasset-search-prd -p 7006:6006 -v /home/3223901/temp/logs/dataasset.search:/app/logs"
    ["itportal.res"]="--name dataasset-res-prd -p 7003:6003 -v /home/3223901/temp/logs/dataasset.res:/app/logs -v /usr/resource/dataops:/app/uploads"
    ["itportal.dashbord"]="--name dataasset-dashbord-prd -p 7008:7008 -v /home/3223901/temp/logs/dataasset.dashbord:/app/logs"
)


# 默认版本号前缀
DEFAULT_VERSION_PREFIX="0.2."

# ------------------------------------------------------------------------------
# 颜色定义
# ------------------------------------------------------------------------------
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
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

# 显示标题
title() {
    echo -e "${BLUE}========================================"
    echo -e "  $1"
    echo -e "========================================${NC}"
}

# 显示用法
usage() {
    echo "Docker容器化应用部署自动化脚本"
    echo "用法: $0 [选项] [项目名称...]"
    echo
    echo "选项:"
    echo "  -h, --help           显示此帮助信息"
    echo "  -y, --yes            自动确认所有操作"
    echo "  -v, --version        指定版本号（如 0.2.26）"
    echo "  -a, --auto-version   自动生成版本号（基于最新版本）"
    echo "  -l, --list-projects  列出所有可用项目"
    echo "  --work-dir           指定工作目录（默认: ${WORK_DIR}）"
    echo
    echo "示例:"
    echo "  $0 -v 0.2.26 dataasset                # 部署dataasset项目，版本号0.2.26"
    echo "  $0 -a itportal.flow itportal.search   # 自动生成版本号，部署多个项目"
    echo "  $0 -y -v 0.3.0 itportal.res           # 自动确认，部署itportal.res项目"
    echo "  $0 -l                                 # 列出所有可用项目"
}

# 列出所有可用项目
list_projects() {
    title "可用项目列表"
    echo "项目名称               Dockerfile"
    echo "---------------------- ----------------------"
    for project in "${!PROJECTS[@]}"; do
        printf "%-22s %s\n" "$project" "${PROJECTS[$project]}"
    done
    echo
}

# 切换到工作目录
change_to_work_dir() {
    info "正在切换到工作目录: ${WORK_DIR}"
    
    if [ ! -d "$WORK_DIR" ]; then
        error_exit "工作目录不存在: ${WORK_DIR}"
    fi
    
    if ! cd "$WORK_DIR"; then
        error_exit "无法切换到工作目录: ${WORK_DIR}"
    fi
    
    info "当前工作目录: $(pwd)"
}

# 获取最新版本号
get_latest_version() {
    local project="$1"
    
    # 获取所有标签并排序
    local tags=$(docker images --format "{{.Tag}}" "$project" | grep -E '^prd_v[0-9]+\.[0-9]+\.[0-9]+$' | sort -V)
    
    if [ -z "$tags" ]; then
        echo "${DEFAULT_VERSION_PREFIX}0"
        return
    fi
    
    # 获取最新版本
    local latest_tag=$(echo "$tags" | tail -n 1)
    local latest_version=${latest_tag#prd_v}
    
    echo "$latest_version"
}

# 自动生成新版本号
generate_new_version() {
    local project="$1"
    local latest_version=$(get_latest_version "$project")
    
    # 解析版本号
    IFS='.' read -r major minor patch <<< "$latest_version"
    
    # 递增补丁版本
    new_patch=$((patch + 1))
    new_version="${major}.${minor}.${new_patch}"
    
    echo "$new_version"
}

# 确认版本号
confirm_version() {
    local project="$1"
    local version="$2"
    
    if [ "$AUTO_CONFIRM" = "true" ]; then
        return 0
    fi
    
    read -p "确定要使用版本号 $version 部署项目 $project 吗？[y/N] " confirm
    if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
        error_exit "用户取消了部署操作"
    fi
}

# 构建Docker镜像
build_image() {
    local project="$1"
    local version="$2"
    local dockerfile="${PROJECTS[$project]}"
    
    title "构建项目: $project (版本: $version)"
    
    # 检查Dockerfile是否存在
    if [ ! -f "$dockerfile" ]; then
        error_exit "Dockerfile不存在: $dockerfile"
    fi
    
    # 检查是否需要打标签
    local existing_tags=$(docker images --format "{{.Tag}}" "$project" | grep -w "prd")
    
    if [ -n "$existing_tags" ]; then
        info "为现有镜像打标签..."
        if ! docker tag "$project:prd" "$project:prd_v$version"; then
            error_exit "打标签失败: $project:prd -> $project:prd_v$version"
        fi
    fi
    
    # 构建新镜像
    info "正在构建Docker镜像..."
    if ! docker build -f "$dockerfile" -t "$project:prd" -t "$project:prd_v$version" .; then
        error_exit "构建镜像失败: $project:prd_v$version"
    fi
    
    info "镜像构建完成: $project:prd_v$version"
}

# 停止容器
stop_container() {
    local container_name="$1"
    
    if [ "$(docker ps -q -f name="$container_name")" ]; then
        info "正在停止容器: $container_name"
        if ! docker stop "$container_name"; then
            warning "停止容器失败: $container_name"
            return 1
        fi
        info "容器已停止: $container_name"
    else
        info "容器未运行: $container_name"
    fi
    
    return 0
}

# 删除容器
remove_container() {
    local container_name="$1"
    
    if [ "$(docker ps -aq -f name="$container_name")" ]; then
        info "正在删除容器: $container_name"
        if ! docker rm -f "$container_name"; then
            error_exit "删除容器失败: $container_name"
        fi
        info "容器已删除: $container_name"
    else
        info "容器不存在: $container_name"
    fi
}

# 启动容器
start_container() {
    local project="$1"
    local config="${CONTAINER_CONFIGS[$project]}"
    
    if [ -z "$config" ]; then
        warning "没有找到项目 $project 的容器配置，跳过启动"
        return 0
    fi
    
    title "启动容器: $project"
    
    # 解析容器名称
    # 使用兼容的方法解析，不依赖Perl正则表达式
    local container_name=$(echo "$config" | grep -- '--name' | sed 's/.*--name //;s/ .*//')
    
    if [ -z "$container_name" ]; then
        error_exit "无法从配置中解析容器名称: $config"
    fi
    
    # 停止并删除旧容器
    stop_container "$container_name"
    remove_container "$container_name"
    
    # 启动新容器
    info "正在启动新容器: $container_name"
    
    # 构建docker run命令的固定部分
    local run_command="docker run -d --restart=always --privileged -v /etc/hosts:/tmp/hosts:ro"
    
    # 添加基础环境变量
    run_command+=' -e TZ=Asia/Shanghai'
    run_command+=' -e "ASPNETCORE_ENVIRONMENT=Production"'
    
    # 根据项目设置特定的环境变量
    if [ "$project" = "itportal.auth" ]; then
        # 为auth项目特殊处理，支持http和https
        run_command+=' -e "ASPNETCORE_URLS=http://+:6000;https://+:6001;"'
    else
        # 提取所有所有容器端口号
        # 使用兼容的方法解析，不依赖grep -o选项
        local ports=$(echo "$config" | tr ' ' '\n' | grep '^-p$' -A1 | grep ':' | cut -d: -f2)
        
        if [ -n "$ports" ]; then
            # 生成URL列表
            local urls=""
            while IFS= read -r port; do
                if [ -n "$urls" ]; then
                    urls+=";"
                fi
                urls+="http://+:${port}"
            done <<< "$ports"
            
            run_command+=" -e \"ASPNETCORE_URLS=${urls}\""
        fi
    fi
    
    # 添加容器配置和镜像名称
    run_command+=" $config $project:prd"
    
    info "执行命令: $run_command"
    
    if ! eval "$run_command"; then
        error_exit "启动容器失败: $container_name"
    fi
    
    info "容器启动成功: $container_name"
    
    # 显示容器状态
    info "容器状态:"
    docker ps -f name="$container_name" --format "ID: {{.ID}}, 名称: {{.Names}}, 状态: {{.Status}}, 端口: {{.Ports}}"
}

# 验证项目是否存在
validate_project() {
    local project="$1"
    
    if [ -z "${PROJECTS[$project]}" ]; then
        error_exit "项目不存在: $project。使用 -l 选项查看可用项目。"
    fi
}

# 部署单个项目
deploy_project() {
    local project="$1"
    
    validate_project "$project"
    
    # 获取版本号
    local version
    if [ "$AUTO_VERSION" = "true" ]; then
        local latest_version=$(get_latest_version "$project")
        info "项目 $project 的最新版本: $latest_version"
        version=$(generate_new_version "$project")
        info "自动生成新版本: $version"
    else
        version="$VERSION"
    fi
    
    # 确认版本号
    confirm_version "$project" "$version"
    
    # 构建镜像
    build_image "$project" "$version"
    
    # 启动容器
    start_container "$project"
    
    info "项目 $project 部署完成！"
}

# 主函数
main() {
    # 初始化变量
    AUTO_CONFIRM="false"
    AUTO_VERSION="false"
    VERSION=""
    SELECTED_PROJECTS=()
    
    # 解析命令行参数
    while [[ $# -gt 0 ]]; do
        case "$1" in
            -h|--help)
                usage
                exit 0
                ;;
            -y|--yes)
                AUTO_CONFIRM="true"
                shift
                ;;
            -v|--version)
                VERSION="$2"
                shift 2
                ;;
            -a|--auto-version)
                AUTO_VERSION="true"
                shift
                ;;
            -l|--list-projects)
                list_projects
                exit 0
                ;;
            --work-dir)
                WORK_DIR="$2"
                shift 2
                ;;
            *)
                # 检查是否是项目名称
                if [ -n "${PROJECTS[$1]}" ]; then
                    SELECTED_PROJECTS+=("$1")
                else
                    error_exit "未知选项或项目: $1。使用 -l 选项查看可用项目。"
                fi
                shift
                ;;
        esac
    done
    
    # 检查版本号参数
    if [ "$AUTO_VERSION" = "false" ] && [ -z "$VERSION" ]; then
        error_exit "请指定版本号（使用 -v 选项）或启用自动版本生成（使用 -a 选项）"
    fi
    
    # 检查项目选择
    if [ ${#SELECTED_PROJECTS[@]} -eq 0 ]; then
        error_exit "请选择至少一个项目进行部署。使用 -l 选项查看可用项目。"
    fi
    
    # 显示欢迎信息
    title "Docker容器化应用部署自动化脚本"
    
    # 显示配置信息
    echo "部署配置:"
    echo "工作目录:     ${WORK_DIR}"
    echo "自动确认:     ${AUTO_CONFIRM}"
    echo "自动版本:     ${AUTO_VERSION}"
    if [ -n "$VERSION" ]; then
        echo "指定版本:     ${VERSION}"
    fi
    echo "选择项目:     ${SELECTED_PROJECTS[*]}"
    echo "日志文件:     ${LOG_FILE}"
    echo
    
    # 确认开始执行
    if [ "$AUTO_CONFIRM" != "true" ]; then
        read -p "是否继续执行部署流程？[y/N] " confirm
        if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
            error_exit "用户取消了部署操作"
        fi
    fi
    
    # 切换到工作目录
    change_to_work_dir
    
    # 部署每个选定的项目
    for project in "${SELECTED_PROJECTS[@]}"; do
        deploy_project "$project"
        echo -e "\n========================================\n"
    done
    
    # 部署完成
    title "部署流程全部完成！"
    echo "已部署项目: ${SELECTED_PROJECTS[*]}"
    echo "日志文件:   ${WORK_DIR}/${LOG_FILE}"
    echo "========================================"
}

# 执行主函数
main "$@" 2>&1 | tee -a "$LOG_FILE"
