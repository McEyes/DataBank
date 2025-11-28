#!/bin/bash
# ==============================================================================
# Docker部署配置文件示例
# 复制此文件为 deploy_config.sh 并根据您的环境修改
# ==============================================================================

# ------------------------------------------------------------------------------
# 基本配置
# ------------------------------------------------------------------------------

# 工作目录
WORK_DIR="/home/3223901/src/Data Asset Management.NorthAsia.HUA/"

# 默认版本号前缀
DEFAULT_VERSION_PREFIX="0.2."

# 日志文件前缀
LOG_FILE_PREFIX="docker_deployment_"

# ------------------------------------------------------------------------------
# 项目配置
# ------------------------------------------------------------------------------

# 项目定义：项目名称 -> Dockerfile路径
declare -A PROJECTS=(
    ["dataasset"]="Dockerfile.DataAsset"
    ["itportal.flow"]="Dockerfile.Flow"
    ["itportal.gateways"]="Dockerfile.Gateways"
    ["itportal.search"]="Dockerfile.Search"
    ["itportal.res"]="Dockerfile.Res"
    ["itportal.auth"]="Dockerfile.Auth"
    # 添加新项目
    # ["新项目名称"]="Dockerfile路径"
)

# ------------------------------------------------------------------------------
# 容器运行配置
# ------------------------------------------------------------------------------

# 容器配置：项目名称 -> 容器运行参数
declare -A CONTAINER_CONFIGS=(
    ["dataasset"]="--name dataasset-api-prd -p 7001:6001 -v /home/3223901/temp/logs/dataasset.api:/app/logs"
    ["itportal.auth"]="--name dataasset-auth-prd -p 6000:6000 -p 6001:6001 -v /home/3223901/temp/logs/dataasset.auth:/app/logs"
    ["itportal.flow"]="--name dataasset-flow-prd -p 7005:6005 -v /home/3223901/temp/logs/dataasset.flow:/app/logs"
    ["itportal.search"]="--name dataasset-search-prd -p 7006:6006 -v /home/3223901/temp/logs/dataasset.search:/app/logs"
    ["itportal.res"]="--name dataasset-res-prd -p 7003:6003 -v /home/3223901/temp/logs/dataasset.res:/app/logs -v /usr/resource/dataops:/app/uploads"
    # 添加新容器配置
    # ["新项目名称"]="容器运行参数"
)

# ------------------------------------------------------------------------------
# 环境变量配置
# ------------------------------------------------------------------------------

# 全局环境变量（所有容器都会使用）
GLOBAL_ENV_VARS=(
    "-e TZ=Asia/Shanghai"
    "-e \"ASPNETCORE_ENVIRONMENT=Production\""
    # 添加更多全局环境变量
)

# 项目特定环境变量
declare -A PROJECT_ENV_VARS=(
    ["itportal.auth"]="-e \"ASPNETCORE_URLS=http://+:6000;https://+:6001;\""
    # 添加项目特定环境变量
    # ["项目名称"]="环境变量参数"
)

# ------------------------------------------------------------------------------
# 容器运行通用参数
# ------------------------------------------------------------------------------

# 所有容器都会使用的通用参数
COMMON_RUN_PARAMS="--restart=always --privileged -v /etc/hosts:/tmp/hosts:ro"

# ------------------------------------------------------------------------------
# 部署后操作
# ------------------------------------------------------------------------------

# 部署后执行的命令
POST_DEPLOY_COMMANDS=(
    # "echo 部署完成"
    # "docker system prune -f"  # 清理无用镜像
    # 添加更多部署后命令
)

# ------------------------------------------------------------------------------
# 版本号管理配置
# ------------------------------------------------------------------------------

# 版本号格式正则表达式
VERSION_PATTERN="^[0-9]+\.[0-9]+\.[0-9]+$"

# 自动版本号递增策略：major, minor, patch
AUTO_VERSION_STRATEGY="patch"

# ------------------------------------------------------------------------------
# 日志配置
# ------------------------------------------------------------------------------

# 日志级别：info, warn, error, debug
LOG_LEVEL="info"

# 日志保留天数
LOG_RETENTION_DAYS=7

# ------------------------------------------------------------------------------
# 安全配置
# ------------------------------------------------------------------------------

# 是否允许删除运行中的容器
ALLOW_DELETE_RUNNING_CONTAINER="true"

# 是否在部署前备份数据
BACKUP_BEFORE_DEPLOY="false"

# 备份目录
BACKUP_DIR="/home/3223901/backups"
