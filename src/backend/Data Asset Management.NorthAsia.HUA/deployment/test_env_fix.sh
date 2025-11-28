#!/bin/bash

# 测试环境变量修复脚本
# 验证ASPNETCORE_URLS环境变量是否被正确设置，而不是作为命令参数传递

# 颜色定义
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # 无颜色

# 检查是否有正在运行的itportal.search容器
container_id=$(docker ps -q --filter "name=itportal.search")

if [ -z "$container_id" ]; then
    echo -e "${YELLOW}注意: 未找到运行中的itportal.search容器${NC}"
    echo -e "${YELLOW}请先使用修复后的脚本部署itportal.search项目${NC}"
    exit 1
fi

echo -e "${GREEN}找到运行中的itportal.search容器: $container_id${NC}"

# 获取容器的详细信息
echo -e "\n${YELLOW}=== 容器配置详情 ==="
docker inspect "$container_id" | grep -E '"Env":|ASPNETCORE_URLS|Cmd|Args' -A 10

# 检查ASPNETCORE_URLS是否在Env中
echo -e "\n${YELLOW}=== 环境变量检查 ==="
env_check=$(docker inspect "$container_id" | grep -A 10 '"Env":' | grep 'ASPNETCORE_URLS')
if [ -n "$env_check" ]; then
    echo -e "${GREEN}✓ ASPNETCORE_URLS环境变量已正确设置${NC}"
    echo "$env_check"
else
    echo -e "${RED}✗ ASPNETCORE_URLS环境变量未找到${NC}"
fi

# 检查-e参数是否在Args中（应该不在）
echo -e "\n${YELLOW}=== 命令参数检查 ==="
args_check=$(docker inspect "$container_id" | grep -A 10 '"Args":' | grep '\-e')
if [ -z "$args_check" ]; then
    echo -e "${GREEN}✓ -e参数未出现在命令参数中${NC}"
else
    echo -e "${RED}✗ 发现-e参数在命令参数中${NC}"
    echo "$args_check"
fi

# 检查Cmd是否为空
echo -e "\n${YELLOW}=== Cmd配置检查 ==="
cmd_check=$(docker inspect "$container_id" | grep '"Cmd":' | grep -v 'null')
if [ -z "$cmd_check" ]; then
    echo -e "${GREEN}✓ Cmd配置为空（正确）${NC}"
else
    echo -e "${RED}✗ Cmd配置不为空${NC}"
    echo "$cmd_check"
fi

# 检查容器日志
echo -e "\n${YELLOW}=== 容器启动日志 ==="
docker logs "$container_id" 2>&1 | head -20

# 检查端口访问
echo -e "\n${YELLOW}=== 端口访问测试 ==="
# 假设容器映射到7006端口
curl -s -o /dev/null -w "%{http_code}" http://localhost:7006/health || echo "连接失败"

echo -e "\n${YELLOW}=== 总结 ==="
if [ -n "$env_check" ] && [ -z "$args_check" ] && [ -z "$cmd_check" ]; then
    echo -e "${GREEN}✓ 所有检查通过！环境变量配置正确。${NC}"
    echo -e "${GREEN}✓ 项目应该可以正常访问了。${NC}"
else
    echo -e "${RED}✗ 检查未通过！请检查配置。${NC}"
fi
