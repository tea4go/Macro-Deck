## 1. 系统概述
Macro Deck 的前端界面采用 **双轨制** 架构：
1. **桌面管理端 (Desktop Host)**：基于 **Windows Forms**，通过完全自定义的控件绘制逻辑实现统一的深色主题（Dark Mode）和圆角（Rounded）视觉风格。
2. **移动端/Web 客户端 (Web Client)**：基于 **Ionic/Angular** 框架，使用 CSS 变量和 Material Design Icons 实现响应式深色界面。

## 2. 桌面端样式体系 (Windows Forms)
由于 Windows Forms 原生不支持现代 UI 风格，项目通过继承并重写 `OnPaint` 方法实现了全套自定义控件。

### 2.1 设计令牌 (Design Tokens)
核心颜色定义在 `src/MacroDeck/GUI/Colors.cs` 中：
- **背景色 (Background)**: `#1C1C1C` (RGB: 28, 28, 28)
- **表面色 (Surface)**: `#262626` - `#3E3E3E` (多级层级)
- **强调色 (Accent)**: `#007BFF` (蓝色系)
- **边框色 (Border)**: `#373737`

### 2.2 核心自定义控件
所有自定义控件位于 `src/MacroDeck/GUI/CustomControls/`：
- **RoundedButton / ButtonPrimary**: 支持圆角、进度条显示、GIF 动画背景及悬停效果。使用 `GraphicsPath` 绘制圆角矩形并设置 `Region` 实现裁剪。
- **RoundedPanel / RoundedUserControl**: 提供圆角容器，解决原生 Panel 直角与现代 UI 不协调的问题。
- **RoundedTextBox / RoundedComboBox**: 封装原生输入控件，添加占位符 (Placeholder)、图标支持及圆角边框绘制。
- **Form / DialogForm**: 自定义基类窗口，统一处理字体应用 (`FontManager.Apply`) 和 ESC 键关闭逻辑。

### 2.3 渲染技术
- **抗锯齿**: 广泛使用 `SmoothingMode.AntiAlias` 和 `InterpolationMode.HighQualityBicubic` 确保圆角和图像缩放质量。
- **双缓冲**: 启用 `OptimizedDoubleBuffer` 防止界面闪烁。
- **GDI+ 路径裁剪**: 通过 `GetFigurePath` 计算圆角路径，并赋值给控件的 `Region` 属性实现非矩形点击区域。

## 3. Web 客户端样式体系 (Ionic/Angular)
位于 `src/MacroDeck/wwwroot/client/`，为预编译的 Angular/Ionic 应用。

### 3.1 技术栈
- **框架**: Ionic Framework (基于 Web Components) + Angular。
- **图标**: Material Design Icons (MDI)。
- **样式策略**: 使用 CSS Custom Properties (CSS Variables) 进行主题管理。

### 3.2 主题配置
在 `index.html` 中内联定义了 Ionic 的颜色变量：
- `--ion-color-primary`: `#2d2d2d` (深色主调)
- `--ion-background-color`: 动态绑定至 body 背景。
- 支持 `prefers-color-scheme` 媒体查询适配系统深色模式。

## 4. 开发者规范
1. **桌面端新增控件**：必须继承自 `UserControl` 或现有自定义基类，严禁直接使用原生 `System.Windows.Forms.Button` 等控件，以保持视觉一致性。
2. **颜色引用**：所有硬编码颜色应参考 `GUI.Colors` 类，避免在控件内部直接写死 RGB 值。
3. **字体管理**：所有窗口加载时需调用 `FontManager.Apply(this)` 以确保全局字体统一。
4. **资源释放**：自定义绘制的 `GraphicsPath`、`Pen`、`Brush` 必须在 `OnPaint` 结束后通过 `using` 语句及时释放，防止 GDI 泄漏。