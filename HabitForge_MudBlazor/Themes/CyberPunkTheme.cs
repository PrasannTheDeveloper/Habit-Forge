using MudBlazor;

namespace HabitForge_MudBlazor.Themes
{
    public static class CyberPunkTheme
    {
        public static MudTheme LightTheme => new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#E91E8C",
                PrimaryContrastText = "#FFFFFF",
                PrimaryDarken = "#C0156F",
                PrimaryLighten = "#FF4DB8",

                Secondary = "#00D4FF",  // Muted cyan instead of bright green
                SecondaryContrastText = "#0A0A1A",
                SecondaryDarken = "#00A8CC",
                SecondaryLighten = "#4DE3FF",

                Tertiary = "#B400FF",  // Purple for accent
                TertiaryContrastText = "#FFFFFF",

                Background = "#FFFFFF",
                BackgroundGray = "#E0DCFF",

                Surface = "#FFFFFF",

                AppbarBackground = "#FFFFFF",
                AppbarText = "#1A0033",

                DrawerBackground = "#FFFFFF",
                DrawerText = "#1A0033",
                DrawerIcon = "#E91E8C",

                TextPrimary = "#1A0033",
                TextSecondary = "#5C4080",
                TextDisabled = "#B0A0C8",

                Divider = "#C8B8F0",
                DividerLight = "#E0DCFF",

                ActionDefault = "#5C4080",
                ActionDisabled = "#B0A0C8",
                ActionDisabledBackground = "#E0DCFF",

                Info = "#00B4D8",
                InfoContrastText = "#FFFFFF",
                Success = "#00D68F",  // Cyber teal instead of bright green
                SuccessContrastText = "#0A0A1A",
                Warning = "#FF9F1C",
                WarningContrastText = "#0A0A1A",
                Error = "#FF2D55",
                ErrorContrastText = "#FFFFFF",

                LinesDefault = "#C8B8F0",
                LinesInputs = "#9C7BC8",
                TableLines = "#D4C8F4",
                TableStriped = "#F5F0FF",
                TableHover = "#EDE6FF",

                OverlayDark = "rgba(26,0,51,0.6)",
                OverlayLight = "rgba(240,238,255,0.8)",

                Dark = "#1A0033",
                DarkContrastText = "#E0DCFF",
                DarkDarken = "#0D0019",
                DarkLighten = "#2D0055",

                GrayDefault = "#9C7BC8",
                GrayLight = "#D4C8F4",
                GrayLighter = "#EDE6FF",
                GrayDark = "#5C4080",
                GrayDarker = "#1A0033",
            },

            LayoutProperties = CyberPunkLayoutProperties(),
            Typography = CyberPunkTypography(),
        };

        public static MudTheme DarkTheme => new MudTheme
        {
            PaletteDark = new PaletteDark
            {
                Primary = "#FF2D9B",
                PrimaryContrastText = "#FFFFFF",
                PrimaryDarken = "#CC2278",
                PrimaryLighten = "#FF70C2",

                Secondary = "#00D4FF",  // Muted cyan
                SecondaryContrastText = "#030314",
                SecondaryDarken = "#00A8CC",
                SecondaryLighten = "#4DE3FF",

                Tertiary = "#B400FF",  // Purple accent
                TertiaryContrastText = "#FFFFFF",

                Background = "#030314",
                BackgroundGray = "#0D0D2B",

                Surface = "#0D0D2B",

                AppbarBackground = "#07071F",
                AppbarText = "#00D4FF",

                DrawerBackground = "#07071F",
                DrawerText = "#C8C8FF",
                DrawerIcon = "#FF2D9B",

                TextPrimary = "#E8E8FF",
                TextSecondary = "#A0A0D0",
                TextDisabled = "#4A4A7A",

                Divider = "#2A2A5A",
                DividerLight = "#1A1A3A",

                ActionDefault = "#A0A0D0",
                ActionDisabled = "#4A4A7A",
                ActionDisabledBackground = "#1A1A3A",

                Info = "#00B4FF",
                InfoContrastText = "#030314",
                Success = "#00D68F",  // Cyber teal instead of bright green
                SuccessContrastText = "#030314",
                Warning = "#FF9F1C",
                WarningContrastText = "#030314",
                Error = "#FF2D55",
                ErrorContrastText = "#FFFFFF",

                LinesDefault = "#2A2A5A",
                LinesInputs = "#4A3A8A",
                TableLines = "#1E1E4A",
                TableStriped = "#0F0F30",
                TableHover = "#1A1A44",

                OverlayDark = "rgba(3,3,20,0.85)",
                OverlayLight = "rgba(13,13,43,0.6)",

                Dark = "#07071F",
                DarkContrastText = "#C8C8FF",
                DarkDarken = "#030314",
                DarkLighten = "#1A1A44",

                GrayDefault = "#4A4A7A",
                GrayLight = "#2A2A5A",
                GrayLighter = "#1A1A3A",
                GrayDark = "#A0A0D0",
                GrayDarker = "#E8E8FF",

                White = "#E8E8FF",
                Black = "#030314",
            },

            LayoutProperties = CyberPunkLayoutProperties(),
            Typography = CyberPunkTypography(),
        };

        private static LayoutProperties CyberPunkLayoutProperties() => new LayoutProperties
        {
            DefaultBorderRadius = "4px",
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "260px",
            AppbarHeight = "64px",
        };

        private static Typography CyberPunkTypography() => new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "Orbitron", "sans-serif" },
                FontSize = ".9rem",

                LetterSpacing = ".02em"
            },
            H1 = new DefaultTypography
            {
                FontFamily = new[] { "Orbitron", "sans-serif" },
                FontSize = "2.5rem",

                LetterSpacing = ".08em"
            },
            H2 = new DefaultTypography
            {
                FontFamily = new[] { "Orbitron", "sans-serif" },
                FontSize = "2rem",

                LetterSpacing = ".06em"
            },
            H3 = new DefaultTypography
            {
                FontFamily = new[] { "Orbitron", "sans-serif" },
                FontSize = "1.75rem",

                LetterSpacing = ".05em"
            },
            H4 = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = "1.5rem",

                LetterSpacing = ".04em"
            },
            H5 = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = "1.25rem",

                LetterSpacing = ".03em"
            },
            H6 = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = "1rem",

                LetterSpacing = ".03em"
            },
            Button = new DefaultTypography
            {
                FontFamily = new[] { "Orbitron", "sans-serif" },
                FontSize = ".8rem",

                LetterSpacing = ".1em"
            },
            Caption = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = ".75rem",

                LetterSpacing = ".04em"
            },
            Overline = new DefaultTypography
            {
                FontFamily = new[] { "Orbitron", "sans-serif" },
                FontSize = ".625rem",

                LetterSpacing = ".15em"
            },
            Subtitle1 = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = "1rem",

                LetterSpacing = ".02em"
            },
            Subtitle2 = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = ".875rem",

                LetterSpacing = ".02em"
            },
            Body1 = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = ".9rem",

                LetterSpacing = ".02em"
            },
            Body2 = new DefaultTypography
            {
                FontFamily = new[] { "Rajdhani", "sans-serif" },
                FontSize = ".8rem",
                LetterSpacing = ".02em"
            },
        };
    }
}