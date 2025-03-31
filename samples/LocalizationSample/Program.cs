﻿using System.ComponentModel;
using dotnetCampus.Localizations;

namespace LocalizationSample;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var tags = Lang.SupportedLanguageTags;
        Console.WriteLine(string.Join(", ", tags));

        var a = Lang.Current.A.A2.ToString(1);
        Console.WriteLine(a);

        if (Lang.Current is INotifyPropertyChanged changed)
        {
            changed.PropertyChanged += ChangedOnPropertyChanged;
        }

        await Lang.SetCurrent("en");
    }

    private static void ChangedOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        Console.WriteLine($"语言项变更：{e.PropertyName}");
    }
}

[LocalizedConfiguration(Default = "zh-Hans", SupportsNotification = true)]
internal partial class Lang;
