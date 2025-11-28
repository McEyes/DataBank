# deploy.ps1 - 一键部署脚本
param (
    [Parameter(Mandatory=$true)]
    [string]$ServerName = "app01",
    
    [Parameter(Mandatory=$true)]
    [string]$AppName = "dataassetmanager",
    
    [Parameter(Mandatory=$true)]
    [string]$ReleaseNumber,
    
    [Parameter(Mandatory=$false)]
    [string]$DockerfilePath = ".\Dockerfile.prd",
    
    [Parameter(Mandatory=$false)]
    [string]$SourcePath = ".\",
    
    [Parameter(Mandatory=$false)]
    [string]$RemoteDeployPath = "/opt/deployments"
)

# 设置远程服务器路径
$remoteAppPath = "$RemoteDeployPath/$AppName-$ReleaseNumber"
$remoteDockerfilePath = "$remoteAppPath/Dockerfile"

# 确保 Podman 已安装
Write-Host "检查 Podman 安装..." -ForegroundColor Cyan
try {
    podman --version | Out-Null
    Write-Host "Podman 已安装" -ForegroundColor Green
} catch {
    Write-Host "错误: Podman 未安装或无法访问" -ForegroundColor Red
    exit 1
}

# 检查 SSH 连接
Write-Host "验证 SSH 连接到 $ServerName..." -ForegroundColor Cyan
try {
    ssh $ServerName "echo SSH 连接成功" | Out-Null
    Write-Host "SSH 连接验证成功" -ForegroundColor Green
} catch {
    Write-Host "错误: 无法通过 SSH 连接到 $ServerName" -ForegroundColor Red
    Write-Host "请确保已配置 SSH 密钥或密码认证" -ForegroundColor Yellow
    exit 1
}

# 创建远程目录结构
Write-Host "在 $ServerName 上创建部署目录..." -ForegroundColor Cyan
ssh $ServerName "mkdir -p $remoteAppPath"

# 复制源代码到远程服务器
Write-Host "复制源代码到 $ServerName..." -ForegroundColor Cyan
$sourceFiles = Get-ChildItem -Path $SourcePath -Recurse | Where-Object { 
    $_.FullName -notmatch "\.git|bin|obj|node_modules" 
}

foreach ($file in $sourceFiles) {
    $relativePath = $file.FullName.Substring($SourcePath.Length)
    $remoteFilePath = "$remoteAppPath/$relativePath"
    $remoteDir = Split-Path -Path $remoteFilePath -Parent
    
    # 创建远程目录（如果不存在）
    ssh $ServerName "mkdir -p $remoteDir"
    
    # 复制文件
    scp $file.FullName "$ServerName`:$remoteFilePath" | Out-Null
}

# 复制 Dockerfile
Write-Host "复制 Dockerfile 到 $ServerName..." -ForegroundColor Cyan
scp $DockerfilePath "$ServerName`:$remoteDockerfilePath" | Out-Null

# 构建并部署 Docker 镜像
Write-Host "在 $ServerName 上构建 Docker 镜像..." -ForegroundColor Cyan
$dockerBuildCmd = "cd $remoteAppPath && podman build -t $AppName`:$ReleaseNumber ."
ssh $ServerName $dockerBuildCmd

# 停止并删除旧容器（如果存在）
Write-Host "停止并删除旧容器（如果存在）..." -ForegroundColor Cyan
$stopContainerCmd = "podman stop $AppName && podman rm $AppName"
ssh $ServerName $stopContainerCmd

# 运行新容器
Write-Host "启动新容器..." -ForegroundColor Cyan
$runContainerCmd = "podman run -d --name $AppName -p 6001:6001 $AppName`:$ReleaseNumber"
ssh $ServerName $runContainerCmd

# 验证部署
Write-Host "验证部署状态..." -ForegroundColor Cyan
$statusCmd = "podman ps | grep $AppName"
$deploymentStatus = ssh $ServerName $statusCmd

if ($deploymentStatus) {
    Write-Host "部署成功! 应用 $AppName (版本 $ReleaseNumber) 已在 $ServerName 上运行" -ForegroundColor Green
    Write-Host "容器状态: $deploymentStatus" -ForegroundColor Green
} else {
    Write-Host "部署可能失败。请检查服务器日志以获取更多信息" -ForegroundColor Red
}