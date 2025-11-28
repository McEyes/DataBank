# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY ["ITPortal.AuthServer.Web.Entry.csproj", "."]
RUN dotnet restore "./ITPortal.AuthServer.Web.Entry.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "ITPortal.AuthServer.Web.Entry.csproj" -c Release -o /app/build

# 发布阶段
FROM build AS publish
RUN dotnet publish "ITPortal.AuthServer.Web.Entry.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 安装Kerberos客户端（支持Negotiate认证）
RUN apt-get update && apt-get install -y --no-install-recommends \
    krb5-user \
    && rm -rf /var/lib/apt/lists/*

# 复制Kerberos配置
COPY krb5.conf /etc/krb5.conf

# 暴露端口
EXPOSE 6001

# 启动应用
USER app
ENTRYPOINT ["dotnet", "ITPortal.AuthServer.Web.Entry.dll"]
    