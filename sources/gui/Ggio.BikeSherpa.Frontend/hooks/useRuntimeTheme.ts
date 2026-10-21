import {useEffect, useMemo, useState} from "react";
import {Platform, useColorScheme} from "react-native";
import {MD3DarkTheme, MD3LightTheme, MD3Theme} from "react-native-paper";
import bundledColors from "@/assets/colors/colors.json";

type ColorConfig = {
    light?: Partial<MD3Theme["colors"]>;
    dark?: Partial<MD3Theme["colors"]>;
};

const isValid = (json: unknown): json is ColorConfig =>
    typeof json === "object" && json !== null;

function useColorConfig(): ColorConfig | null {
    // Native has no nginx to ask: use the bundled file directly.
    const [config, setConfig] = useState<ColorConfig | null>(
        Platform.OS === "web" ? null : (bundledColors as ColorConfig)
    );

    useEffect(() => {
        if (Platform.OS !== "web") return;
        let cancelled = false;

        fetch("/colors.json", {cache: "no-cache"})
            .then(r => (r.ok ? r.json() : Promise.reject()))
            .then(json => {
                if (!cancelled) setConfig(isValid(json) ? json : (bundledColors as ColorConfig));
            })
            // 404, invalid JSON, dev server returning HTML... -> bundled defaults
            .catch(() => {
                if (!cancelled) setConfig(bundledColors as ColorConfig);
            });

        return () => {
            cancelled = true;
        };
    }, []);

    return config;
}

export function useRuntimeTheme(): MD3Theme | null {
    const scheme = useColorScheme();
    const config = useColorConfig();

    return useMemo(() => {
        if (!config) return null;
        const base = scheme === "dark" ? MD3DarkTheme : MD3LightTheme;
        const overrides = (scheme === "dark" ? config.dark : config.light) ?? {};
        return {...base, colors: {...base.colors, ...overrides}};
    }, [scheme, config]);
}