# 部署参数
# 自动获取当前脚本所在目录作为项目路径
$ProjectPath = $PSScriptRoot
$DockerHost = "tcp://cnhuam0itds01:9001"
$ImageName = "itportal.flow"
$ImageTag = "latest"
$ContainerName = "itportal.flow-container"
$PortMapping = "6005:6005"

# 设置Docker环境变量指向远程服务器
$env:DOCKER_HOST = $DockerHost

# 导航到项目目录
Set-Location -Path $ProjectPath

# 构建Docker镜像
Write-Host "正在构建Docker镜像..."
docker build -t "${ImageName}:${ImageTag}" .

# 检查并删除现有容器
$existingContainer = docker ps -a --filter "name=$ContainerName" --format "{{.Names}}"
if ($existingContainer) {
    Write-Host "正在停止并删除现有容器..."
    docker stop $ContainerName
    docker rm $ContainerName
}

# 运行新容器
Write-Host "正在启动新容器..."
docker run -d -p $PortMapping --name $ContainerName "${ImageName}:${ImageTag}"

Write-Host "部署完成！应用程序已在http://$($DockerHost.Split('://')[1].Split(':')[0]):$($PortMapping.Split(':')[0]) 上运行"
