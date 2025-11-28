#!/bin/bash
set -e

# ==============================================================================
# Docker容器化应用部署简化脚本
# 功能：按照用户命令执行Docker构建和部署
# 作者：AI Assistant
# 日期：2025-10-23
# ==============================================================================

# ------------------------------------------------------------------------------
# 配置参数
# ------------------------------------------------------------------------------
# 工作目录
WORK_DIR="/home/3223901/src/Data Asset Management.NorthAsia.HUA/"

# 日志文件
LOG_FILE="docker_deployment_simple_$(date +%Y%m%d%H%M%S).log"

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

# 切换到工作目录
change_to_work_dir() {
    info "1. 正在切换到工作目录: ${WORK_DIR}"
    
    if [ ! -d "$WORK_DIR" ]; then
        error_exit "工作目录不存在: ${WORK_DIR}"
    fi
    
    if ! cd "$WORK_DIR"; then
        error_exit "无法切换到工作目录: ${WORK_DIR}"
    fi
    
    info "当前工作目录: $(pwd)"
}

# 提示输入版本号
prompt_version() {
    read -p "请输入版本号（如 0.2.26）: " version
    
    if [ -z "$version" ]; then
        error_exit "版本号不能为空"
    fi
    
    echo "$version"
}

# 构建DataAsset镜像
build_dataasset() {
    local version="$1"
    
    info "2. 正在构建DataAsset镜像（版本: $version）"
    
    if ! docker build -f Dockerfile.DataAsset -t dataasset:prd -t dataasset:prd_v$version .; then
        error_exit "构建DataAsset镜像失败"
    fi
    
    info "DataAsset镜像构建完成"
}

# 构建其他项目镜像
build_other_projects() {
    info "3. 正在构建其他项目镜像..."
    
    # 构建itportal.flow
    if [ -f "Dockerfile.Flow" ]; then
        docker tag itportal.flow:prd itportal.flow:prd_v0.2.13 &
        docker build -f Dockerfile.Flow -t itportal.flow:prd -t itportal.flow:prd_v0.2.14 .
        info "itportal.flow镜像构建完成"
    else
        echo "Dockerfile.Flow不存在，跳过构建"
    fi
    
    # 构建itportal.gateways
    if [ -f "Dockerfile.Gateways" ]; then
        docker tag itportal.gateways:prd itportal.gateways:prd_v0.2.4 &
        docker build -f Dockerfile.Gateways -t itportal.gateways:prd -t itportal.gateways:prd_v0.2.5 .
        info "itportal.gateways镜像构建完成"
    else
        echo "Dockerfile.Gateways不存在，跳过构建"
    fi
    
    # 构建itportal.search
    if [ -f "Dockerfile.Search" ]; then
        docker tag itportal.search:prd itportal.search:prd_v0.2.3 &
        docker build -f Dockerfile.Search -t itportal.search:prd -t itportal.search:prd_v0.2.4 .
        info "itportal.search镜像构建完成"
    else
        echo "Dockerfile.Search不存在，跳过构建"
    fi
    
    # 构建itportal.res
    if [ -f "Dockerfile.Res" ]; then
        docker tag itportal.res:prd itportal.res:prd_v0.2.0 &
        docker build -f Dockerfile.Res -t itportal.res:prd -t itportal.res:prd_v0.3.0 .
        info "itportal.res镜像构建完成"
    else
        echo "Dockerfile.Res不存在，跳过构建"
    fi
}

# 停止并删除容器
manage_container() {
    local container_name="$1"
    
    info "4. 正在停止容器: $container_name"
    if [ "$(docker ps -q -f name="$container_name")" ]; then
        docker stop "$container_name"
    else
        echo "容器未运行: $container_name"
    fi
    
    info "5. 正在删除容器: $container_name"
    if [ "$(docker ps -aq -f name="$container_name")" ]; then
        docker rm -f "$container_name"
    else
        echo "容器不存在: $container_name"
    fi
}

# 部署DataAsset API
deploy_dataasset_api() {
    info "6. 正在部署DataAsset API..."
    
    docker run -d \
    --restart=always \
    --name dataasset-api-prd \
    -p 7001:6001 \
    --privileged \
    -v /etc/hosts:/tmp/hosts:ro \
    -v /home/3223901/temp/logs/dataasset.api:/app/logs \
    -e TZ=Asia/Shanghai \
    -e "ASPNETCORE_URLS=http://+:6001" \
    -e "ASPNETCORE_ENVIRONMENT=Production" \
    dataasset:prd
    
    info "DataAsset API部署完成"
}

# 部署其他服务
deploy_other_services() {
    info "7. 正在部署其他服务..."
    
    # 部署Auth服务
    if [ "$(docker images -q itportal.auth:prd)" ]; then
        echo "正在部署Auth服务..."
        docker run -d \
        --restart=always \
        --name dataasset-auth-prd \
        -p 6000:6000 \
        -p 6001:6001 \
        --privileged \
        -v /etc/hosts:/tmp/hosts:ro \
        -v /home/3223901/temp/logs/dataasset.auth:/app/logs \
        -e TZ=Asia/Shanghai \
        -e "ASPNETCORE_URLS=http://+:6000;https://+:6001;" \
        -e "ASPNETCORE_ENVIRONMENT=Production" \
        itportal.auth:prd
        info "Auth服务部署完成"
    fi
    
    # 部署Flow服务
    if [ "$(docker images -q itportal.flow:prd)" ]; then
        echo "正在部署Flow服务..."
        docker run -d \
        --restart=always \
        --name dataasset-flow-prd \
        -p 7005:6005 \
        --privileged \
        -v /etc/hosts:/tmp/hosts:ro \
        -v /home/3223901/temp/logs/dataasset.flow:/app/logs \
        -e TZ=Asia/Shanghai \
        -e "ASPNETCORE_URLS=http://+:6005" \
        -e "ASPNETCORE_ENVIRONMENT=Production" \
        itportal.flow:prd
        info "Flow服务部署完成"
    fi
    
    # 部署Search服务
    if [ "$(docker images -q itportal.search:prd)" ]; then
        echo "正在部署Search服务..."
        docker run -d \
        --restart=always \
        --name dataasset-search-prd \
        -p 7006:6006 \
        --privileged \
        -v /etc/hosts:/tmp/hosts:ro \
        -v /home/3223901/temp/logs/dataasset.search:/app/logs \
        -e TZ=Asia/Shanghai \
        -e "ASPNETCORE_URLS=http://+:6006" \
        -e "ASPNETCORE_ENVIRONMENT=Production" \
        itportal.search:prd
        info "Search服务部署完成"
    fi
    
    # 部署Res服务
    if [ "$(docker images -q itportal.res:prd)" ]; then
        echo "正在部署Res服务..."
        docker run -d \
        --restart=always \
        --name dataasset-res-prd \
        -p 7003:6003 \
        --privileged \
        -v /etc/hosts:/tmp/hosts:ro \
        -v /home/3223901/temp/logs/dataasset.res:/app/logs \
        -v /usr/resource/dataops:/app/uploads \
        -e TZ=Asia/Shanghai \
        -e "ASPNETCORE_URLS=http://+:6003" \
        -e "ASPNETCORE_ENVIRONMENT=Production" \
        itportal.res:prd
        info "Res服务部署完成"
    fi
}

# 显示部署状态
show_status() {
    info "8. 部署状态:"
    
    echo -e "\n运行中的容器:"
    docker ps --format "ID: {{.ID}}, 名称: {{.Names}}, 状态: {{.Status}}, 端口: {{.Ports}}"
    
    echo -e "\n镜像列表:"
    docker images --format "仓库: {{.Repository}}, 标签: {{.Tag}}, ID: {{.ID}}, 大小: {{.Size}}" | grep -E 'dataasset|itportal'
}

# 主函数
main() {
    # 显示欢迎信息
    echo "========================================"
    echo "Docker容器化应用部署简化脚本"
    echo "========================================"
    echo
    
    # 提示输入版本号
    VERSION=$(prompt_version)
    echo
    
    # 确认开始执行
    read -p "是否继续执行部署流程？[y/N] " confirm
    if [[ ! "$confirm" =~ ^[Yy]$ ]]; then
        echo "用户取消了部署操作"
        exit 0
    fi
    echo
    
    # 开始执行部署流程
    change_to_work_dir
    echo
    
    build_dataasset "$VERSION"
    echo
    
    build_other_projects
    echo
    
    manage_container "dataasset-api-prd"
    echo
    
    deploy_dataasset_api
    echo
    
    deploy_other_services
    echo
    
    show_status
    echo
    
    # 部署完成
    echo "========================================"
    echo "部署流程全部完成！"
    echo "版本号: $VERSION"
    echo "日志文件: $LOG_FILE"
    echo "========================================"
}

# 执行主函数
main 2>&1 | tee -a "$LOG_FILE"
