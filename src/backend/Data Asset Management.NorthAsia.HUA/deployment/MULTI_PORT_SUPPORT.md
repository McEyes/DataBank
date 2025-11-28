# 多端口映射支持功能说明

## 功能概述

Docker部署脚本现在支持处理多个端口映射配置，并为每个端口自动生成相应的`ASPNETCORE_URLS`环境变量。这使得应用程序能够在容器中监听多个端口。

## 功能特点

1. **自动识别多个端口映射**：脚本会自动检测容器配置中的所有`-p`参数
2. **生成URL列表**：为每个容器端口生成`http://+:端口号`格式的URL
3. **正确的分隔符**：使用分号(`;`)分隔多个URL
4. **特殊项目支持**：为`itportal.auth`项目保留HTTPS支持
5. **向后兼容**：保持与单端口配置的兼容性

## 工作原理

### 配置解析

脚本会解析容器配置中的所有`-p`参数：

```bash
# 示例配置
"--name multi-port-app -p 8080:80 -p 8443:443 -p 8081:81"
```

### URL生成

对于每个端口映射，脚本会提取容器端口号（冒号后面的数字），并生成URL：

```
-p 8080:80   →   http://+:80
-p 8443:443  →   http://+:443
-p 8081:81   →   http://+:81
```

### 环境变量设置

生成的URL列表会被设置为`ASPNETCORE_URLS`环境变量：

```bash
-e "ASPNETCORE_URLS=http://+:80;http://+:443;http://+:81"
```

### 特殊处理

对于`itportal.auth`项目，保持特殊处理以支持HTTPS：

```bash
-e "ASPNETCORE_URLS=http://+:6000;https://+:6001;"
```

## 使用示例

### 1. 单端口配置（保持兼容）

**配置：**
```bash
["dataasset"]="--name dataasset-api-prd -p 7001:6001 -v /logs:/app/logs"
```

**生成的环境变量：**
```
-e "ASPNETCORE_URLS=http://+:6001"
```

### 2. 多端口配置

**配置：**
```bash
["my-app"]="--name my-app-prd -p 7000:6000 -p 7001:6001 -v /logs:/app/logs"
```

**生成的环境变量：**
```
-e "ASPNETCORE_URLS=http://+:6000;http://+:6001"
```

### 3. 三个或更多端口

**配置：**
```bash
["complex-app"]="--name complex-app-prd -p 8080:80 -p 8443:443 -p 8081:81 -p 8082:82"
```

**生成的环境变量：**
```
-e "ASPNETCORE_URLS=http://+:80;http://+:443;http://+:81;http://+:82"
```

### 4. Auth项目特殊处理

**配置：**
```bash
["itportal.auth"]="--name dataasset-auth-prd -p 6000:6000 -p 6001:6001 -v /logs:/app/logs"
```

**生成的环境变量（特殊处理）：**
```
-e "ASPNETCORE_URLS=http://+:6000;https://+:6001;"
```

## 应用程序配置

为了让应用程序正确处理多个端口，需要确保：

1. **ASP.NET Core应用程序配置**：
   ```csharp
   // Program.cs
   var builder = WebApplication.CreateBuilder(args);
   var app = builder.Build();
   
   // 应用程序会自动使用ASPNETCORE_URLS环境变量中指定的所有URL
   app.Run();
   ```

2. **Kestrel配置**（如需要）：
   ```json
   // appsettings.json
   {
     "Kestrel": {
       "Endpoints": {
         "Http": {
           "Url": "http://+:6000"
         },
         "Https": {
           "Url": "https://+:6001",
           "Certificate": {
             "Path": "/app/cert.pfx",
             "Password": "password"
           }
         }
       }
     }
   }
   ```

## 部署命令示例

```bash
# 部署多端口应用
./deploy_docker.sh -v 0.1.0 my-app

# 自动版本号部署多端口应用
./deploy_docker.sh -a complex-app

# 查看生成的docker run命令（使用--dry-run选项，如果有）
# 注意：实际脚本中没有--dry-run选项，这只是示例
```

## 注意事项

1. **端口映射顺序**：URL列表的顺序与配置中`-p`参数的顺序一致
2. **容器端口vs主机端口**：脚本使用的是容器端口（冒号后面的数字），而不是主机端口
3. **HTTPS支持**：对于需要HTTPS的项目，目前只有`itportal.auth`有特殊处理。其他项目如需HTTPS，需要手动配置或添加新的特殊处理
4. **没有端口映射**：如果配置中没有`-p`参数，脚本不会设置`ASPNETCORE_URLS`环境变量，应用程序将使用默认设置

## 扩展建议

1. **自定义URL格式**：添加配置选项，允许为不同项目自定义URL格式（如添加HTTPS支持）
2. **端口分类**：根据端口号范围自动区分HTTP和HTTPS端口
3. **配置文件支持**：从外部配置文件读取URL格式和端口映射规则
4. **验证功能**：添加端口号验证，确保生成的URL格式正确

## 兼容性

多端口映射功能与以下系统和工具兼容：
- 所有支持Bash的Linux系统
- 所有版本的Docker
- 所有支持`ASPNETCORE_URLS`环境变量的ASP.NET Core版本（2.0及以上）
