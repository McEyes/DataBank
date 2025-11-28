
## docker 发布命令

```bash

# 根目录
cd /home/3223901/src/Data Asset Management.NorthAsia.HUA

# 构建image
docker build -t dataasset -f "MicroServices/DataAssetManager.DataApiServer/DataAssetManager.DataApiServer.Web.Entry/Dockerfile.prd" .

# 重新创建docker
docker rm -f dataasset

#部署服务
docker run -d \
--restart=always \
--name dataasset-api \
-p 6001:6001 \
--privileged \
-e "ASPNETCORE_URLS=http://+:6001" \
-e "ASPNETCORE_ENVIRONMENT=Production" \
dataasset:latest
```