# SuchByte.MacroDeck

用于开发 [Macro Deck](https://github.com/Macro-Deck-App/Macro-Deck) 插件的 API 程序集。

这是一个**编译期引用包**。插件在编译时依赖此 API，但程序集*不会*被复制到插件输出目录——Macro Deck 宿主在运行时已经加载了 `Macro Deck 2.dll`，提供了插件所需的所有功能。

## 使用方法

在您的插件项目中添加此包：

```
dotnet add package SuchByte.MacroDeck
```

引用仅通过 `ref/` 目录分发，因此不会向您的构建输出中生成运行时程序集。

## 相关链接

- [仓库地址](https://github.com/Macro-Deck-App/Macro-Deck)
- [许可证：Apache-2.0](https://github.com/Macro-Deck-App/Macro-Deck/blob/main/LICENSE)
# SuchByte.MacroDeck

API assembly for developing [Macro Deck](https://github.com/Macro-Deck-App/Macro-Deck) plugins.

This is a **compile-time reference package**. Plugins compile against the API, but the
assembly is *not* copied to the plugin output — the Macro Deck host already has
`Macro Deck 2.dll` loaded at runtime and provides everything the plugin needs.

## Usage

Add the package to your plugin project:

```
dotnet add package SuchByte.MacroDeck
```

The reference is shipped under `ref/` only, so no runtime assembly is emitted to your
build output.

## Links

- [Repository](https://github.com/Macro-Deck-App/Macro-Deck)
- [License: Apache-2.0](https://github.com/Macro-Deck-App/Macro-Deck/blob/main/LICENSE)
