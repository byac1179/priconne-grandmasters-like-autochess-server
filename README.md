# BYAC 自走棋服务端

用AI模仿《公主连结！Grand Masters》（Priconne Grand Masters / プリコネ！グランドマスターズ）自制的一个自走棋游戏的服务端。这里只包含**服务端部分**，不含客户端、美术资源和数值表。有很多已知可以优化的部分还未处理。

技术栈：.NET 8 + gRPC（MagicOnion）+ MessagePack。

## 目录

```
grpc/Byac.Server        服务进程：匹配、对战、排行榜、账号、GM 接口
grpc/Byac.ServerShared  协议与数据结构（双端共用）
```

## 编译运行

```bash
dotnet build grpc/Byac.Server/Byac.Server.csproj -c Release
dotnet run  --project grpc/Byac.Server/Byac.Server.csproj
```

发布自包含可执行文件：

```bash
dotnet publish grpc/Byac.Server/Byac.Server.csproj -c Release -r win-x64 --self-contained false
```

## 端口

| 端口 | 环境变量 | 默认 | 说明 |
| --- | --- | --- | --- |
| 明文 h2c | `BYAC_H2C_PORT` | 18081 | 客户端连接用的 gRPC 端口，局域网常用 |
| HTTPS | `GSERVER_PORT` | 9444 | 需要 `server.pfx`（或 `GSERVER_CERT` + `GSERVER_KEY`） |
| TLS | `BYAC_TLS_PORT` | 0（关闭） | 运行时加载证书，配 `BYAC_TLS_CERT` / `BYAC_TLS_KEY` |
| GM 页面 | `BYAC_GM_PORT` | 9445 | 默认只绑 127.0.0.1 |
| 指标 | `BYAC_METRICS_PORT` | 见代码 | HTTP1，默认只绑 127.0.0.1 |

## 运行数据

| 项 | 说明 |
| --- | --- |
| `master.json` / `masterdata.bytes` | 数值表，**不随仓库提供**，由使用者自行准备；路径用 `GSERVER_MASTER` / `GSERVER_MASTERDATA` 指定 |
| `BYAC_DATA_DIR` | 账号库、排行榜等运行数据目录 |
| `BYAC_ENC_KEY` | 账号数据加密密钥（base64，32 字节） |
| `BYAC_OWNER_HASH_KEY` | 设备归属 HMAC 密钥；不设置时首次启动自动生成 `owner.key` |

其余可选开关（AI 镜像、帧转储、DEBUG 倍率等）见代码里的 `Environment.GetEnvironmentVariable`。

## 注意

- 明文 h2c 传输**没有加密**，只适合局域网或可信网络；不要把 18081 直接暴露到公网。
- 账号密码登录是可选项：客户端首次连接会自动建档，需要账号体系时再另行配置。
- 本仓仅服务端源码，运行数据、二进制运行件与使用说明另行提供。

## 免责声明

- 本仓库**不包含任何游戏资源**（客户端、图片、音频、模型、数值表等），只有自制的服务端代码。
- 本项目是自制的仿制游戏，**仅供学习与技术交流**，不得用于商业用途。
- 本项目与《公主连结！Grand Masters》（プリコネ！グランドマスターズ / Priconne Grand Masters）的著作权方、运营方无任何关系，也未获得其授权。
- 使用本代码产生的任何后果由使用者自行承担。

## 许可证

[MIT](LICENSE)
