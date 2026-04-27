using MudBlazor;

namespace HabitForge_MudBlazor.Themes
{
    /// <summary>
    /// RetroForge Theme — Old-school UI vibes.
    /// Inspired by: CRT monitors, MS-DOS, Windows 3.1/95, Commodore 64,
    /// pixelated UI kits, and the golden age of floppy disks.
    ///
    /// Font stack: "VT323" (pixel terminal), "Press Start 2P" (chunky retro),
    ///             "Courier Prime" (typewriter body), "Special Elite" (worn label feel)
    ///
    /// Suggested Google Fonts import (put in index.html or _Host.cshtml):
    /// https://fonts.googleapis.com/css2?family=VT323&family=Press+Start+2P&family=Courier+Prime:wght@400;700&family=Special+Elite&display=swap
    /// </summary>
    public static class RetroForgeTheme
    {
        // ─── LIGHT THEME (Warm Paper / Amber CRT) ────────────────────────────────
        public static MudTheme LightTheme => new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                // Amber phosphor CRT on warm parchment
                Primary = "#C47A00",        // deep amber — like a lit monitor glow
                PrimaryContrastText = "#FFF8DC",        // cornsilk text on amber
                Secondary = "#5A7A3A",        // muted olive green (DOS prompt green)
                SecondaryContrastText = "#F0F0D0",
                Tertiary = "#8B1A1A",        // dark brick red — old error color
                TertiaryContrastText = "#FFF0D0",

                Background = "#F5F0DC",        // aged paper / parchment
                BackgroundGray = "#EDE8CE",        // slightly darker parchment
                Surface = "#FDFBF0",        // off-white card surface
                AppbarBackground = "#1C1C1C",        // dark CRT bezel
                AppbarText = "#FFD700",        // gold phosphor title text
                DrawerBackground = "#1C1C1C",        // same dark bezel
                DrawerText = "#C8C090",        // faded amber text
                DrawerIcon = "#FFB800",

                TextPrimary = "#1A1200",        // near-black ink
                TextSecondary = "#4A3F10",        // dark sepia
                TextDisabled = "#A89860",

                Divider = "#C8B870",        // aged gold rule line
                DividerLight = "#E0D4A0",
                ActionDefault = "#4A3F10",
                ActionDisabled = "#C8B870",
                ActionDisabledBackground = "#EDE8CE",

                Info = "#2E6B8A",        // muted steel blue (like old info boxes)
                InfoContrastText = "#F0F8FF",
                Success = "#3A6B2A",        // DOS-green success
                SuccessContrastText = "#F0FFE0",
                Warning = "#C47A00",        // amber warning (same as primary)
                WarningContrastText = "#1A1200",
                Error = "#8B1A1A",        // brick red error
                ErrorContrastText = "#FFF0D0",

                LinesDefault = "#C8B870",
                LinesInputs = "#A89840",
                TableLines = "#D4C880",
                TableStriped = "#F0EBD0",
                TableHover = "#E8E0B8",

                OverlayDark = "rgba(20,15,0,0.65)",
                OverlayLight = "rgba(253,251,240,0.92)",
                Dark = "#1C1C0A",
                DarkContrastText = "#F5F0DC"
            },

            LayoutProperties = RetroLayoutProperties(),
            Typography = RetroTypography(),
        };

        // ─── DARK THEME (Green Phosphor CRT Terminal) ────────────────────────────
        public static MudTheme DarkTheme => new MudTheme
        {
            PaletteDark = new PaletteDark
            {
                // Classic green phosphor terminal on near-black
                Primary = "#00C853",        // bright phosphor green
                PrimaryContrastText = "#000E05",
                Secondary = "#00B0A0",        // teal-cyan accent (ANSI cyan)
                SecondaryContrastText = "#001210",
                Tertiary = "#FFB300",        // amber highlight
                TertiaryContrastText = "#1A0E00",

                Background = "#0A0A0A",        // near-black CRT background
                BackgroundGray = "#0F0F0F",
                Surface = "#121212",        // slightly lighter card
                AppbarBackground = "#050505",        // absolute dark bezel
                AppbarText = "#00FF41",        // matrix green title
                DrawerBackground = "#050505",
                DrawerText = "#00C853",
                DrawerIcon = "#00FF41",

                TextPrimary = "#D4F5D4",        // pale green body text
                TextSecondary = "#80C880",        // dimmer green secondary
                TextDisabled = "#1E5C1E",

                Divider = "#1A4A1A",        // dark green divider
                DividerLight = "#143214",
                ActionDefault = "#80C880",
                ActionDisabled = "#1E5C1E",
                ActionDisabledBackground = "#0A1A0A",

                Info = "#00B0A0",        // ANSI cyan info
                InfoContrastText = "#001412",
                Success = "#00E676",        // bright green success
                SuccessContrastText = "#001A0A",
                Warning = "#FFB300",        // amber warning
                WarningContrastText = "#1A0E00",
                Error = "#FF1744",        // red BSOD-style error
                ErrorContrastText = "#FFF0F0",

                LinesDefault = "#1A4A1A",
                LinesInputs = "#205A20",
                TableLines = "#143214",
                TableStriped = "#0D140D",
                TableHover = "#0F1F0F",

                OverlayDark = "rgba(0,0,0,0.80)",
                OverlayLight = "rgba(0,255,65,0.05)",
                Dark = "#000000",
                DarkContrastText = "#D4F5D4"
            },

            LayoutProperties = RetroLayoutProperties(),
            Typography = RetroTypography(),
        };

        // ─── LAYOUT ──────────────────────────────────────────────────────────────
        private static LayoutProperties RetroLayoutProperties() => new LayoutProperties
        {
            // Sharp square corners — retro UIs had ZERO border radius
            DefaultBorderRadius = "0px",
            DrawerWidthLeft = "240px",
            DrawerWidthRight = "240px",
            AppbarHeight = "56px",
        };

        // ─── TYPOGRAPHY ──────────────────────────────────────────────────────────
        private static Typography RetroTypography() => new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Courier Prime", "Courier New", "monospace" },
                FontSize = ".9rem",
                LetterSpacing = ".04em"
            },
            H1 = new DefaultTypography
            {
                FontFamily = new[] { "Press Start 2P", "VT323", "monospace" },
                FontSize = "2rem",
                LetterSpacing = ".06em"
            },
            H2 = new DefaultTypography
            {
                FontFamily = new[] { "Press Start 2P", "VT323", "monospace" },
                FontSize = "1.6rem",
                LetterSpacing = ".05em"
            },
            H3 = new DefaultTypography
            {
                FontFamily = new[] { "VT323", "Courier Prime", "monospace" },
                FontSize = "1.8rem",
                LetterSpacing = ".04em"
            },
            H4 = new DefaultTypography
            {
                FontFamily = new[] { "VT323", "Courier Prime", "monospace" },
                FontSize = "1.5rem",
                LetterSpacing = ".03em"
            },
            H5 = new DefaultTypography
            {
                FontFamily = new[] { "Special Elite", "Courier Prime", "monospace" },
                FontSize = "1.2rem",
                LetterSpacing = ".03em"
            },
            H6 = new DefaultTypography
            {
                FontFamily = new[] { "Special Elite", "Courier Prime", "monospace" },
                FontSize = "1rem",
                LetterSpacing = ".02em"
            },
            Button = new DefaultTypography
            {
                FontFamily = new[] { "VT323", "Courier Prime", "monospace" },
                FontSize = "1rem",
                LetterSpacing = ".08em"
            },
            Caption = new DefaultTypography
            {
                FontFamily = new[] { "Courier Prime", "monospace" },
                FontSize = ".75rem",
                LetterSpacing = ".05em"
            },
            Overline = new DefaultTypography
            {
                FontFamily = new[] { "VT323", "monospace" },
                FontSize = ".7rem",
                LetterSpacing = ".12em"
            },
            Subtitle1 = new DefaultTypography
            {
                FontFamily = new[] { "Special Elite", "Courier Prime", "monospace" },
                FontSize = ".95rem",
                LetterSpacing = ".02em"
            },
            Subtitle2 = new DefaultTypography
            {
                FontFamily = new[] { "Special Elite", "Courier Prime", "monospace" },
                FontSize = ".85rem",
                LetterSpacing = ".02em"
            },
            Body1 = new DefaultTypography
            {
                FontFamily = new[] { "Courier Prime", "Courier New", "monospace" },
                FontSize = ".9rem",
                LetterSpacing = ".03em"
            },
            Body2 = new DefaultTypography
            {
                FontFamily = new[] { "Courier Prime", "Courier New", "monospace" },
                FontSize = ".8rem",
                LetterSpacing = ".03em"
            },
        };
    }
}