import {Icon} from "react-native-paper/src";
import React from "react";
import {useTheme} from "react-native-paper";

export default function ThemedDocumentIcon() {
    const theme = useTheme();
    return <Icon source="file" size={24} color={theme.colors.onBackground}/>
}