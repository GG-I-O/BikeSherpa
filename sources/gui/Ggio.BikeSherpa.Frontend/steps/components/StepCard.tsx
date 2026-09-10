import { Dimensions, Pressable, View } from "react-native";
import { Button, Card, Divider, Modal, Portal, Text, useTheme } from "react-native-paper";
import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import { useState } from "react";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { IAddressService } from "@/spi/AddressSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

type Props = {
    step: StepToDisplay,
    onPress?: (step: StepToDisplay) => void,
    isSelected?: boolean
}

export default function StepCard({ step, onPress, isSelected = false }: Props) {
    const theme = useTheme();

    const [isModalVisible, setIsModalVisible] = useState<boolean>(false);

    const addressService = IOCContainer.get<IAddressService>(ServicesIdentifiers.AddressService);

    const screenWidth = Dimensions.get('window').width;

    return (
        <>
            <Card
                style={{
                    backgroundColor: isSelected ? theme.colors.primary : theme.colors.background,
                    width: screenWidth >= 350 ? 300 : 'auto'
                }}
                onPress={() => {
                    if (onPress) onPress(step);
                }}
            >
                <Card.Content>
                    <View style={{ flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' }}>
                        <DeliveryTypeIcon type={step.type} />
                        <Text>{step.deliveryCode}</Text>
                        <Text style={{ fontWeight: "bold"}}>{step.estimatedTime}</Text>
                    </View>
                    <Divider />
                    <Pressable
                        style={{ justifyContent: 'space-evenly', marginInline: 8, marginTop: 4 }}
                        onPress={() => {
                            setIsModalVisible(true);
                        }}
                    >
                        <Text style={{textAlign: 'right', fontStyle: 'italic', marginLeft: 24}} numberOfLines={1}>{step.address.name}</Text>
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
                                addressService.openAddressInMaps(`${step.address.streetInfo}, ${step.address.postcode} ${step.address.city}`);
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