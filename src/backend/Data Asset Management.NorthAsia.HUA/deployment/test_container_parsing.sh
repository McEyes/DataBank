#!/bin/bash
set -eo pipefail

# ==============================================================================
# 容器配置解析测试脚本
# 用于验证容器名称和端口号解析功能是否正确工作
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

# 测试容器名称解析
test_container_name_parsing() {
    echo "========================================"
    echo "测试容器名称解析"
    echo "========================================"
    
    # 测试配置
    test_configs=(
        "--name dataasset-api-prd -p 7001:6001 -v /logs:/app/logs"
        "--name dataasset-auth-prd -p 6000:6000 -p 6001:6001"
        "--name my-container-123 -p 8080:80"
        "-v /data:/app/data --name test-container -p 9000:9000"
        "--name special_name.with.dots -p 1234:5678"
    )
    
    # 期望的容器名称
    expected_names=(
        "dataasset-api-prd"
        "dataasset-auth-prd"
        "my-container-123"
        "test-container"
        "special_name.with.dots"
    )
    
    # 测试每个配置
    for i in "${!test_configs[@]}"; do
        config="${test_configs[$i]}"
        expected="${expected_names[$i]}"
        
        echo -e "\n测试配置: $config"
        echo "期望名称: $expected"
        
        # 使用修复后的方法解析容器名称
        container_name=$(echo "$config" | grep -- '--name' | sed 's/.*--name //;s/ .*//')
        
        echo "解析结果: $container_name"
        
        if [ "$container_name" = "$expected" ]; then
            success "✓ 容器名称解析正确"
        else
            error "✗ 容器名称解析错误"
            error "  期望: $expected"
            error "  实际: $container_name"
            return 1
        fi
    done
    
    return 0
}

# 测试端口号解析
test_port_parsing() {
    echo -e "\n========================================"
    echo "测试端口号解析"
    echo "========================================"
    
    # 测试配置
    test_configs=(
        "--name dataasset-api-prd -p 7001:6001 -v /logs:/app/logs"
        "--name dataasset-auth-prd -p 6000:6000 -p 6001:6001"
        "--name my-container -p 8080:80"
        "-v /data:/app/data --name test-container -p 9000:9000"
        "--name web-server -p 80:80 -p 443:443"
    )
    
    # 期望的端口号
    expected_ports=(
        "6001"
        "6000"
        "80"
        "9000"
        "80"
    )
    
    # 测试每个配置
    for i in "${!test_configs[@]}"; do
        config="${test_configs[$i]}"
        expected="${expected_ports[$i]}"
        
        echo -e "\n测试配置: $config"
        echo "期望端口: $expected"
        
        # 使用修复后的方法解析端口号
        # 提取第一个-p参数的端口号，兼容不支持-E选项的grep
        port=$(echo "$config" | grep -o '--p [0-9]*:[0-9]*' | head -n 1 | sed 's/--p //' | cut -d: -f2)
        # 如果上面的命令失败，尝试另一种方法
        if [ -z "$port" ]; then
            port=$(echo "$config" | grep -o '-p [0-9]*:[0-9]*' | head -n 1 | sed 's/-p //' | cut -d: -f2)
        fi
        
        echo "解析结果: $port"
        
        if [ "$port" = "$expected" ]; then
            success "✓ 端口号解析正确"
        else
            error "✗ 端口号解析错误"
            error "  期望: $expected"
            error "  实际: $port"
            return 1
        fi
    done
    
    return 0
}

# 测试边界情况
test_edge_cases() {
    echo -e "\n========================================"
    echo "测试边界情况"
    echo "========================================"
    
    # 测试空配置
    echo -e "\n测试空配置:"
    config=""
    container_name=$(echo "$config" | grep -- '--name' | sed 's/.*--name //;s/ .*//')
    if [ -z "$container_name" ]; then
        success "✓ 空配置处理正确"
    else
        error "✗ 空配置处理错误"
        return 1
    fi
    
    # 测试没有--name的配置
    echo -e "\n测试没有--name的配置:"
    config="-p 8080:80 -v /data:/app/data"
    container_name=$(echo "$config" | grep -- '--name' | sed 's/.*--name //;s/ .*//')
    if [ -z "$container_name" ]; then
        success "✓ 没有--name的配置处理正确"
    else
        error "✗ 没有--name的配置处理错误"
        return 1
    fi
    
    # 测试没有-p的配置
    echo -e "\n测试没有-p的配置:"
    config="--name test-container -v /data:/app/data"
    port=$(echo "$config" | grep -- '-p' | head -n 1 | sed 's/.*-p //;s/ .*//' | cut -d: -f2)
    if [ -z "$port" ]; then
        success "✓ 没有-p的配置处理正确"
    else
        error "✗ 没有-p的配置处理错误"
        return 1
    fi
    
    return 0
}

# 主测试函数
main() {
    echo "========================================"
    echo "容器配置解析测试脚本"
    echo "========================================"
    
    # 运行所有测试
    if ! test_container_name_parsing; then
        error "容器名称解析测试失败！"
        exit 1
    fi
    
    if ! test_port_parsing; then
        error "端口号解析测试失败！"
        exit 1
    fi
    
    if ! test_edge_cases; then
        error "边界情况测试失败！"
        exit 1
    fi
    
    echo -e "\n========================================"
    success "所有测试通过！容器配置解析功能正常。"
    echo -e "========================================${NC}"
}

# 执行主函数
main
