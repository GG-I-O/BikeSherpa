import AppStyle from "@/constants/AppStyle";
import {useLocalSearchParams} from "expo-router";
import {Linking, View} from "react-native";
import {Button, Divider, Text, useTheme} from "react-native-paper";
import useStepDetailViewModel from "@/steps/viewModel/useStepDetailViewModel";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import {Icon} from "react-native-paper/src";
import {StepType} from "@/steps/models/StepType";

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
                    <Button
                        mode="outlined"
                        onPress={() => {
                        }}
                    >
                        <Icon source="draw-pen" size={24} color={theme.colors.onBackground}/>
                    </Button>
                    <Button
                        mode="outlined"
                        onPress={() => {
                        }}
                    >
                        <Icon source="camera" size={24} color={theme.colors.onBackground}/>
                    </Button>
                    <Button
                        mode="outlined"
                        onPress={() => {
                        }}
                    >
                        <Icon source="file" size={24} color={theme.colors.onBackground}/>
                    </Button>
                </View>
                <View style={{gap: 8}}>
                    <Button
                        mode="outlined"
                        onPress={() => {
                        }}
                    >
                        <Text>Valider</Text>
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
                    <DeliveryTypeIcon type={viewModel.step.type}/>
                    <View>
                        <Text style={AppStyle.textStyle.h3}>{viewModel.step.address.name}</Text>
                        <Text>{viewModel.step.address.streetInfo}</Text>
                        <Text>{`${viewModel.step.address.postcode} ${viewModel.step.address.city}`}</Text>
                    </View>
                    <Button
                        mode="outlined"
                        onPress={() => Linking.openURL(`tel:${"06 45 45 45 45"}`)}
                    >
                        <View style={{flexDirection: 'row', gap: 8, alignItems: 'center'}}>
                        <Icon source="phone" size={24} color={theme.colors.onBackground}/>
                        <Text>06 45 45 45 45</Text>
                        </View>
                    </Button>
                </View>
                <View style={{flexDirection: "row", marginInline: 32, justifyContent: "space-between", alignItems: "center"}}>
                    <Text>Horaire contractuel</Text>
                    {viewModel.step.type === StepType.PickUp ? (
                        <Text style={AppStyle.textStyle.h3}>A partir de : {viewModel.step.deliveryTime}</Text>
                    ) : (
                        <Text style={AppStyle.textStyle.h3}>Avant : {viewModel.step.deliveryLimitDate}</Text>
                    )}
                </View>
                <View style={{alignItems: "center", width: "100%"}}>
                    <Text style={{textAlign: 'center'}}>Infos de livraison</Text>
                    <Divider/>
                    <Text style={AppStyle.textStyle.h2}>{viewModel.step.comment}</Text>
                </View>

                <View style={{alignItems: "center", width: "100%"}}>
                    <Text style={{textAlign: 'center'}}>Colisage</Text>
                    <Divider/>
                    <Text style={AppStyle.textStyle.h3}>{viewModel.step.packing}</Text>
                </View>
            </View>
        </View>
    );
}