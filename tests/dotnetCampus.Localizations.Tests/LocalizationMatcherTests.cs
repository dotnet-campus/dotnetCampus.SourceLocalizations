namespace dotnetCampus.Localizations.Tests;

[TestClass]
public class LocalizationMatcherTests
{
    [TestMethod("直接精确匹配")]
    public void TC01_ExactMatch()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-Hans-CN",
            ["zh-Hans-CN", "en-US"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("自动转换简体中文区域代码")]
    public void TC02_SimplifiedChineseCodeConversion()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-CN",
            ["zh-Hans-CN", "en"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("旧版代码转换+区域性变体匹配")]
    public void TC03_LegacyCodeConversionWithRegionalVariant()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-CHS",
            ["zh-Hans", "zh-Hans-CN"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("中性文化匹配其区域变体")]
    public void TC04_NeutralCultureMatchesRegionalVariant()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-Hans",
            ["zh-Hans-CN", "zh"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("繁体中文区域匹配")]
    public void TC05_TraditionalChineseRegionMatching()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-TW",
            ["zh-Hant-TW", "zh-Hant"]);

        Assert.AreEqual("zh-Hant-TW", match);
    }

    [TestMethod("区域变体回退到父文化")]
    public void TC06_RegionalVariantFallbackToParentCulture()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-Hans-SG",
            ["zh-Hans-CN", "zh-Hans"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("英语区域回退到其他英语变体")]
    public void TC07_EnglishRegionFallbackToOtherEnglishVariant()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "en-GB",
            ["en-US", "en"]);

        Assert.AreEqual("en-US", match);
    }

    [TestMethod("中性语言代码匹配区域变体")]
    public void TC08_NeutralLanguageCodeMatchesRegionalVariant()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "fr",
            ["fr-FR", "en-US"]);

        Assert.AreEqual("fr-FR", match);
    }

    [TestMethod("无匹配时返回null")]
    public void TC09_NoMatchReturnsNull()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "ja-JP",
            ["en-US"]);

        Assert.IsNull(match);
    }

    [TestMethod("无效输入时按可用列表顺序返回")]
    public void TC10_InvalidInputReturnsFirstAvailable()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "invalid-culture",
            ["zh-Hans-CN", "en-US"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("两字母代码匹配首个可用中性文化")]
    public void TC11_TwoLetterCodeMatchesFirstAvailableNeutralCulture()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh",
            ["zh-Hans", "zh-Hant"]);

        Assert.AreEqual("zh-Hans", match);
    }

    [TestMethod("繁体中文特殊区域匹配")]
    public void TC12_TraditionalChineseSpecialRegionMatching()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-HK",
            ["zh-Hant-MO", "zh-Hant"]);

        Assert.AreEqual("zh-Hant-MO", match);
    }

    [TestMethod("中性代码匹配第一个区域变体")]
    public void TC13_NeutralCodeMatchesFirstRegionalVariant()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "en",
            ["en-AU", "en-CA"]);

        Assert.AreEqual("en-AU", match);
    }

    [TestMethod("区域文化回退到同语言其他区域")]
    public void TC14_RegionalCultureFallbackToSameLanguageOtherRegion()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "es-ES",
            ["es-MX"]);

        Assert.AreEqual("es-MX", match);
    }

    [TestMethod("区域文化回退到中性文化")]
    public void TC15_RegionalCultureFallbackToNeutralCulture()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "ru-RU",
            ["ru"]);

        Assert.AreEqual("ru", match);
    }

    [TestMethod("精确匹配失败后回退到语言代码")]
    public void TC16_ExactMatchFailsFallbackToLanguageCode()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-Hans-CN",
            ["zh", "en"]);

        Assert.AreEqual("zh", match);
    }

    [TestMethod("同时存在中性文化和区域文化时优先匹配区域文化")]
    public void TC17_PreferRegionalCultureOverNeutralWhenBothExist()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-CN",
            ["zh-Hans", "zh-Hans-CN"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("区域文化回退到中性文化")]
    public void TC18_RegionalCultureFallbackToNeutralCulture2()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "en-US",
            ["en"]);

        Assert.AreEqual("en", match);
    }

    [TestMethod("空输入时返回null")]
    public void TC19_EmptyInputReturnsNull()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "",
            ["zh-Hans-CN", "en-US"]);

        Assert.IsNull(match);
    }

    [TestMethod("大小写不敏感匹配")]
    public void TC20_CaseInsensitiveMatching()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "ZH-hans-CN",
            ["zh-Hans-CN"]);

        Assert.AreEqual("zh-Hans-CN", match);
    }

    [TestMethod("区域变体优先于中性文化")]
    public void TC21_RegionalVariantPreferredOverNeutralCulture()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-Hans-CN",
            ["zh-Hans-SG", "zh-Hans"]);

        Assert.AreEqual("zh-Hans-SG", match);
    }

    [TestMethod("繁体中性文化匹配第一个区域变体")]
    public void TC22_TraditionalNeutralCultureMatchesFirstRegionalVariant()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "zh-Hant",
            ["zh-HK", "zh-TW"]);

        Assert.AreEqual("zh-HK", match);
    }

    [TestMethod("德语区域回退到其他德语区域")]
    public void TC23_GermanRegionFallbackToOtherGermanRegion()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "de-DE",
            ["de-AT", "de-CH"]);

        Assert.AreEqual("de-AT", match);
    }

    [TestMethod("葡萄牙语中性代码优先巴西变体")]
    public void TC24_PortugueseNeutralCodePrefersBrazilianVariant()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "pt",
            ["pt-BR", "pt-PT"]);

        Assert.AreEqual("pt-BR", match);
    }

    [TestMethod("区域文化回退到中性语言代码")]
    public void TC25_RegionalCultureFallbackToNeutralLanguageCode()
    {
        var match = LocalizationFallbackProvider.FindBestMatch(
            "ko-KR",
            ["ko"]);

        Assert.AreEqual("ko", match);
    }
}
