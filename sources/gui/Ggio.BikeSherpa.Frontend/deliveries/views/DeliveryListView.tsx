import {navigate} from "expo-router/build/global-state/routing";
import React, {useState} from "react";
import {ScrollView, View} from "react-native";
import {Button, SegmentedButtons, Text, useTheme} from 'react-native-paper';
import {Dropdown, MultiSelectDropdown} from 'react-native-paper-dropdown';
import DeliveryDataTable from "../components/DeliveryDataTable";
import StepDataTableAssign from "@/steps/components/StepDatatableAssign";
import {useDeliverySelection} from "../hooks/useDeliverySelection";
import useDeliveryListViewModel from "@/deliveries/viewModel/useDeliveryListViewModel";
import ThemedConfirmationModal from "@/components/themed/ThemedConfirmationModal";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";
import {DatePickerInput} from "react-native-paper-dates";
import dateFilterDropdown from "@/deliveries/data/dateFilterDropdown";
import dateFilterEnum from "@/deliveries/data/dateFilterEnum";

export function DeliveryListView() {
    const theme = useTheme();

    const [isAssigning, setIsAssigning] = useState<boolean>(false);
    const [courierToAssign, setCourierToAssign] = useState<string>('');
    const [displayConfirmationModal, setDisplayConfirmationModal] = useState(false);

    const viewModel = useDeliveryListViewModel();
    const {
        selectedSteps,
        isStepSelected,
        isDeliverySelected,
        toggleStepSelection,
        toggleDeliverySelection,
        clearSelection
    } = useDeliverySelection();

    return (
        <>
            <ScrollView
                horizontal
                style={{
                    flexDirection: 'row', flexGrow: 0, flexShrink: 0,
                    backgroundColor: theme.colors.background
                }}
                contentContainerStyle={{flexDirection: 'row', gap: 8, alignItems: "center", flexShrink: 1}}
            >
                {!isAssigning ? (
                    <>
                        <SegmentedButtons
                            value={viewModel.dateFilter}
                            onValueChange={viewModel.setDateFilter}
                            buttons={dateFilterDropdown.map(b => ({
                                ...b,
                                style: {width: 100}
                            }))}
                        />
                        <View style={{width: 'auto', overflow: 'hidden'}}>
                            <DatePickerInput
                                locale={"fr"}
                                inputMode={"start"}
                                onChange={(date: Date | undefined): void => viewModel.setDatePicker(date)}
                                value={viewModel.datePicker}
                                disabled={viewModel.dateFilter !== dateFilterEnum.Date}
                            />
                        </View>
                        <MultiSelectDropdown
                            key={`courier-filter-${viewModel.courierFilter || 'empty'}`}
                            label="Assignées à"
                            options={viewModel.couriers}
                            value={viewModel.courierFilter}
                            onSelect={(value) => viewModel.setCourierFilter(value)}
                            mode="outlined"
                        />
                        <Button
                            style={{
                                backgroundColor: theme.colors.background,
                                borderWidth: 1,
                                borderColor: theme.colors.onBackground
                            }}
                            onPress={() => {
                                clearSelection();
                                setIsAssigning(true);
                            }}
                            mode="outlined"
                        >
                            <Text>Assigner des courses</Text>
                        </Button>
                    </>
                ) : (
                    <>
                        <Dropdown
                            key={`courier-filter-${courierToAssign || 'empty'}`}
                            label="Assigner à"
                            options={viewModel.couriers.slice(1)}
                            value={courierToAssign}
                            onSelect={(value) => setCourierToAssign(value ?? '')}
                            mode="outlined"
                        />
                        <Button
                            style={{
                                backgroundColor: theme.colors.background,
                                borderWidth: 1,
                                borderColor: theme.colors.onBackground
                            }}
                            onPress={() => {
                                if (courierToAssign)
                                    viewModel.assignCourier(selectedSteps, courierToAssign);
                                else
                                    viewModel.unassignCourier(selectedSteps);
                                clearSelection();
                                setIsAssigning(false);
                            }}
                        >
                            <Text>Valider</Text>
                        </Button>
                        <Button
                            style={{
                                backgroundColor: theme.colors.background,
                                borderWidth: 1,
                                borderColor: theme.colors.onBackground
                            }}
                            onPress={() => {
                                clearSelection();
                                setIsAssigning(false);
                            }}
                        >
                            <Text>Annuler</Text>
                        </Button>
                    </>
                )}

            </ScrollView>
            {viewModel.courierFilter.length === 0 ? (
                <DeliveryDataTable
                    deliveries={viewModel.deliveries}
                    isDeliverySelected={isAssigning ? isDeliverySelected : undefined}
                    isStepSelected={isAssigning ? isStepSelected : undefined}
                    onDeliveryPress={isAssigning ? toggleDeliverySelection : undefined}
                    onStepPress={isAssigning ? toggleStepSelection : undefined}
                    onDetails={
                        (delivery: DeliveryToDisplay) => navigate({
                            pathname: '/(tabs)/(deliveries)/[deliveryId]',
                            params: {deliveryId: delivery.id}
                        })}
                    onEdit={(delivery: DeliveryToDisplay) => navigate({
                        pathname: '/(tabs)/(deliveries)/edit',
                        params: {deliveryId: delivery.id}
                    })}
                    onCopy={(delivery: DeliveryToDisplay) => navigate({
                        pathname: '/(tabs)/(deliveries)/copy',
                        params: {deliveryId: delivery.id}
                    })}
                    onDelete={(delivery: DeliveryToDisplay) => {
                        viewModel.setDeliveryToDelete(delivery.id);
                        setDisplayConfirmationModal(true);
                    }}
                />
            ) : (
                <StepDataTableAssign
                    steps={viewModel.steps}
                    isStepSelected={isAssigning ? isStepSelected : undefined}
                    onRowPress={isAssigning ? toggleStepSelection : undefined}
                    canChangeDate={true}
                    showHeader={true}
                />
            )}
            <ThemedConfirmationModal
                visible={displayConfirmationModal}
                title="Supprimer la course ?"
                confirmButton={() => {
                    viewModel.deleteDelivery();
                    setDisplayConfirmationModal(false);
                }}
                cancelButton={() => {
                    viewModel.setDeliveryToDelete(null);
                    setDisplayConfirmationModal(false);
                }}
            />
        </>
    );
}