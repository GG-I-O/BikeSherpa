import {Icon} from "react-native-paper/src";
import {Button, useTheme} from "react-native-paper";
import React from "react";
import * as ImagePicker from 'expo-image-picker';
import UploadableFile from "@/models/UploadableFile";

type Props = {
    deliveryCode: string;
    onPhoto: (file: UploadableFile) => void;
}

export default function Photo(props: Props) {
    const theme = useTheme();
    
    const photoQuality = parseFloat(process.env.EXPO_PUBLIC_PHOTO_QUALITY ?? '0.8');

    const takePhoto = () => {
        ImagePicker.launchCameraAsync({
            mediaTypes: 'images',
            quality: photoQuality,
            base64: true,
        }).then(result => {
            if (!result.canceled) {
                props.onPhoto({
                    uri: result.assets[0].uri,
                    type: 'image/png',
                    name: `photo_${props.deliveryCode}_${Date.now()}.png`,
                });
            }
        })
    };
    return (
        <Button
            mode="outlined"
            onPress={() => takePhoto()}
        >
            <Icon source="camera" size={24} color={theme.colors.onBackground}/>
        </Button>
    );
}