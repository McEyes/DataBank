# 常见问题解决指南

## sudo权限问题

### 问题描述
执行脚本时遇到以下错误：
```
sudo: account validation failure, is your account locked?
sudo: a password is required
[ERROR] 备份失败！请检查源目录和权限。
```

### 可能原因
1. 用户账号被锁定
2. sudoers配置问题
3. 用户没有sudo权限
4. 需要输入密码但脚本无法交互获取

## 解决方案

### 方案1：使用修复后的脚本版本
我们已经更新了脚本，添加了智能sudo使用机制：

```bash
# 完整版脚本（推荐）
./deploy_site.sh

# 简化版脚本
./deploy_simple.sh

# 回滚脚本
./rollback.sh
```

这些脚本会：
- 首先尝试不使用sudo执行操作
- 如果失败，再尝试使用sudo
- 提供更详细的错误信息

### 方案2：使用无sudo版本
如果sudo完全不可用，可以使用专门的无sudo版本：

```bash
./deploy_no_sudo.sh
```

**注意**：此版本需要用户对所有相关目录有读写权限。

### 方案3：手动检查和设置权限

#### 1. 检查目录权限
```bash
# 检查目标目录权限
ls -ld /usr/local/app/websites/dataops/

# 检查源目录权限
ls -ld "/home/3223901/src/Data Asset Management.NorthAsia.HUA/WebApp/DataOps.Web/"

# 检查当前用户
whoami
```

#### 2. 临时提升权限
如果有管理员权限，可以临时切换到root用户：
```bash
sudo su -
# 然后执行脚本
./deploy_site.sh
```

#### 3. 检查sudo配置
```bash
# 检查sudo是否可用
sudo -l

# 检查sudoers文件
cat /etc/sudoers | grep -E '^[^#]'
```

#### 4. 修复账号锁定
如果账号被锁定，可以尝试：
```bash
# 检查账号状态
passwd -S $(whoami)

# 如果被锁定，需要管理员解锁
# sudo passwd -u $(whoami)
```

### 方案4：修改目录所有权
如果有权限，可以修改相关目录的所有权：
```bash
# 注意：需要root权限执行
sudo chown -R $(whoami):$(whoami) /usr/local/app/websites/dataops/
sudo chown -R $(whoami):$(whoami) "/home/3223901/src/Data Asset Management.NorthAsia.HUA/WebApp/DataOps.Web/"
```

## 不同版本脚本的选择

| 脚本版本 | 使用场景 | 特点 |
|---------|---------|------|
| deploy_site.sh | 推荐，适用于大多数情况 | 智能sudo使用，详细错误处理，完整功能 |
| deploy_simple.sh | 简单部署需求 | 简化功能，智能sudo使用 |
| deploy_no_sudo.sh | sudo不可用的环境 | 完全不使用sudo，需要用户有足够权限 |
| rollback.sh | 部署失败时恢复 | 从备份恢复网站，智能sudo使用 |

## 最佳实践

1. **测试环境验证**：在生产环境使用前，先在测试环境验证脚本
2. **权限检查**：执行前检查所有目录的权限
3. **备份重要数据**：执行部署前手动备份重要数据
4. **日志记录**：查看脚本生成的日志文件进行问题排查
5. **逐步执行**：如果复杂操作，可以分步骤手动执行，确认每一步成功

## 联系支持

如果以上问题无法解决，请，请提供以下信息寻求帮助：
- 完整的错误信息
- 执行的脚本命令
- 目录权限信息
- sudo配置信息
