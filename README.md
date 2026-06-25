# Macro Deck
![VersionBadge](https://img.shields.io/github/v/release/Macro-Deck-org/Macro-Deck)
![LicenseBadge](https://img.shields.io/github/license/Macro-Deck-org/Macro-Deck)
![PlatformBadge](https://img.shields.io/badge/platform-windows-blue)
![ExtensionStoreBadge](https://img.shields.io/website?down_message=offline&label=Extension%20Store&up_message=online&url=https%3A%2F%2Fmacrodeck.org%2Fextensionstore%2Fextensionstore.php)

## 不止是宏按键面板！

- 开源免费
- 插件支持
- 图标包
- [Web 客户端](http://web.macrodeck.org)
- 内置包管理器（可下载插件和图标包）
- [变量系统](https://github.com/SuchByte/Macro-Deck/wiki/Variables)
- [Cottle 模板引擎](https://cottle.readthedocs.io/en/stable/page/03-builtin.html)
- 多配置文件
- 无限层级文件夹
- [Discord 社区](https://discord.gg/yr7TRaXum8)


# 开发插件

Macro Deck API 已发布至 NuGet 平台，包名为
[`SuchByte.MacroDeck`](https://www.nuget.org/packages/SuchByte.MacroDeck)。
请在您的插件项目中添加此包引用，代替直接引用 `Macro Deck 2.dll` 文件：

```
dotnet add package SuchByte.MacroDeck
```

此包仅为**编译期引用**——运行时由 Macro Deck 宿主提供程序集，因此不会被复制到插件的输出目录。

# 手机伴侣 App
[仓库地址](https://github.com/Macro-Deck-App/Macro-Deck-Client-App)

[Google Play](https://play.google.com/store/apps/details?id=com.suchbyte.macrodeck)

[App Store](https://apps.apple.com/de/app/macro-deck-client/id6475241728)

# 下载
[官方网站](https://macro-deck.app)

[GitHub Releases](https://github.com/Macro-Deck-App/Macro-Deck/releases)

# 特别鸣谢

JetBrains 系列产品许可证由 [JetBrains](https://www.jetbrains.com/) 友情提供

Macro Deck 使用了 [Material Design Icons](https://materialdesignicons.com/) 提供的免费图标
