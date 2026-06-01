# CLAUDE.md - SheepBattle 项目

请使用中文写提案和回答

这是 SheepBattle 联网游戏项目的根目录配置。

---

## 📁 项目结构

```
SheepBattle/
├── Client.Unity/         # Unity 客户端（TEngine 框架）
├── Server.Fantasy/       # C# 服务端（Fantasy.Net 框架）
├── Shared.Config/        # 共享配置（Luban 配置表）
├── Shared.Protocol/      # 共享协议定义
├── Tools.Config/         # 配置工具
├── Tools.Protocol/       # 协议工具
├── Tools.Map/            # 地图工具
└── Deploy/               # 部署相关
```

---

## 🎯 工作模式选择

根据你当前的工作内容，选择对应的工作目录：

| 工作内容 | 推荐目录 | 可用 Skills |
|---------|---------|------------|
| **客户端开发** | `cd Client.Unity` | `tengine-dev`, `html-to-ugui`, `luban-dev` |
| **服务端开发** | `cd Server.Fantasy` | `fantasy-net`, `luban-dev` |
| **配置表编辑** | 当前目录或 `Shared.Config` | `luban-dev` |
| **协议定义** | `Shared.Protocol` | `fantasy-net` |
| **跨模块开发** | 当前目录（SheepBattle） | 所有 skills 可用 |

---

## 🛠️ 可用 Skills

### 客户端相关
- **tengine-dev**: TEngine Unity 框架开发（UI、资源、事件、热更）
- **html-to-ugui**: HTML 原型转 UGUI 界面生成

### 服务端相关
- **fantasy-net**: Fantasy.Net 服务端框架开发（ECS、网络、路由）

### 共享工具
- **luban-dev**: Luban 配置表全栈工具（客户端+服务端通用）
- **wiki-synchelper**: Wiki 文档同步助手

---

## 📚 技术栈

### 客户端（Client.Unity）
- **框架**: TEngine (HybridCLR + YooAsset + UniTask)
- **配置**: Luban
- **语言**: C# (热更新支持)

### 服务端（Server.Fantasy）
- **框架**: Fantasy.Net
- **架构**: ECS + 分布式
- **语言**: C#

### 共享
- **配置系统**: Luban（Excel/JSON → C# 代码生成）
- **协议**: 自定义协议（客户端+服务端共享）

---

## 🚀 快速开始

### 客户端开发
```bash
cd Client.Unity
# 查看客户端专属 CLAUDE.md 获取详细指导
```

### 服务端开发
```bash
cd Server.Fantasy
# 使用 fantasy-net skill 获取框架指导
```

### 配置表编辑
```bash
# 在当前目录使用 luban-dev skill
# 配置文件位于 Shared.Config/
```

---

## ⚠️ 注意事项

1. **客户端特定任务**：请切换到 `Client.Unity` 目录，那里有更详细的 TEngine 开发规范
2. **服务端特定任务**：请切换到 `Server.Fantasy` 目录
3. **跨模块任务**：在当前目录（SheepBattle）工作，所有 skills 都可用
4. **配置表修改**：会同时影响客户端和服务端，需要重新生成代码

---

## 📖 更多文档

- 客户端详细规范：`Client.Unity/CLAUDE.md`
- Skills 参考文档：`.claude/skills/*/references/`
