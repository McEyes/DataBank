# 版本号处理修复说明

## 问题描述

在执行 `./deploy_docker.sh -a itportal.search` 时出现以下错误：

```
[INFO] 为现有镜像打标签...
Error parsing reference: "itportal.search:prd_v\x1b[0;32m[INFO] 项目 itportal.search 的最新版本: 0.2.3\x1b[0m\n\x1b[0;32m[INFO] 自动生成新版本: 0.2.4\x1b[0m\n0.2.4" is not a valid repository/tag: invalid reference format
[ERROR] 打标签失败: itportal.search:prd -> itportal.search:prd_v[INFO] 项目 itportal.search 的最新版本: 0.2.3
```

## 问题原因

错误显示版本号包含了ANSI颜色代码和额外的信息输出。这是因为在 `generate_new_version` 函数中同时使用了 `info` 函数输出信息和 `echo` 输出版本号：

```bash
generate_new_version() {
    local project="$1"
    local latest_version=$(get_latest_version "$project")
    
    info "项目 $project 的最新版本: $latest_version"  # 这行输出了带颜色的信息
    
    # 解析版本号
    IFS='.' read -r major minor patch <<< "$latest_version"
    
    # 递增补丁版本
    new_patch=$((patch + 1))
    new_version="${major}.${minor}.${new_patch}"
    
    info "自动生成新版本: $new_version"  # 这行也输出了带颜色的信息
    echo "$new_version"  # 这行输出版本号
}
```

当函数被调用时，所有的输出（包括 `info` 函数的带颜色信息和 `echo` 的版本号）都被捕获并作为版本号使用，导致版本号包含了无效字符。

## 修复方案

将信息输出从 `generate_new_version` 函数中移到调用该函数的地方：

1. 修改 `generate_new_version` 函数，使其只输出版本号：

```bash
generate_new_version() {
    local project="$1"
    local latest_version=$(get_latest_version "$project")
    
    # 解析版本号
    IFS='.' read -r major minor patch <<< "$latest_version"
    
    # 递增补丁版本
    new_patch=$((patch + 1))
    new_version="${major}.${minor}.${new_patch}"
    
    echo "$new_version"  # 只输出版本号
}
```

2. 在调用 `generate_new_version` 函数的地方添加信息输出：

```bash
if [ "$AUTO_VERSION" = "true" ]; then
    local latest_version=$(get_latest_version "$project")
    info "项目 $project 的最新版本: $latest_version"
    version=$(generate_new_version "$project")
    info "自动生成新版本: $version"
else
    version="$VERSION"
fi
```

## 测试验证

创建了 `test_version_fix.sh` 测试脚本，验证修复是否有效：

```bash
./test_version_fix.sh
```

测试结果显示版本号处理现在能够正确工作，生成的版本号不包含任何无效字符。

## 修复后的使用

修复后的脚本可以正常使用自动版本号生成功能：

```bash
# 自动生成版本号部署项目
./deploy_docker.sh -a itportal.search

# 自动生成版本号并自动确认
./deploy_docker.sh -y -a itportal.search
```

## 注意事项

1. 确保在使用自动版本号功能时，Docker镜像标签格式正确
2. 版本号应该只包含数字和点（例如：0.2.4）
3. 如果需要手动指定版本号，请使用 `-v` 选项：

```bash
./deploy_docker.sh -v 0.2.4 itportal.search
```
