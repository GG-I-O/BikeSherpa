import {Button, useTheme} from "react-native-paper";
import * as DocumentPicker from "expo-document-picker";
import React from "react";
import UploadableFile from "@/models/UploadableFile";
import ThemedDocumentIcon from "@/components/themed/ThemedDocumentIcon";

type Props = {
    deliveryCode: string;
    onDocument: (file: UploadableFile) => void;
}

export default function Document(props: Props) {
    const theme = useTheme();

    const selectFile = () => {
        DocumentPicker.getDocumentAsync({
            type: "*/*",
            copyToCacheDirectory: false
        }).then(result => {
            if (!result.canceled) {
                const asset = result.assets[0];
                const extension = asset.name?.includes(".")
                    ? asset.name.substring(asset.name.lastIndexOf("."))
                    : "";
                props.onDocument({
                    uri: result.assets[0].uri,
                    mimeType: result.assets[0].mimeType ?? "application/octet-stream",
                    name: `document_${props.deliveryCode}_${Date.now()}${extension}`,
                    domainType: 'document'
                });
            }
        })
    };
    return (
        <Button
            mode="outlined"
            onPress={() => selectFile()}
        >
            <ThemedDocumentIcon />
        </Button>
    );
}