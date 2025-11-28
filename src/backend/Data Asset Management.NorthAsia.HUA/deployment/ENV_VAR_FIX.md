# 环境变量配置修复说明

## 问题描述

在部署itportal.search项目时，发现容器无法通过端口7006访问。通过对比容器配置发现以下问题：

### 错误配置
- `ASPNETCORE_URLS`环境变量被错误地作为命令参数传递
- 容器的`Cmd`字段包含`-e`参数
- 容器的`Args`字段包含`-e`和环境变量值

### 正确配置
- `ASPNETCORE_URLS`应该出现在容器的`Env`字段中
- `Cmd`字段应该为null
- `Args`字段只应该包含应用程序的入口点（如`ITPortal.Search.Web.Entry.dll`）

## 根本原因

在`deploy_docker.sh`脚本中，构建`docker run`命令时，环境变量选项（`-e`）被添加到了命令的末尾，在镜像名称之后。这导致Docker将这些选项解释为容器的命令参数，而不是`docker run`命令的选项。

## 修复方案

修改了`deploy_docker.sh`脚本中的命令构建逻辑：

1. 将`docker run`命令分为几个部分构建：
   - 固定部分：基础命令和选项
   - 环境变量部分：所有`-e`选项
   - 配置部分：端口映射、容器名称等
   - 镜像名称部分：项目镜像

2. 确保环境变量选项在镜像名称之前添加，这样Docker会正确识别它们作为`docker run`命令的选项。

## 修复代码对比

### 修复前
```bash
local run_command="docker run -d ... $config $project:prd"
run_command+=" -e \"ASPNETCORE_URLS=${urls}\""
```

### 修复后
```bash
local run_command="docker run -d ..."
run_command+=" -e TZ=Asia/Shanghai"
run_command+=" -e \"ASPNETCORE_ENVIRONMENT=Production\""
run_command+=" -e \"ASPNETCORE_URLS=${urls}\""
run_command+=" $config $project:prd"
```

## 验证方法

1. 使用修复后的脚本重新部署itportal.search项目
2. 运行测试脚本验证配置：
   ```bash
   chmod +x test_env_fix.sh
   ./test_env_fix.sh
   ```
3. 测试脚本会检查：
   - `ASPNETCORE_URLS`是否在容器的`Env`中
   - `-e`参数是否不在`Args`中
   - `Cmd`是否为空
   - 容器日志是否正常
   - 端口是否可以访问

## 注意事项

- 确保在所有Docker部署脚本中都使用正确的命令构建顺序
- 环境变量选项（`-e`）必须在镜像名称之前
- 容器配置选项（如`-p`、`--name`等）可以在环境变量之后，但也应该在镜像名称之前
