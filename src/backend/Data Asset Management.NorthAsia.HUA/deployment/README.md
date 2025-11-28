# Docker容器化应用部署自动化脚本

## 概述

这是一套用于Docker容器化应用部署的自动化脚本，支持多项目构建、版本管理和容器部署功能。

## 脚本文件

### 1. `deploy_docker.sh`（推荐使用）

**功能特点：**
- 支持多个项目的构建和部署
- 灵活的版本号管理（手动输入或自动生成）
- 详细的错误处理和日志记录
- 支持命令行参数配置
- 可选择部署特定项目

**使用方法：**

```bash
# 基本用法：指定版本号部署单个项目
./deploy_docker.sh -v 0.2.26 dataasset

# 自动生成版本号部署多个项目
./deploy_docker.sh -a itportal.flow itportal.search

# 自动确认所有操作
./deploy_docker.sh -y -v 0.3.0 itportal.res

# 列出所有可用项目
./deploy_docker.sh -l

# 使用自定义工作目录
./deploy_docker.sh --work-dir /path/to/workdir -v 0.2.26 dataasset
```

### 2. `deploy_docker_simple.sh`（简化版）

**功能特点：**
- 严格按照用户提供的命令执行
- 交互式版本号输入
- 依次执行所有项目的构建和部署
- 适合简单的部署场景

**使用方法：**

```bash
./deploy_docker_simple.sh
```

## 项目配置

### 支持的项目

当前脚本支持以下项目：

| 项目名称 | Dockerfile | 容器名称 | 端口映射 |
|---------|-----------|---------|---------|
| dataasset | Dockerfile.DataAsset | dataasset-api-prd | 7001:6001 |
| itportal.flow | Dockerfile.Flow | dataasset-flow-prd | 7005:6005 |
| itportal.gateways | Dockerfile.Gateways | - | - |
| itportal.search | Dockerfile.Search | dataasset-search-prd | 7006:6006 |
| itportal.res | Dockerfile.Res | dataasset-res-prd | 7003:6003 |
| itportal.auth | Dockerfile.Auth | dataasset-auth-prd | 6000:6000, 6001:6001 |

### 自定义配置

要添加新的项目或修改现有配置，可以编辑脚本中的以下变量：

1. **PROJECTS**：定义项目名称和对应的Dockerfile
   ```bash
   declare -A PROJECTS=(
       ["项目名称"]="Dockerfile路径"
   )
   ```

2. **CONTAINER_CONFIGS**：定义容器运行配置
   ```bash
   declare -A CONTAINER_CONFIGS=(
       ["项目名称"]="容器运行参数"
   )
   ```

## 部署流程

### 完整版本流程

1. **参数解析**：处理命令行参数和配置
2. **工作目录切换**：切换到指定的工作目录
3. **版本号管理**：
   - 手动输入版本号
   - 或自动生成新版本号（基于最新版本）
4. **项目构建**：
   - 为现有镜像打标签（如果存在）
   - 构建新的Docker镜像
5. **容器部署**：
   - 停止旧容器
   - 删除旧容器
   - 启动新容器
6. **状态检查**：显示部署状态

### 简化版本流程

1. **版本号输入**：提示用户输入版本号
2. **工作目录切换**：切换到指定的工作目录
3. **DataAsset构建**：构建DataAsset项目
4. **其他项目构建**：构建其他所有项目
5. **容器管理**：停止并删除旧容器
6. **服务部署**：部署所有服务
7. **状态显示**：显示部署状态

## 版本号管理

### 手动版本号

使用 `-v` 或 `--version` 选项指定版本号：

```bash
./deploy_docker.sh -v 0.2.26 dataasset
```

### 自动版本号

使用 `-a` 或 `--auto-version` 选项自动生成版本号：

```bash
./deploy_docker.sh -a dataasset
```

自动版本号生成规则：
- 获取项目的最新版本号
- 递增补丁版本号（例如：0.2.5 → 0.2.6）
- 如果没有找到现有版本，使用默认前缀 `0.2.` 并从 0 开始

## 注意事项

1. **权限要求**：
   - 需要Docker权限（通常需要sudo或加入docker组）
   - 对工作目录和日志目录有读写权限

2. **数据安全**：
   - 确保所有挂载目录存在且有适当权限
   - 重要数据建议备份

3. **网络配置**：
   - 确保端口不冲突
   - 如需自定义网络，请修改容器运行参数

4. **环境变量**：
   - 脚本设置了基本的环境变量
   - 如需添加自定义环境变量，请修改容器运行参数

## 扩展建议

1. **添加测试功能**：在部署前运行测试
2. **远程部署**：支持SSH远程服务器部署
3. **镜像仓库**：添加镜像推送和拉取功能
4. **配置文件**：支持从外部配置文件读取配置
5. **邮件通知**：部署完成后发送邮件通知
6. **监控集成**：与监控系统集成，监控容器状态

## 故障排除

### 常见问题

1. **Docker权限问题**：
   ```bash
   # 将用户添加到docker组
   sudo usermod -aG docker $USER
   # 重新登录生效
   ```

2. **端口冲突**：
   - 检查占用端口的进程：`sudo lsof -i :端口号`
   - 修改脚本中的端口映射配置

3. **目录权限**：
   ```bash
   # 确保日志目录存在并有权限
   mkdir -p /home/3223901/temp/logs
   chmod -R 755 /home/3223901/temp/logs
   ```

4. **镜像构建失败**：
   - 检查Dockerfile是否存在
   - 检查依赖是否正确
   - 查看详细日志：`cat 日志文件`

## 联系方式

如有问题或建议，请联系脚本作者。
