import {navigate} from "expo-router/build/global-state/routing";
import {useEffect, useState} from "react";
import {ScrollView} from "react-native";
import {Button, SegmentedButtons, Text, useTheme} from 'react-native-paper';
import {Dropdown} from 'react-native-paper-dropdown';
import DeliveryDataTable from "../components/DeliveryDataTable";
import StepDataTableAssign from "@/steps/components/StepDatatableAssign";
import {useDeliverySelection} from "../hooks/useDeliverySelection";
import useDeliveryListViewModel from "@/deliveries/viewModel/useDeliveryListViewModel";
import ThemedConfirmationModal from "@/components/themed/ThemedConfirmationModal";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {DeliveryToDisplay} from "@/deliveries/models/DeliveryToDisplay";

export function DeliveryListView() {
    const theme = useTheme();

    const [dateFilter, setDateFilter] = useState<string>('1');
    const [courierFilter, setCourierFilter] = useState<string>('NONE');
    const [isAssigning, setIsAssigning] = useState<boolean>(false);
    const [assignDropdown, setAssignDropdown] = useState<string>('NONE');
    const [displayConfirmationModal, setDisplayConfirmationModal] = useState(false);

    // Data
    const [deliveries, setDeliveries] = useState<DeliveryToDisplay[]>([]);
    const [steps, setSteps] = useState<StepToDisplay[]>([]);

    const viewModel = useDeliveryListViewModel();
    const {
        selectedSteps,
        isStepSelected,
        isDeliverySelected,
        toggleStepSelection,
        toggleDeliverySelection,
        clearSelection
    } = useDeliverySelection();

    useEffect(() => {
        setDeliveries(viewModel.getFilteredDeliveries(dateFilter, courierFilter));
        setSteps(viewModel.getFilteredStepList(dateFilter, courierFilter));
    }, [viewModel, dateFilter, courierFilter, isAssigning]);

    return (
        <>
            <ScrollView
                horizontal
                style={{
                    flexDirection: 'row', flexGrow: 0, flexShrink: 0,
                    backgroundColor: theme.colors.background
                }}
                contentContainerStyle={{
                    alignItems: "center", gap: 8
                }}
            >
                {!isAssigning ? (
                    <>
                        <SegmentedButtons
                            style={{height: "auto", flex: 1, alignItems: "center"}}
                            value={dateFilter}
                            onValueChange={setDateFilter}
                            buttons={[
                                {
                                    value: '1',
                                    label: 'Aujourd\'hui'
                                },
                                {
                                    value: '7',
                                    label: 'Semaine'
                                },
                                {
                                    value: '365',
                                    label: 'Toutes'
                                }
                            ]}
                        />
                        <Dropdown
                            label="Assignées à"
                            options={[]}
                            value={courierFilter}
                            onSelect={(value) => setCourierFilter(value ?? 'NONE')}
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
                            label="Assigner à"
                            options={[]}
                            value={assignDropdown}
                            onSelect={(value) => setAssignDropdown(value ?? 'NONE')}
                        />
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
            {courierFilter === 'NONE' ? (
                <DeliveryDataTable
                    deliveries={deliveries}
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
                    steps={steps}
                    isStepSelected={isAssigning ? isStepSelected : undefined}
                    onRowPress={(step) => {
                        const delivery = deliveries.find((d) => d.code === step.id);
                        if (!delivery) return
                        if (isAssigning)
                            toggleStepSelection(step, delivery);
                        else
                            navigate({
                                pathname: '/(tabs)/(deliveries)/[deliveryId]',
                                params: {deliveryId: delivery.id}
                            })
                    }}
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