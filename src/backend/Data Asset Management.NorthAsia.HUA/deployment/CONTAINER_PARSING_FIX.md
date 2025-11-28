# 容器配置解析修复说明

## 问题描述

在执行 `sudo ./deploy_docker.sh -a itportal.search` 时出现以下错误：

```
========================================
  启动容器: itportal.search
========================================
grep: unrecognized option '--name \K\S+'
Usage: grep [OPTION]... PATTERNS [FILE]...
Try 'grep --help' for more information.
[ERROR] 无法从配置中解析容器名称: --name dataasset-search-prd -p 7006:6006 -v /home/3223901/temp/logs/dataasset.search:/app/logs
```

## 问题原因

错误显示grep命令无法识别`--name \K\S+`选项。这是因为脚本中使用了grep的`-P`选项（Perl正则表达式），该选项在某些系统上可能不可用或不被支持：

```bash
# 原始代码
local container_name=$(echo "$config" | grep -oP '--name \K\S+')
local port=$(echo "$config" | grep -oP '-p \K\d+:\d+' | head -n 1 | cut -d: -f2)
```

`-P`选项启用Perl兼容的正则表达式，其中`\K`是一个特殊的正则表达式构造，用于重置匹配的起始点。这个特性在标准的POSIX grep中不可用。

## 修复方案

将依赖Perl正则表达式的解析方法替换为使用标准grep和sed的更兼容方法：

1. **容器名称解析修复**：

```bash
# 修复前
local container_name=$(echo "$config" | grep -oP '--name \K\S+')

# 修复后
local container_name=$(echo "$config" | grep -- '--name' | sed 's/.*--name //;s/ .*//')
```

2. **端口号解析修复**：

```bash
# 修复前
local port=$(echo "$config" | grep -oP '-p \K\d+:\d+' | head -n 1 | cut -d: -f2)

# 修复后
local port=$(echo "$config" | grep -- '-p' | head -n 1 | sed 's/.*-p //;s/ .*//' | cut -d: -f2)
```

新的解析方法工作原理：
- 使用`grep -- '--name'`查找包含`--name`的行
- 使用`sed 's/.*--name //;s/ .*//'`提取`--name`后面的第一个单词
- 对于端口号，还使用`cut -d: -f2`提取端口映射中的容器端口部分

## 测试验证

创建了 `test_container_parsing.sh` 测试脚本，验证修复是否有效：

```bash
./test_container_parsing.sh
```

测试结果显示容器名称和端口号解析现在能够在各种情况下正确工作，包括：
- 标准配置格式
- 多个参数的不同顺序
- 包含特殊字符的容器名称
- 边界情况（空配置、缺少参数等）

## 修复后的使用

修复后的脚本可以正常使用所有功能：

```bash
# 自动生成版本号部署项目
./deploy_docker.sh -a itportal.search

# 部署多个项目
./deploy_docker.sh -y -v 0.2.4 itportal.flow itportal.search

# 查看可用项目
./deploy_docker.sh -l
```

## 注意事项

1. 确保容器配置中包含`--name`参数，否则脚本会报错
2. 确保容器配置中包含`-p`参数（端口映射），否则ASP.NET Core URL环境变量可能无法正确设置
3. 如果需要自定义容器启动参数，请修改`CONTAINER_CONFIGS`数组中的对应项目配置

## 兼容性

修复后的脚本使用标准的Unix工具（grep和sed），应该在所有Linux系统上兼容，包括：
- Ubuntu
- CentOS
- Debian
- Red Hat
- macOS（需要安装GNU sed或调整sed语法）
