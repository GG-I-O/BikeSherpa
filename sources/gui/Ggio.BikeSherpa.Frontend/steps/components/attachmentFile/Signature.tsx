import React, {useRef, useState} from 'react';
import {Modal, View, StyleSheet, Image} from 'react-native';
import SignatureCanvas, {SignatureViewRef} from 'react-native-signature-canvas';
import {Icon} from "react-native-paper/src";
import {Button, TextInput, useTheme} from "react-native-paper";
import {File, Paths} from "expo-file-system";
import UploadableFile from "@/models/UploadableFile";
import formStyle from "@/style/formStyle";

type Props = {
    deliveryCode: string;
    onSignature: (file: UploadableFile) => void;
}

export default function Signature(props: Props) {
    const theme = useTheme();

    const [signature, setSignature] = useState<string | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [show, setShow] = useState<boolean>(false);
    const ref = useRef<SignatureViewRef | null>(null);
    
    const [signatureName, setSignatureName] = useState<string>("");

    const handleSignature = (signature: string) => {
        setIsLoading(true);
        setSignature(signature);

        const match = signature.match(/^data:(.+);base64,(.*)$/);
        if (!match) {
            throw new Error("Invalid base64 data URL.");
        }
        const mimeType = match[1];
        const base64 = match[2];

        const signatureFile = new File(Paths.cache, `signature_${props.deliveryCode}_${Date.now()}.png`);
        signatureFile.create({overwrite: true, intermediates: true});
        signatureFile.write(base64, {encoding: 'base64'});

        props.onSignature({
            uri: signatureFile.uri,
            type: mimeType,
            name: `signature_${signatureName}_${props.deliveryCode}_${Date.now()}`,
        });

        setIsLoading(false);
        setShow(false);
    };

    const handleEmpty = () => {
        setIsLoading(false);
    };

    const handleClear = () => {
        ref.current?.clearSignature();
        setSignature(null);
        setIsLoading(false);
    };

    const handleError = (error: Error) => {
        setIsLoading(false);
    };

    const handleConfirm = () => {
        ref.current?.readSignature();
    };

    const handleClose = () => {
        setShow(false);
        setIsLoading(false);
    };

    return (
        <>
            <Button
                mode="outlined"
                onPress={() => setShow(true)}
            >
                <Icon source="draw-pen" size={24} color={theme.colors.onBackground}/>
            </Button>
            <Modal
                animationType="slide"
                transparent={true}
                visible={show}
                onRequestClose={handleClose}
            >
                <View style={styles.modalContent}>
                    <View style={styles.preview}>
                        {signature && (
                            <Image
                                resizeMode="contain"
                                style={{width: '100%', height: 150}}
                                source={{uri: signature}}
                            />
                        )}
                    </View>
                    <SignatureCanvas
                        ref={ref}
                        onOK={handleSignature}
                        onEmpty={handleEmpty}
                        onError={handleError}
                        autoClear={false}
                        descriptionText="Sign here"
                        penColor="#000000"
                        backgroundColor="rgba(255,255,255,0)"
                        webviewProps={{
                            cacheEnabled: true,
                            androidLayerType: "hardware",
                        }}
                    />

                    <TextInput
                        value={signatureName}
                        onChangeText={(value) => setSignatureName(value)}
                        placeholder="Nom..."
                        placeholderTextColor={'#3636367e'}
                        mode='outlined'
                        style={[formStyle.input,
                            {
                                backgroundColor: theme.colors.background,
                                color: theme.colors.onBackground,
                            }
                        ]}
                        contentStyle={{color: theme.colors.onBackground}}
                    />

                    <View style={styles.actions}>
                        <Button
                            mode="outlined"
                            onPress={handleClose}
                            disabled={isLoading}
                        >
                            Cancel
                        </Button>

                        <Button
                            mode="outlined"
                            onPress={handleClear}
                            disabled={isLoading}
                        >
                            Clear
                        </Button>

                        <Button
                            mode="outlined"
                            onPress={handleConfirm}
                            loading={isLoading}
                            disabled={isLoading}
                        >
                            Confirm
                        </Button>
                    </View>
                </View>
            </Modal>
        </>
    );
};

const styles = StyleSheet.create({
    modalContent: {
        width: '80%',
        height: '80%',
        top: 0,
        borderRadius: 12,
        padding: 12,
        gap: 12,
        marginInline: 'auto'
    },
    container: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
    },
    canvasContainer: {
        flex: 1,
        borderWidth: 1,
        borderRadius: 8,
        overflow: 'hidden',
    },
    preview: {
        width: 335,
        height: 114,
        justifyContent: 'center',
        alignItems: 'center',
        marginTop: 15,
    },
    actions: {
        flexDirection: 'row',
        justifyContent: 'center',
        gap: 4
    }
});