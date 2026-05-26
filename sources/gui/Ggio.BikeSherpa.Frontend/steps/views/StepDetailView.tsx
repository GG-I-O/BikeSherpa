import AppStyle from "@/constants/AppStyle";
import {useLocalSearchParams} from "expo-router";
import {Linking, View} from "react-native";
import {Button, Divider, Text, TextInput, useTheme} from "react-native-paper";
import useStepDetailViewModel from "@/steps/viewModel/useStepDetailViewModel";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import {Icon} from "react-native-paper/src";
import {StepType} from "@/steps/models/StepType";
import unassignedPhoneNumber from "@/steps/constants/unassignedPhoneNumber";
import React from "react";
import Signature from "@/steps/components/attachmentFile/Signature";
import Photo from "@/steps/components/attachmentFile/Photo";
import Document from "@/steps/components/attachmentFile/Document";

export default function StepDetailView() {
    const theme = useTheme();

    const {stepId} = useLocalSearchParams<{ stepId: string }>();

    const viewModel = useStepDetailViewModel(stepId);

    if (!viewModel.step)
        return (
            <View style={{alignItems: 'center', justifyContent: 'center'}}>
                <Text>Étape inexistante</Text>
            </View>
        )

    return (
        <View style={{backgroundColor: theme.colors.background, padding: 8, height: '100%'}}>
            <View style={{flexDirection: 'column', justifyContent: 'flex-start', gap: 8}}>
                <View style={{gap: 8, flexDirection: 'row'}}>
                    <Signature
                        deliveryCode={viewModel.step.deliveryCode}
                        onSignature={(file) => viewModel.addAttachment(file)}
                    />
                    <Photo
                        deliveryCode={viewModel.step.deliveryCode}
                        onPhoto={(file) => viewModel.addAttachment(file)}
                    />
                    <Document
                        deliveryCode={viewModel.step.deliveryCode}
                        onDocument={(file) => viewModel.addAttachment(file)}
                    />
                </View>
                <View style={{gap: 8}}>
                    <Button
                        buttonColor={viewModel.step.completed ? theme.colors.errorContainer : theme.colors.background}
                        mode="outlined"
                        onPress={() => {
                            viewModel.step?.completed ? viewModel.cancelStep() : viewModel.completeStep()
                        }}
                    >
                        <Text>{viewModel.step.completed ? "Annuler" : "Valider"}</Text>
                    </Button>
                </View>
            </View>

            <View style={{
                flexDirection: 'row',
                justifyContent: 'space-between',
                marginBlock: 24,
                flexWrap: 'wrap',
                gap: 8
            }}>
                <Text style={AppStyle.textStyle.h2}>{viewModel.step.deliveryCode}</Text>
            </View>

            <View style={{
                flexDirection: 'column',
                justifyContent: 'space-evenly',
                flexWrap: 'wrap',
                width: '100%',
                gap: 32
            }}>
                <View style={{flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center'}}>
                    <View style={{flexDirection: 'row', alignItems: 'center', marginInline: 16, gap: 32}}>
                        <DeliveryTypeIcon type={viewModel.step.type}/>
                        <View>
                            <Text style={AppStyle.textStyle.h3}>{viewModel.step.address.name}</Text>
                            <Text>{viewModel.step.address.streetInfo}</Text>
                            {viewModel.step.address.complement && <Text>{viewModel.step.address.complement}</Text>}
                            <Text>{`${viewModel.step.address.postcode} ${viewModel.step.address.city}`}</Text>
                        </View>
                    </View>
                    <Button
                        mode="outlined"
                        onPress={() => Linking.openURL(`tel:${viewModel.step!.address.phone}`)}
                        disabled={!viewModel.step.address.phone}
                    >
                        <View style={{flexDirection: 'row', gap: 8, alignItems: 'center'}}>
                            <Icon source="phone" size={24} color={theme.colors.onBackground}/>
                            <Text>{viewModel.step!.address.phone != "" ? viewModel.step!.address.phone : unassignedPhoneNumber}</Text>
                        </View>
                    </Button>
                </View>
                <Divider/>
                <View style={{
                    flexDirection: "row",
                    marginInline: 32,
                    justifyContent: "space-between",
                    alignItems: "center"
                }}>
                    <Text>Horaire contractuel</Text>
                    {viewModel.step.type === StepType.PickUp ? (
                        <Text style={AppStyle.textStyle.h3}>A partir de : {viewModel.step.deliveryTime}</Text>
                    ) : (
                        <Text style={AppStyle.textStyle.h3}>Avant : {viewModel.step.deliveryLimitDate}</Text>
                    )}
                </View>
                <View style={{width: "100%"}}>
                    <Text style={{textAlign: 'center'}}>Infos de livraison</Text>
                    <Divider style={{width: '50%', margin: 'auto'}} />
                    <Text style={[AppStyle.textStyle.h3, {textAlign: 'center'}]}>{viewModel.step.comment}</Text>
                </View>

                <View style={{width: "100%"}}>
                    <Text style={{textAlign: 'center'}}>Colisage</Text>
                    <Divider style={{width: '50%', margin: 'auto'}} />
                    <Text style={[AppStyle.textStyle.h3, {textAlign: 'center'}]}>{viewModel.step.packing}</Text>
                </View>

                <View style={{width: "100%"}}>
                    <Text style={{textAlign: 'center'}}>Commentaire livreur</Text>
                    <TextInput
                        style={{width: '80%', margin: 'auto'}}
                        value={viewModel.courierComment}
                        onChangeText={viewModel.setCourierComment}
                        mode="outlined"
                    />
                </View>

                <View style={{width: "100%"}}>
                    <Text style={{textAlign: 'center'}}>Pièces jointes</Text>
                    <Divider style={{width: '50%', margin: 'auto'}} />
                    {viewModel.step.attachmentFilePaths.map((filePath, index) =>
                        <Text
                            key={`${viewModel.step!.id}-${index}`}
                            style={{textAlign: 'center', marginTop: 16}}
                            onPress={() => Linking.openURL(filePath)}
                        >
                            {filePath}
                        </Text>
                    )}
                </View>
            </View>
        </View>
    );
}