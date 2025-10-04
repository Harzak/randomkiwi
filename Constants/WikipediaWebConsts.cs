using System.Text;

namespace randomkiwi.Constants;

public static class WikipediaWebConsts
{
    internal static readonly CompositeFormat COOKIE_THEME_CLIENT_KEY_FORMAT = CompositeFormat.Parse("{0}wikimwclientpreferences");
    internal static readonly CompositeFormat COOKIE_THEME_CLIENT_VALUE_FORMAT = CompositeFormat.Parse("skin-theme-clientpref-{0}");
    internal const string COOKIE_THEME_CLIENT_VALUE_NIGHT = "night";
    internal const string COOKIE_THEME_CLIENT_VALUE_LIGHT = "light";
}
