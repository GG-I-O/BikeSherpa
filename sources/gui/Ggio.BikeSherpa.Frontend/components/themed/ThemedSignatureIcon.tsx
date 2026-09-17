import {Icon} from "react-native-paper/src";
import React from "react";
import {useTheme} from "react-native-paper";

export default function ThemedSignatureIcon() {
    const theme = useTheme();
    return <Icon source="draw-pen" size={24} color={theme.colors.onBackground}/>
}