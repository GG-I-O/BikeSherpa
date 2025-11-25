import { Dimensions, Pressable, View } from "react-native";
import { Button, Card, Divider, Modal, Portal, Text, useTheme } from "react-native-paper";
import { Step } from "../models/Step";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import AddressToolbox from "@/services/AddressToolbox";
import { useState } from "react";

type Props = {
    step: Step,
    onPress?: (step: Step) => void,
    isSelected?: boolean
}

export default function StepCard({ step, onPress, isSelected = false }: Props) {
    const theme = useTheme();

    const [isModalVisible, setIsModalVisible] = useState<boolean>(false);

    const screenWidth = Dimensions.get('window').width;

    return (
        <>
            <Card
                style={{
                    backgroundColor: isSelected ? theme.colors.primary : theme.colors.background,
                    width: screenWidth >= 280 ? 250 : 'auto'
                }}
                onPress={() => {
                    if (onPress) onPress(step);
                }}
            >
                <Card.Content>
                    <View style={{ flexDirection: 'row', justifyContent: 'space-evenly', alignItems: 'center' }}>
                        <DeliveryTypeIcon type={step.type} />
                        <Text>{step.id}</Text>
                        <Text>{step.estimatedDate ? step.getEstimatedTime() : step.getContractTime()}</Text>
                    </View>
                    <Divider />
                    <Pressable
                        style={{ justifyContent: 'space-evenly', marginInline: 16, marginTop: 8, maxWidth: '80%' }}
                        onPress={() => {
                            setIsModalVisible(true);
                        }}
                    >
                        <Text>{step.address.name}</Text>
                        <Text>{step.address.streetInfo}</Text>
                        <Text>{`${step.address.postcode} ${step.address.city}`}</Text>
                    </Pressable>
                </Card.Content>
            </Card>

            <Portal>
                <Modal
                    visible={isModalVisible}
                    onDismiss={() => setIsModalVisible(false)}
                    contentContainerStyle={{ backgroundColor: theme.colors.background, padding: 32, gap: 16, justifyContent: 'center', alignItems: 'center' }}
                >
                    <Text style={{}}>
                        Ouvrir la carte pour cette adresse ?
                    </Text>

                    <View style={{ flexDirection: 'row', gap: 8 }}>
                        <Button
                            mode="outlined"
                            onPress={() => {
                                AddressToolbox.openAdressInMaps(`${step.address.streetInfo}, ${step.address.postcode} ${step.address.city}`);
                                setIsModalVisible(false);
                            }}
                            style={{}}
                        >
                            Ouvrir carte
                        </Button>
                        <Button
                            mode="outlined"
                            onPress={() => {
                                if (onPress) onPress(step);
                                setIsModalVisible(false);
                            }}
                            style={{}}
                        >
                            Voir détails
                        </Button>
                    </View>
                </Modal>
            </Portal>
        </>
    );
}