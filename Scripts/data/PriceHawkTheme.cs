using UnityEngine;

namespace PriceHawk.Data
{
    /// <summary>
    /// Central theme palette — mirrors the CSS variables in pricehawk-v3.html.
    /// Use these anywhere you need programmatic color access.
    /// For UI components, assign matching colors in the Inspector instead.
    /// </summary>
    public static class PriceHawkTheme
    {
        // Backgrounds
        public static readonly Color Bg       = HEX("#0d0f14");
        public static readonly Color Surface  = HEX("#161b24");
        public static readonly Color Surface2 = HEX("#1e2535");
        public static readonly Color CardBg   = HEX("#131921");

        // Borders & text
        public static readonly Color Border   = HEX("#2a3347");
        public static readonly Color Text     = HEX("#e8edf5");
        public static readonly Color Muted    = HEX("#7a8ba8");

        // Accents
        public static readonly Color Accent   = HEX("#00e5a0");  // green
        public static readonly Color Accent2  = HEX("#0099ff");  // blue
        public static readonly Color Warn     = HEX("#ff6b35");  // orange-red
        public static readonly Color Good     = HEX("#00e5a0");

        // Store brand colors
        public static readonly Color Amazon   = HEX("#ff9900");
        public static readonly Color Flipkart = HEX("#2874f0");

        // ── Helper ────────────────────────────────────────────────────────────────
        public static Color HEX(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out Color c);
            return c;
        }
    }
}
