import {Icon} from "react-native-paper/src";
import React from "react";
import {useTheme} from "react-native-paper";

export default function ThemedPhotoIcon() {
    const theme = useTheme();
    return <Icon source="camera" size={24} color={theme.colors.onBackground}/>
}