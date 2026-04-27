using MudBlazor;

namespace HabitForge_MudBlazor.Themes
{
    public static class AnimeClassroomTheme
    {
        public static MudTheme LightTheme => new MudTheme
        {
            PaletteLight = new PaletteLight
            {
                Primary = "#FF9F7C",
                PrimaryContrastText = "#FFFFFF",
                Secondary = "#A8E6CF",
                SecondaryContrastText = "#2C3E2F",
                Tertiary = "#FFB3BA",
                TertiaryContrastText = "#4A2F3A",
                Background = "#FFF8F0",
                BackgroundGray = "#FFF2E6",
                Surface = "#FFFFFF",
                AppbarBackground = "#FFF0E0",
                AppbarText = "#5D4037",
                DrawerBackground = "#FFF8F0",
                DrawerText = "#5D4037",
                DrawerIcon = "#FF9F7C",
                TextPrimary = "#2D1E17",
                TextSecondary = "#5D4037",
                TextDisabled = "#BCA594",
                Divider = "#F0DCC8",
                DividerLight = "#FAEDE0",
                ActionDefault = "#5D4037",
                ActionDisabled = "#D4C0B0",
                ActionDisabledBackground = "#FAEDE0",
                Info = "#87CEEB",
                InfoContrastText = "#2C3E50",
                Success = "#66BB6A",
                SuccessContrastText = "#FFFFFF",
                Warning = "#FFD93D",
                WarningContrastText = "#4A3728",
                Error = "#E57373",
                ErrorContrastText = "#FFFFFF",
                LinesDefault = "#E8D5BC",
                LinesInputs = "#D4BFA0",
                TableLines = "#F0E0D0",
                TableStriped = "#FFF5ED",
                TableHover = "#FFF0E6",
                OverlayDark = "rgba(45,30,23,0.5)",
                OverlayLight = "rgba(255,245,235,0.9)",
                Dark = "#3E2A23",
                DarkContrastText = "#FFF8F0"
            },

            LayoutProperties = AnimeClassroomLayoutProperties(),
            Typography = AnimeClassroomTypography(),
        };

        public static MudTheme DarkTheme => new MudTheme
        {
            PaletteDark = new PaletteDark()
            {
                Primary = "#FFAD8C",
                PrimaryContrastText = "#1A150E",
                Secondary = "#B8E6D4",
                SecondaryContrastText = "#0D1A12",
                Tertiary = "#FFC4CC",
                TertiaryContrastText = "#1A1014",
                Background = "#121212",
                BackgroundGray = "#181818",
                Surface = "#1E1E1E",
                AppbarBackground = "#121212",
                AppbarText = "#FFAD8C",
                DrawerBackground = "#121212",
                DrawerText = "#E0E0E0",
                DrawerIcon = "#FFAD8C",
                TextPrimary = "#F5F5F5",
                TextSecondary = "#B0B0B0",
                TextDisabled = "#424242",
                Divider = "#2C2C2C",
                DividerLight = "#383838",
                ActionDefault = "#B0B0B0",
                ActionDisabled = "#424242",
                ActionDisabledBackground = "#242424",
                Info = "#7EC8E0",
                InfoContrastText = "#0D1418",
                Success = "#4CAF50",
                SuccessContrastText = "#FFFFFF",
                Warning = "#FFD970",
                WarningContrastText = "#1A150E",
                Error = "#EF5350",
                ErrorContrastText = "#FFFFFF",
                LinesDefault = "#333333",
                LinesInputs = "#444444",
                TableLines = "#2C2C2C",
                TableStriped = "#1A1A1A",
                TableHover = "#242424",
                OverlayDark = "rgba(0,0,0,0.7)",
                OverlayLight = "rgba(255,255,255,0.1)",
                Dark = "#0A0A0A",
                DarkContrastText = "#F5F5F5"
            },

            LayoutProperties = AnimeClassroomLayoutProperties(),
            Typography = AnimeClassroomTypography(),
        };

        private static LayoutProperties AnimeClassroomLayoutProperties() => new LayoutProperties
        {
            DefaultBorderRadius = "12px",      // Softer, rounded corners like school supplies
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "260px",
            AppbarHeight = "64px",
        };

        private static Typography AnimeClassroomTypography() => new Typography
        {
            Default = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "Nunito", "M PLUS Rounded 1c", "sans-serif" },
                FontSize = ".95rem",

                LetterSpacing = ".01em"
            },
            H1 = new DefaultTypography
            {
                FontFamily = new[] { "Fredoka One", "Quicksand", "sans-serif" },
                FontSize = "2.5rem",
   
                LetterSpacing = ".02em"
            },
            H2 = new DefaultTypography
            {
                FontFamily = new[] { "Fredoka One", "Quicksand", "sans-serif" },
                FontSize = "2rem",
       
                LetterSpacing = ".02em"
            },
            H3 = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "sans-serif" },
                FontSize = "1.75rem",
         
                LetterSpacing = ".01em"
            },
            H4 = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "sans-serif" },
                FontSize = "1.5rem",
         
                LetterSpacing = ".01em"
            },
            H5 = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "sans-serif" },
                FontSize = "1.25rem",

                LetterSpacing = ".005em"
            },
            H6 = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "sans-serif" },
                FontSize = "1rem",
              
                LetterSpacing = ".005em"
            },
            Button = new DefaultTypography
            {
                FontFamily = new[] { "Nunito", "sans-serif" },
                FontSize = ".875rem",

                LetterSpacing = ".03em"
            },
            Caption = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "sans-serif" },
                FontSize = ".75rem",
           
                LetterSpacing = ".02em"
            },
            Overline = new DefaultTypography
            {
                FontFamily = new[] { "Nunito", "sans-serif" },
                FontSize = ".625rem",

                LetterSpacing = ".05em"
            },
            Subtitle1 = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "sans-serif" },
                FontSize = "1rem",

                LetterSpacing = ".01em"
            },
            Subtitle2 = new DefaultTypography
            {
                FontFamily = new[] { "Quicksand", "sans-serif" },
                FontSize = ".875rem",

                LetterSpacing = ".01em"
            },
            Body1 = new DefaultTypography
            {
                FontFamily = new[] { "Nunito", "sans-serif" },
                FontSize = ".95rem",
  
                LetterSpacing = ".01em"
            },
            Body2 = new DefaultTypography
            {
                FontFamily = new[] { "Nunito", "sans-serif" },
                FontSize = ".85rem",

                LetterSpacing = ".01em"
            },
        };
    }
}