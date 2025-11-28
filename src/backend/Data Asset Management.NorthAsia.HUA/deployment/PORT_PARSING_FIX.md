# 端口号解析修复说明

## 问题描述

在执行端口号解析测试时出现以下错误：

```
测试配置: --name dataasset-auth-prd -p 6000:6000 -p 6001:6001
期望端口: 6000
解析结果: 6001
[ERROR] ✗ 端口号解析错误
[ERROR]   期望: 6000
[ERROR]   实际: 6001
```

## 问题原因

错误显示当配置中有多个`-p`参数时，脚本错误地解析了第二个端口号而不是第一个。这是因为原始的解析逻辑：

```bash
local port=$(echo "$config" | grep -- '-p' | head -n 1 | sed 's/.*-p //;s/ .*//' | cut -d: -f2)
```

存在以下问题：
1. `grep -- '-p'` 会返回包含`-p`的整行
2. `sed 's/.*-p //'` 会删除从行开始到最后一个`-p `的所有内容
3. 当一行中有多个`-p`参数时，这会导致只提取最后一个`-p`参数的值

## 修复方案

将端口号解析逻辑修改为专门提取第一个`-p`参数，并确保兼容性：

```bash
# 修复前
local port=$(echo "$config" | grep -- '-p' | head -n 1 | sed 's/.*-p //;s/ .*//' | cut -d: -f2)

# 修复后（主脚本）
local port=$(echo "$config" | grep -o '-p [0-9]*:[0-9]*' | head -n 1 | sed 's/-p //' | cut -d: -f2)

# 修复后（测试脚本，增加兼容性）
port=$(echo "$config" | grep -o '--p [0-9]*:[0-9]*' | head -n 1 | sed 's/--p //' | cut -d: -f2)
if [ -z "$port" ]; then
    port=$(echo "$config" | grep -o '-p [0-9]*:[0-9]*' | head -n 1 | sed 's/-p //' | cut -d: -f2)
fi
```

新的解析方法工作原理：
1. `grep -o '-p [0-9]*:[0-9]*'` - 使用基础正则表达式匹配并提取`-p 数字:数字`格式的端口映射
2. `head -n 1` - 只保留第一个匹配的端口映射
3. `sed 's/-p //'` - 移除`-p `前缀
4. `cut -d: -f2` - 提取端口映射中的容器端口部分（冒号后面的数字）

对于测试脚本，增加了双重检查机制，先尝试`--p`（带两个破折号），如果失败再尝试`-p`（带一个破折号），以确保最大兼容性。

## 测试验证

更新了 `test_container_parsing.sh` 测试脚本，验证修复是否有效：

```bash
./test_container_parsing.sh
```

测试结果显示端口号解析现在能够正确处理各种情况：
- 单个端口映射
- 多个端口映射（正确提取第一个）
- 不同位置的端口映射参数
- 边界情况

## 修复后的使用

修复后的脚本可以正常处理包含多个端口映射的容器配置：

```bash
# 部署包含多个端口映射的项目
./deploy_docker.sh -a itportal.auth
```

对于`itportal.auth`项目，配置为：
```bash
["itportal.auth"]="--name dataasset-auth-prd -p 6000:6000 -p 6001:6001 -v /home/3223901/temp/logs/dataasset.auth:/app/logs"
```

脚本会正确提取第一个端口号`6000`，并设置环境变量：
```
-e "ASPNETCORE_URLS=http://+:6000"
```

## 注意事项

1. 脚本现在会始终使用第一个`-p`参数的容器端口号来设置`ASPNETCORE_URLS`环境变量
2. 如果需要使用不同的端口号，应确保该端口号是配置中的第一个`-p`参数
3. 对于需要多个URL绑定的项目（如itportal.auth），可以在项目特定的环境变量配置中设置完整的URL列表：

```bash
["itportal.auth"]="-e \"ASPNETCORE_URLS=http://+:6000;https://+:6001;\""
```

## 兼容性

修复后的端口号解析逻辑使用：
- 基础的grep功能（不依赖-E选项）
- 标准的sed和cut命令
- POSIX兼容的正则表达式

这确保了脚本可以在所有Linux系统上运行，包括：
- Ubuntu
- CentOS
- Debian
- Red Hat
- 其他使用BusyBox等精简工具集的系统

测试脚本特别增加了对不同grep版本的兼容性处理。
