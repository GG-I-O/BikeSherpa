import {View} from "react-native";
import {Button, Divider, Text, useTheme} from "react-native-paper";
import AppStyle from "@/constants/AppStyle";
import DateToolbox from "@/services/DateToolbox";
import usePublicDeliverySummaryViewModel from "@/deliveries/viewModel/usePublicDeliverySummaryViewModel";
import PublicDeliveryCustomerForm from "@/deliveries/components/PublicDeliveryCustomerForm";
import PublicDeliveryCustomerType from "@/deliveries/components/PublicDeliveryCustomerType";
import {KeyboardAwareScrollView} from "react-native-keyboard-controller";
import formStyle from "@/style/formStyle";
import React from "react";
import {StepType} from "@/steps/models/StepType";
import PublicDeliveryConfirmModal from "@/deliveries/components/PublicDeliveryConfirmModal";
import usePublicDeliveryModal from "@/deliveries/hooks/usePublicDeliveryModal";

export default function PublicDeliverySummaryView() {
    const theme = useTheme();

    const viewModel = usePublicDeliverySummaryViewModel();
    const {delivery, customer, estimatedValue} = viewModel;

    const {isConfirmModalVisible, setIsConfirmModalVisible, onConfirmModalDismiss} = usePublicDeliveryModal();

    if (!delivery || !estimatedValue)
        return (
            <View style={{backgroundColor: theme.colors.background, alignItems: 'center', justifyContent: 'center'}}>
                <Text>Erreur lors du chargement de la commande</Text>
            </View>
        );

    return (
        <KeyboardAwareScrollView
            style={{flex: 1, backgroundColor: theme.colors.background}}
            contentContainerStyle={{flexGrow: 1, gap: 16, paddingHorizontal: 32, paddingVertical: 16}}
            keyboardShouldPersistTaps="handled"
        >
            <PublicDeliveryConfirmModal
                visible={isConfirmModalVisible}
                setVisible={setIsConfirmModalVisible}
                onDismiss={onConfirmModalDismiss}/>
            <Text style={[AppStyle.textStyle.h1, {textAlign: 'center'}]}>Récapitulatif de la demande de livraison</Text>

            {delivery.steps.map((step, index) => (
                <View key={index} style={{gap: 8}}>
                    <Text
                        style={AppStyle.textStyle.h3}>
                        {step.stepType === 0 ? "Adresse d'enlèvement" : `Adresse de livraison ${index}`}
                    </Text>
                    <Text
                        style={{marginBottom: 8}}>
                        {`${step.stepAddress.streetInfo} ${step.stepAddress.postcode} ${step.stepAddress.city}`}
                    </Text>
                    {step.stepType === StepType.Drop && (
                        <Text>Colisage
                            : {viewModel.getPackingSizeLabel(step.packingSize)?.label ?? step.packingSize}</Text>
                    )}
                </View>
            ))}

            <Divider/>
            <Text style={AppStyle.textStyle.h2}>Créneau de livraison</Text>
            <Text>Date de la livraison: {DateToolbox.getFormattedDateFromISO(delivery.startDate)} à partir
                de {DateToolbox.getFormattedTimeFromISO(delivery.startDate)}</Text>
            <Text>Durée créneau de livraison: {viewModel.urgencyLabel?.label ?? ''}</Text>

            <Divider/>
            <View style={{
                flexDirection: 'row',
                gap: 8,
                flexWrap: 'wrap',
                justifyContent: 'space-evenly',
                alignItems: 'flex-end'
            }}>
                <Text>Montant HT: {estimatedValue.price.toFixed(2)} €</Text>
                <Text style={AppStyle.textStyle.h3}>Total TTC: {estimatedValue.priceWithVat.toFixed(2)} €</Text>
            </View>

            <Divider/>
            <Text style={[AppStyle.textStyle.h1, {textAlign: 'center'}]}>Coordonnées de facturation</Text>
            {!customer && (
                <PublicDeliveryCustomerType
                    customerType={viewModel.customerType}
                    setCustomerType={viewModel.setCustomerType}
                />
            )}
            <PublicDeliveryCustomerForm
                control={viewModel.control}
                errors={viewModel.errors}
                customerType={viewModel.customerType}
                knownCustomer={customer !== null}
            />
            <Divider/>
            <Button
                testID="formButton"
                mode="outlined"
                onPress={() => viewModel.handleSubmit()}
                style={[formStyle.button, {backgroundColor: theme.colors.secondaryContainer}]}
            >
                <Text testID="buttonName" style={{fontWeight: 'bold'}}>Valider ma demande de livraison (2/2)</Text>
            </Button>
        </KeyboardAwareScrollView>
    );
}