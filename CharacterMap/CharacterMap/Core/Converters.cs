﻿using CharacterMap.Helpers;
using CharacterMap.Models;
using CharacterMap.Services;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CharacterMap.Core
{
    public static class Converters
    {
        public const string Auto = "Auto";
        public const string Star = "*";

        private static AppSettings _settings;
        private static UserCollectionsService _userCollections;



        public static bool False(bool b) => !b;
        public static bool FalseFalse(bool b, bool c) => !b && !c;
        public static bool True(bool b) => b;

        public static Visibility InvertVis(Visibility b) => b == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        public static Visibility FalseToVis(bool b) => !b ? Visibility.Visible : Visibility.Collapsed;
        public static Visibility TrueToVis(bool b) => b ? Visibility.Visible : Visibility.Collapsed;
        public static Visibility IsNotNullToVis(object obj) => obj != null ? Visibility.Visible : Visibility.Collapsed;


        public static bool IsNull(object obj) => obj == null;
        public static bool IsNotNull(object obj) => obj != null;

        public static string ToHex(int i) => Unicode.GetHexValue(i);

        public static string GetWeightName(Windows.UI.Text.FontWeight weight)
        {
            return $"{Utils.GetWeightName(weight)} - {weight.Weight}";
        }

        public static Brush GetForegroundBrush(object obj)
        {
            if (Utils.IsAccentColorDark())
                return ResourceHelper.Get<Brush>("WhiteBrush");
            else
                return ResourceHelper.Get<Brush>("BlackBrush");
        }

        public static ElementTheme ChooseThemeForAccent(object obj)
        {
            return Utils.IsAccentColorDark() ? ElementTheme.Dark : ElementTheme.Light;
        }
        public static double GetFontSize(int d) 
            => d / 2d;

        public static GridLength GridLengthAorB(bool input, string a, string b)
            => input ? ReadFromString(a) : ReadFromString(b);

        private static GridLength ReadFromString(string s) => s switch
        {
            Auto => new GridLength(1, GridUnitType.Auto),
            Star => new GridLength(1, GridUnitType.Star),
            _ => new GridLength(double.Parse(s), GridUnitType.Pixel),
        };

        public static string GetAnnotation(GlyphAnnotation a, int index) => a switch
        {
            GlyphAnnotation.None => string.Empty,
            GlyphAnnotation.UnicodeHex => $"U+{index:x4}".ToUpper(),
            GlyphAnnotation.UnicodeIndex => index.ToString(),
            _ => string.Empty
        };

        public static string GetLocalizedEnumName(Enum a) 
            => Localization.Get($"{a.GetType().Name}_{a}");

        public static FontFamily GetPreviewFontSource(FontVariant variant)
        {
            if (_settings == null)
            {
                _settings = ResourceHelper.Get<AppSettings>(nameof(AppSettings));
                _userCollections = SimpleIoc.Default.GetInstance<UserCollectionsService>();
            }

            if (_settings.UseFontForPreview
                && !variant.FontFace.IsSymbolFont
                && !_userCollections.SymbolCollection.Fonts.Contains(variant.FamilyName))
                return new FontFamily(variant.Source);

            return FontFamily.XamlAutoFontFamily;
        }

        public static string GetFileSize(int fileSize)
        {
            double size = (double)fileSize / 1024;
            if (size < 600)
                return $"{size:0.00} KB";

            size /= 1024;
            return $"{size:0.00} MB";
        }

        public static SupportedLanguage GetSelectedLanguage(string selected, IList<Models.SupportedLanguage> languages) 
            => languages.FirstOrDefault(i => i.LanguageID == selected);

        public static string GetLanguageDisplayFromID(string id)
            => new System.Globalization.CultureInfo(id).DisplayName;

        /// <summary>
        /// This converter only use to show "need restart" text block.
        /// </summary>
        /// <param name="selectedLanguage"></param>
        /// <returns>Return Visible if language is changed and not match current app language, otherwise it return Collapsed.</returns>
        public static Visibility CompareLanguageToSetting(string selectedLanguage) =>
            System.Globalization.CultureInfo.CurrentUICulture.Name == selectedLanguage 
            ? Visibility.Collapsed : Visibility.Visible;
    }
}
