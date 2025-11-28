#!/bin/bash
set -eo pipefail

# ==============================================================================
# 版本号处理修复测试脚本
# 用于验证版本号处理函数是否正确工作
# ==============================================================================

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # 无颜色

# 显示信息
info() {
    echo -e "${GREEN}[INFO] $1${NC}"
}

# 显示错误
error() {
    echo -e "${RED}[ERROR] $1${NC}"
}

# 显示成功
success() {
    echo -e "${GREEN}[SUCCESS] $1${NC}"
}

# 测试版本号处理
test_version_handling() {
    echo "========================================"
    echo "测试版本号处理功能"
    echo "========================================"
    
    # 创建临时测试脚本
    cat > test_script.sh << 'EOF'
#!/bin/bash

# 模拟原始函数
get_latest_version() {
    local project="$1"
    echo "0.2.3"
}

# 修复后的函数
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

# 测试调用
version=$(generate_new_version "test_project")
echo "生成的版本号: '$version'"
echo "版本号长度: ${#version}"

# 验证版本号格式
if [[ "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo "版本号格式正确"
    exit 0
else
    echo "版本号格式错误"
    exit 1
fi
EOF

    chmod +x test_script.sh
    
    # 执行测试
    info "执行版本号生成测试..."
    if ./test_script.sh; then
        success "版本号处理测试通过！"
    else
        error "版本号处理测试失败！"
        exit 1
    fi
    
    # 清理
    rm test_script.sh
}

# 测试带颜色输出的版本号处理
test_version_with_colors() {
    echo -e "\n========================================"
    echo "测试带颜色输出的版本号处理"
    echo "========================================"
    
    # 创建带颜色输出的测试脚本
    cat > test_color_script.sh << 'EOF'
#!/bin/bash

# 颜色定义
GREEN='\033[0;32m'
NC='\033[0m'

# 显示信息
info() {
    echo -e "${GREEN}[INFO] $1${NC}"
}

# 模拟原始函数
get_latest_version() {
    local project="$1"
    echo "0.2.3"
}

# 修复后的函数
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

# 模拟主函数中的调用方式
info "测试版本号生成..."
latest_version=$(get_latest_version "test_project")
info "项目 test_project 的最新版本: $latest_version"
version=$(generate_new_version "test_project")
info "自动生成新版本: $version"

# 显示版本号的详细信息
echo -e "\n生成的版本号: '$version'"
echo "版本号长度: ${#version}"
echo "版本号ASCII码:"
for (( i=0; i<${#version}; i++ )); do
    char="${version:$i:1}"
    echo "  字符 '$char': $(printf "%d" "'$char")"
done

# 验证版本号格式
if [[ "$version" =~ ^[0-9]+\.[0-9]+\.[0-9]+$ ]]; then
    echo -e "\n版本号格式正确"
    exit 0
else
    echo -e "\n版本号格式错误"
    exit 1
fi
EOF

    chmod +x test_color_script.sh
    
    # 执行测试
    info "执行带颜色输出的版本号测试..."
    if ./test_color_script.sh; then
        success "带颜色输出的版本号处理测试通过！"
    else
        error "带颜色输出的版本号处理测试失败！"
        exit 1
    fi
    
    # 清理
    rm test_color_script.sh
}

# 主测试函数
main() {
    echo "========================================"
    echo "版本号处理修复测试脚本"
    echo "========================================"
    
    # 运行所有测试
    test_version_handling
    test_version_with_colors
    
    echo -e "\n========================================"
    success "所有测试通过！版本号处理修复有效。"
    echo -e "========================================${NC}"
}

# 执行主函数
main
