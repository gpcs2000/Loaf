using Microsoft.Win32;

namespace Loaf.Utils
{
    public static class ThemeHelper
    {
        public static bool? IsDarkTheme
        {
            get
            {
                const string registryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
                const string registryValueName = "AppsUseLightTheme";
                object registryValueObject = Registry.CurrentUser.OpenSubKey(registryKeyPath)?.GetValue(registryValueName);
                if (registryValueObject is null) return null;
                return (int) registryValueObject <= 0;
            }
        }
    }
}