#!/bin/bash
set -eo pipefail

# ==============================================================================
# 多端口映射测试脚本
# 用于验证多个端口映射时环境变量配置是否正确
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

# 测试多端口解析函数
test_multi_port_parsing() {
    echo "========================================"
    echo "测试多端口映射解析功能"
    echo "========================================"
    
    # 定义测试函数（模拟脚本中的逻辑）
    generate_urls() {
        local config="$1"
        
        # 提取所有容器端口号
        # 使用兼容的方法，不依赖grep -o选项
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
            
            echo "$urls"
        fi
    }
    
    # 测试配置
    test_configs=(
        "--name test1 -p 7001:6001"
        "--name test2 -p 6000:6000 -p 6001:6001"
        "--name test3 -p 8080:80 -p 8443:443 -p 8081:81"
        "-v /data:/app/data --name test4 -p 9000:9000 -p 9001:9001"
        "--name test5"  # 没有端口映射
    )
    
    # 期望的URL列表
    expected_urls=(
        "http://+:6001"
        "http://+:6000;http://+:6001"
        "http://+:80;http://+:443;http://+:81"
        "http://+:9000;http://+:9001"
        ""
    )
    
    # 测试每个配置
    for i in "${!test_configs[@]}"; do
        config="${test_configs[$i]}"
        expected="${expected_urls[$i]}"
        
        echo -e "\n测试配置: $config"
        echo "期望URLs: $expected"
        
        # 生成URLs
        urls=$(generate_urls "$config")
        
        echo "生成URLs: $urls"
        
        if [ "$urls" = "$expected" ]; then
            success "✓ URL生成正确"
        else
            error "✗ URL生成错误"
            error "  期望: $expected"
            error "  实际: $urls"
            return 1
        fi
    done
    
    return 0
}

# 测试docker run命令构建
test_run_command_build() {
    echo -e "\n========================================"
    echo "测试docker run命令构建"
    echo "========================================"
    
    # 定义测试函数（模拟脚本中的逻辑）
    build_run_command() {
        local project="$1"
        local config="$2"
        
        local run_command="docker run -d --restart=always --privileged -v /etc/hosts:/tmp/hosts:ro -e TZ=Asia/Shanghai -e \"ASPNETCORE_ENVIRONMENT=Production\" $config $project:prd"
        
        if [ "$project" = "itportal.auth" ]; then
            # 为auth项目特殊处理，支持http和https
            run_command+=' -e "ASPNETCORE_URLS=http://+:6000;https://+:6001;"'
        else
            # 提取所有容器端口号
            # 使用兼容的方法，不依赖grep -o选项
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
        
        echo "$run_command"
    }
    
    # 测试场景
    test_scenarios=(
        "test_project --name test1 -p 7001:6001"
        "itportal.auth --name auth1 -p 6000:6000 -p 6001:6001"
        "multi_port_project --name multi1 -p 8080:80 -p 8443:443"
        "no_port_project --name no_port1"
    )
    
    # 期望的命令片段
    expected_snippets=(
        '-e "ASPNETCORE_URLS=http://+:6001"'
        '-e "ASPNETCORE_URLS=http://+:6000;https://+:6001;"'
        '-e "ASPNETCORE_URLS=http://+:80;http://+:443"'
        ''  # 没有URL环境变量
    )
    
    # 测试每个场景
    for i in "${!test_scenarios[@]}"; do
        scenario="${test_scenarios[$i]}"
        expected_snippet="${expected_snippets[$i]}"
        
        # 解析项目和配置
        project=$(echo "$scenario" | awk '{print $1}')
        config=$(echo "$scenario" | cut -d' ' -f2-)
        
        echo -e "\n测试场景: 项目=$project, 配置=$config"
        echo "期望片段: $expected_snippet"
        
        # 构建命令
        command=$(build_run_command "$project" "$config")
        
        # 检查是否包含期望的片段
        if [ -n "$expected_snippet" ]; then
            if [[ "$command" == *"$expected_snippet"* ]]; then
                success "✓ 命令包含正确的URL环境变量"
            else
                error "✗ 命令缺少或包含错误的URL环境变量"
                error "  命令: $command"
                return 1
            fi
        else
            if [[ "$command" == *"ASPNETCORE_URLS"* ]]; then
                error "✗ 命令不应包含URL环境变量但却包含了"
                error "  命令: $command"
                return 1
            else
                success "✓ 命令正确地不包含URL环境变量"
            fi
        fi
    done
    
    return 0
}

# 主测试函数
main() {
    echo "========================================"
    echo "多端口映射功能测试脚本"
    echo "========================================"
    
    # 运行所有测试
    if ! test_multi_port_parsing; then
        error "多端口解析测试失败！"
        exit 1
    fi
    
    if ! test_run_command_build; then
        error "docker run命令构建测试失败！"
        exit 1
    fi
    
    echo -e "\n========================================"
    success "所有测试通过！多端口映射功能正常。"
    echo -e "========================================${NC}"
}

# 执行主函数
main
