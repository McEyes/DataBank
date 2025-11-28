# 定位到当前目录
$currentDir = Get-Location

# SSH 配置
$server = "cnhuam0itds01"
$username = "3223901"
$remoteDir = "/home/3223901/src/Data Asset Management.NorthAsia.HUA/"

# 提示输入版本号
$release = Read-Host "请输入版本号 (例如 v0.1.2)"

# 选择认证方式
Write-Host "请选择认证方式:" -ForegroundColor Yellow
Write-Host "1. SSH 密钥（默认）"
Write-Host "2. 密码"
$authMethod = Read-Host "请输入数字 [1-2]"
if ($authMethod -eq "2") {
    # 提示输入密码
    $securePassword = Read-Host "请输入 SSH 密码" -AsSecureString
    $password = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
        [Runtime.InteropServices.Marshal]::SecureStringToBSTR($securePassword)
    )
} else {
    $keyPath = "$env:USERPROFILE\.ssh\id_rsa"
    # 检查密钥文件是否存在
    if (!(Test-Path $keyPath)) {
        Write-Host "未找到 SSH 密钥，将生成新密钥..." -ForegroundColor Yellow
        ssh-keygen -t rsa -b 4096 -f $keyPath -N ""
        Write-Host "请将公钥 ($keyPath.pub) 复制到服务器" -ForegroundColor Red
        exit 1
    }
}

# 执行批处理文件清理 bin 和 obj 目录（自动输入回车）
Write-Host "正在清理 bin 和 obj 目录..." -ForegroundColor Cyan
$batFile = Join-Path $currentDir "Delete-BIN-OBJ-Folders.bat"
if (Test-Path $batFile) {
    try {
        # 使用 echo 和管道模拟输入回车
        echo | cmd /c $batFile
        Write-Host "清理完成!" -ForegroundColor Green
    } catch {
        Write-Host "清理失败: $_" -ForegroundColor Red
        exit 1
    }
} else {
    Write-Host "错误: 未找到 Delete-BIN-OBJ-Folders.bat 文件!" -ForegroundColor Red
    exit 1
}

# 复制文件到 Linux 服务器
Write-Host "正在上传文件到 $server..." -ForegroundColor Cyan
try {
    if ($authMethod -eq "2") {
        # 尝试使用 plink
        $plinkPath = "C:\Program Files\PuTTY\plink.exe"
        if (Test-Path $plinkPath) {
            & $plinkPath -batch -pw $password -scp -r "$currentDir\." "${username}@${server}:${remoteDir}"
        } else {
            # 尝试使用 sshpass
            $sshpassPath = "C:\Program Files\sshpass\sshpass.exe"
            if (Test-Path $sshpassPath) {
                & $sshpassPath -p $password scp -o StrictHostKeyChecking=no -r "$currentDir\." "${username}@${server}:${remoteDir}"
            } else {
                Write-Host "无法自动传递密码，请手动输入..." -ForegroundColor Yellow
                scp -r "$currentDir\." "${username}@${server}:${remoteDir}"
            }
        }
    } else {
        # 使用 SSH 密钥
        scp -i $keyPath -r "$currentDir\." "${username}@${server}:${remoteDir}"
    }
    Write-Host "文件上传完成!" -ForegroundColor Green
} catch {
    Write-Host "上传失败: $_" -ForegroundColor Red
    exit 1
}

# 构建并执行 SSH 命令
Write-Host "正在服务器上执行 Docker 命令..." -ForegroundColor Cyan

$commands = @"
cd $remoteDir
docker tag dataasset:dev dataasset:dev_$release
docker build -f Dockerfile.DataAsset -t dataasset:dev .
docker tag itportal.auth:dev itportal.auth:dev_$release
docker build -f Dockerfile.Auth -t itportal.auth:dev .
docker tag itportal.flow:dev itportal.flow:dev_$release
docker build -f Dockerfile.Flow -t itportal.flow:dev .
echo "部署完成! 版本号: $release"
"@

# 执行 SSH 命令
try {
    if ($authMethod -eq "2") {
        if (Test-Path $plinkPath) {
            & $plinkPath -batch -pw $password "$username@$server" "$commands"
        } else {
            if (Test-Path $sshpassPath) {
                & $sshpassPath -p $password ssh -o StrictHostKeyChecking=no "$username@$server" "$commands"
            } else {
                ssh "$username@$server" "$commands"
            }
        }
    } else {
        ssh -i $keyPath "$username@$server" "$commands"
    }
} catch {
    Write-Host "执行命令失败: $_" -ForegroundColor Red
    exit 1
}

Write-Host "所有操作已完成!" -ForegroundColor Green