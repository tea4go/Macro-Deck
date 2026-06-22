## 1. 系统与技术栈
Macro Deck 的主机端 GUI 基于 **Windows Forms (WinForms)** 构建，未采用 WPF、MAUI 或现代 Web 前端框架。其视觉风格完全通过**自定义控件（Custom Controls）**和**GDI+ 绘图**实现，摒弃了 Windows 原生控件的默认外观。

- **核心机制**：继承 `Button`, `Panel`, `TextBox`, `ComboBox` 等基础控件，重写 `OnPaint` 方法。
  - 使用 `System.Drawing.Drawing2D.GraphicsPath` 绘制圆角矩形路径。
  - 使用 `Region` 属性裁剪控件区域以实现真正的圆角边界（而非仅视觉圆角）。
  - 启用 `DoubleBuffered` 和 `ControlStyles.OptimizedDoubleBuffer` 以减少闪烁。
- **图标系统**：大量使用嵌入式 PNG/GIF 资源（位于 `Resources/`），并通过 `PictureBox` 或自定义绘制集成到控件中。

## 2. 设计令牌 (Design Tokens)
视觉规范定义在 `src/MacroDeck/GUI/Colors.cs` 中，采用**深色模式（Dark Mode）**为主基调。

| 令牌名称 | 颜色值 (RGB) | 用途 |
| :--- | :--- | :--- |
| `AccentColor` | `0, 123, 255` | 主强调色（蓝色），用于按钮默认背景、进度条 |
| `AccentColorLight` | `20, 143, 255` | 悬停或高亮状态 |
| `AccentColorDark` | `0, 103, 205` | 深色强调，用于进度条填充 |
| `Background` | `28, 28, 28` | 应用最底层背景 |
| `Surface` | `38, 38, 38` | 卡片、面板背景 |
| `Surface2` | `50, 50, 50` | 次级表面 |
| `Surface3` | `62, 62, 62` | 三级表面 |
| `Border` | `55, 55, 55` | 边框颜色 |

- **字体**：全局主要使用 `Tahoma` 字体族。
- **圆角半径**：常用值为 `8px`（小控件）至 `40%` 高度（大按钮/卡片）。

## 3. 关键组件与架构
所有自定义 UI 组件位于 `src/MacroDeck/GUI/CustomControls/` 目录下。

### 核心自定义控件
1.  **ButtonPrimary**: 
    - 扁平化按钮，支持圆角 (`BorderRadius`)。
    - 内置**进度条**功能 (`Progress`, `ProgressColor`)，可直接在按钮上显示加载进度。
    - 支持**加载动画** (`Spinner`)，内置 GIF 动画帧更新逻辑。
    - 悬停效果通过改变背景色实现。
2.  **RoundedButton**: 
    - 继承自 `PictureBox`，主要用于网格布局中的动作按钮。
    - 支持动态圆角裁剪 (`UpdateRegion`)。
    - 集成 **GIF 动画播放器** (`GifAnimator`)，确保背景 GIF 正常播放。
    - 支持前景图层 (`ForegroundImage`) 用于叠加标签或图标。
3.  **RoundedPanel / RoundedUserControl**: 
    - 提供圆角容器，用于包裹内容区域，实现卡片式布局。
4.  **RoundedTextBox / RoundedComboBox**: 
    - 去除原生边框，使用 GDI+ 绘制圆角背景和边框。
    - `RoundedTextBox` 支持内置占位符 (`PlaceHolderText`) 和左侧图标。
5.  **ContentSelectorButton**: 
    - 用于主窗口左侧导航栏，支持选中状态高亮。

### 布局结构
- **MainWindow**: 采用经典的“左侧导航 + 右侧内容”布局。
  - 左侧 `navigation` Panel (宽 60px) 包含垂直排列的 `ContentSelectorButton`。
  - 右侧 `contentPanel` (`BufferedPanel`) 动态加载不同的视图用户控件（如 `DeckView`, `SettingsView`）。

## 4. 开发者规范
1.  **禁止使用原生样式**：不要直接使用 `System.Windows.Forms.Button` 或 `TextBox` 的默认外观。必须使用 `CustomControls` 目录下的对应组件（如 `ButtonPrimary`, `RoundedTextBox`）。
2.  **颜色引用**：严禁硬编码颜色值。必须引用 `SuchByte.MacroDeck.GUI.Colors` 类中的静态属性，以确保主题一致性。
3.  **圆角处理**：若需创建新容器，应继承 `RoundedPanel` 或模仿其 `GetFigurePath` 逻辑，确保 `Region` 被正确设置以裁剪点击区域。
4.  **资源管理**：
    - 自定义控件中创建的 `Pen`, `Brush`, `GraphicsPath` 必须使用 `using` 语句或手动 `Dispose`，防止 GDI 句柄泄漏。
    - 动态生成的 `Image` 对象（如标签位图）需在控件销毁时释放。
5.  **响应式限制**：当前 UI 体系基于固定像素布局（Pixel-based），未实现复杂的流式或自适应响应式系统。调整窗口大小时主要依赖 `Anchor` 和 `Dock` 属性。