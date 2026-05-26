import {Button, useTheme} from "react-native-paper";
import * as DocumentPicker from "expo-document-picker";
import {Icon} from "react-native-paper/src";
import React from "react";
import UploadableFile from "@/models/UploadableFile";

type Props = {
    deliveryCode: string;
    onDocument: (file: UploadableFile) => void;
}

export default function Document(props: Props) {
    const theme = useTheme();

    const selectFile = () => {
        DocumentPicker.getDocumentAsync({
            type: "*/*",
            copyToCacheDirectory: true
        }).then(result => {
            if (!result.canceled) {                
                props.onDocument({
                    uri: result.assets[0].uri,
                    type: result.assets[0].mimeType ?? "application/octet-stream",
                    name: `document_${props.deliveryCode}_${Date.now()}`,
                });
            }
        })
    };
    return (
        <Button
            mode="outlined"
            onPress={() => selectFile()}
        >
            <Icon source="file" size={24} color={theme.colors.onBackground}/>
        </Button>
    );
}