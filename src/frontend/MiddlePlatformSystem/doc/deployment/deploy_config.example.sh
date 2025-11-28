#!/bin/bash
# ==============================================================================
# 网站部署配置文件示例
# 复制此文件为 deploy_config.sh 并根据您的环境修改
# ==============================================================================

# ------------------------------------------------------------------------------
# 基本配置
# ------------------------------------------------------------------------------

# 备份根目录
# 所有备份将存储在此目录下的子目录中
BACKUP_ROOT="/var/backups/websites"

# 目标站点目录
# 这是您的网站在服务器上的实际运行目录
TARGET_DIR="/usr/local/app/websites/dataops/"

# 新站点源目录
# 这是包含您要部署的新网站文件的目录
SOURCE_DIR="/home/user/projects/new_website/"

# ------------------------------------------------------------------------------
# 高级配置
# ------------------------------------------------------------------------------

# 保留的备份数量
# 超过此数量的旧备份将被自动删除
MAX_BACKUPS=10

# 日志文件位置
LOG_FILE="/var/log/website_deployment.log"

# 部署前执行的命令
# 可以用于停止服务、维护模式等
PRE_DEPLOY_COMMANDS=(
    # "systemctl stop nginx"
    # "touch /var/www/maintenance.html"
)

# 部署后执行的命令
# 可以用于启动服务、清理缓存等
POST_DEPLOY_COMMANDS=(
    # "systemctl start nginx"
    # "rm /var/www/maintenance.html"
    # "chown -R www-data:www-data /var/www/site"
    # "find /var/www/site -type f -exec chmod 644 {} \;"
    # "find /var/www/site -type d -exec chmod 755 {} \;"
)

# 要排除的文件/目录（相对于源目录）
# 这些文件/目录不会被复制到目标目录
EXCLUDE_FILES=(
    # ".git"
    # "node_modules"
    # "tests"
    # "*.log"
)

# 要保留的目标文件/目录
# 这些文件/目录不会被删除，即使源目录中没有
PRESERVE_FILES=(
    # "config.php"
    # "uploads"
    # "data"
)
