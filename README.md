# dotnetCampus.SourceLocalizations

| Build | NuGet |
|--|--|
|![](https://github.com/dotnet-campus/dotnetCampus.SourceLocalizations/workflows/.NET%20Core/badge.svg)|[![](https://img.shields.io/nuget/v/dotnetCampus.SourceLocalizations.svg)](https://www.nuget.org/packages/dotnetCampus.SourceLocalizations)|

dotnetCampus.SourceLocalizations is a source generator that can convert text localization files (e.g. .toml) into C# code and provide strong type support for localization keys.

## Features

```csharp
static void Main()
{
    Console.WriteLine(LocalizedText.Current.App.Title);         // "Hello, World!"
    Console.WriteLine(LocalizedText.Current.App.Description);   // "This is a sample application."
    Console.WriteLine(LocalizedText.Current.Cli.Usage);         // "Usage: dotnetCampus.SourceLocalizations [options]"
    Console.WriteLine(LocalizedText.Current.PressAnyKeyToExit); // "Press any key to exit..."
}
```

- Source Generators
    - [x] Generate C# codes
    - [ ] Generate properties for implementation types (so that reflections on types can get localized properties which is very important for WPF Bindings)
    - [ ] Generate localized types for each language item which contains more than one arguments (This fixes different argument orders among different languages.)
- File formats
    - [x] TOML
    - [x] YAML `🤡 Might be deprecated in the future.`
- UI Frameworks Support
    - [x] Avalonia      `😉 We look forward to your better suggestions.`
    - [ ] MAUI          `😶‍🌫️ Not tested yet`
    - [x] Uno Platform  `😉 We look forward to your better suggestions.`
    - [ ] Wpf           `😶‍🌫️ Not tested yet`
- Diagnostics Analyzers and Code Fixes
    - [ ] Detect (and generate) missing localization keys
    - [ ] Detect (and remove) unused localization keys
    - [ ] Detect arguments mismatch among localized texts (e.g. `Hello, {name:string}` in en but `こんにちは、{errorCode:int}` in ja)
    - [ ] Detect invalid IETF language tags and report errors

## Installation

[![](https://img.shields.io/nuget/v/dotnetCampus.SourceLocalizations.svg)](https://www.nuget.org/packages/dotnetCampus.SourceLocalizations)

```shell
dotnet add package dotnetCampus.SourceLocalizations
```

## Usage

### 1. Create localization files

```toml
// Localizations/en.toml
App.Title = "Hello, World!"
App.Description = "This is a sample application."
Cli.Usage = "Usage: dotnetCampus.SourceLocalizations [options]"
PressAnyKeyToExit = "Press any key to exit..."
```

```toml
// Localizations/zh-hans.toml
App.Title = "你好，世界！"
App.Description = "这是一个示例应用程序。"
Cli.Usage = "用法：dotnetCampus.SourceLocalizations [选项]"
PressAnyKeyToExit = "按任意键退出..."
```

The file name must conform to the [IETF BCP 47 standard](https://en.wikipedia.org/wiki/IETF_language_tag).

Add these files to your project `csproj` file:

```xml
<ItemGroup>
    <AdditionalFiles Include="Localizations\**\*.toml" />
</ItemGroup>
```

### 2. Write a localization class

```csharp
// LocalizedText.cs
using dotnetCampus.SourceLocalizations;

namespace SampleApp;

// The default language is used to generate localization interfaces, so it must be the most complete one.
[LocalizedConfiguration(Default = "en", Current = "zh-hans")]
public partial class LocalizedText;
```

### 3. Use the generated code

```csharp
// Program.cs
static void Main()
{
    Console.WriteLine(LocalizedText.Current.App.Title);         // "Hello, World!"
    Console.WriteLine(LocalizedText.Current.App.Description);   // "This is a sample application."
    Console.WriteLine(LocalizedText.Current.Cli.Usage);         // "Usage: dotnetCampus.SourceLocalizations [options]"
    Console.WriteLine(LocalizedText.Current.PressAnyKeyToExit); // "Press any key to exit..."
}
```

```xml
<!-- Avalonia MainWindow.axaml -->
<TextBlock Text="{Binding App.Title, Source={x:Static l:LocalizedText.Current}}" />
<TextBlock Text="{Binding App.Description, Source={x:Static l:LocalizedText.Current}}" />
```

```xml
<!-- Uno Platform MainPage.xaml -->
<TextBlock Text="{x:Bind l:Lang.Current.App.Title}" />
<TextBlock Text="{x:Bind l:Lang.Current.App.Description}" />
```

```csharp
// Uno Platform MainPage.xaml.cs
using dotnetCampus.Localizations;

namespace dotnetCampus.SampleUnoApp;

public sealed partial class MainPage : Page
{
    public MainPage() => InitializeComponent();

    // IMPORTANT: The Lang property must be public.
    public ILocalizedValues Lang => global::dotnetCampus.SampleUnoApp.Localizations.LocalizedText.Current;
}
```
