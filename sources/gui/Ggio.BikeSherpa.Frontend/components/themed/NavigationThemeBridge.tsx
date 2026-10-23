import {useTheme} from "react-native-paper";
import {useMemo} from "react";
import {DefaultTheme, Theme, ThemeProvider} from "@react-navigation/native";

export default function NavigationThemeBridge({children}: {children: React.ReactNode}) {
    const paper = useTheme();

    const navTheme = useMemo<Theme>(() => ({
        dark: paper.dark,
        colors: {
            primary: paper.colors.onBackground,   // back arrow + header buttons
            background: paper.colors.background,  // screen background
            card: paper.colors.background,        // header background
            text: paper.colors.onBackground,      // header title
            border: paper.colors.outlineVariant,  // header bottom border
            notification: paper.colors.error,
        },
        fonts: DefaultTheme.fonts,
    }), [paper]);

    return <ThemeProvider value={navTheme}>{children}</ThemeProvider>;
}