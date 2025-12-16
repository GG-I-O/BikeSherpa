import { navigate } from "expo-router/build/global-state/routing";
import { useEffect, useState } from "react";
import { Dimensions, ScrollView } from "react-native";
import { Button, SegmentedButtons, Text, useTheme } from 'react-native-paper';
import { Dropdown } from 'react-native-paper-dropdown';
import { Delivery } from "../models/Delivery";
import { Step } from "@/steps/models/Step";
import useDeliveryViewModel from "../viewModel/DeliveryViewModel";
import DeliveryCardList from "../components/DeliveryCardList";
import DeliveryDataTable from "../components/DeliveryTadaTable";
import StepDataTableAssign from "@/steps/components/StepDatatableAssign";
import { useDeliverySelection } from "../hooks/useDeliverySelection";

export function DeliveryListView() {
    const theme = useTheme();

    const [dateFilter, setDateFilter] = useState<string>('1');
    const [courierFilter, setCourierFilter] = useState<string>('NONE');
    const [isAssigning, setIsAssigning] = useState<boolean>(false);
    const [assignDropdown, setAssignDropdown] = useState<string>('NONE');

    // Data
    const [deliveries, setDeliveries] = useState<Delivery[]>([]);
    const [steps, setSteps] = useState<Step[]>([]);

    const viewModel = useDeliveryViewModel();
    const { selectedSteps, isStepSelected, isDeliverySelected, toggleStepSelection, toggleDeliverySelection, clearSelection } = useDeliverySelection();

    useEffect(() => {
        setDeliveries(viewModel.getFilteredDeliveries(dateFilter, courierFilter));
        setSteps(viewModel.getFilteredStepList(dateFilter, courierFilter));
    }, [viewModel, dateFilter, courierFilter, isAssigning]);

    const assignSteps = (courier: string, steps: Step[]) => {
        viewModel.assignSteps(courier, steps);
    }

    const screenWidth = Dimensions.get('window').width;

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
                            style={{ height: "auto", flex: 1, alignItems: "center" }}
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
                            style={{ backgroundColor: theme.colors.background, borderWidth: 1, borderColor: theme.colors.onBackground }}
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
                            style={{ backgroundColor: theme.colors.background, borderWidth: 1, borderColor: theme.colors.onBackground }}
                            onPress={() => {
                                assignSteps(assignDropdown, selectedSteps);
                                clearSelection();
                                setIsAssigning(false);
                            }}
                        >
                            <Text>Valider</Text>
                        </Button>
                        <Button
                            style={{ backgroundColor: theme.colors.background, borderWidth: 1, borderColor: theme.colors.onBackground }}
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
            {screenWidth <= 992 ? (
                <DeliveryCardList
                    deliveries={deliveries}
                    isDeliverySelected={isAssigning ? isDeliverySelected : undefined}
                    onDeliveryPress={isAssigning ?
                        toggleDeliverySelection :
                        (delivery: Delivery) => navigate({
                            pathname: '/(tabs)/(deliveries)/assign',
                            params: { deliveryId: delivery.id }
                        })
                    }
                />
            ) : (
                <>
                    {courierFilter === 'NONE' ? (
                        <DeliveryDataTable
                            deliveries={deliveries}
                            isDeliverySelected={isAssigning ? isDeliverySelected : undefined}
                            isStepSelected={isAssigning ? isStepSelected : undefined}
                            onDeliveryPress={isAssigning ? toggleDeliverySelection : undefined}
                            onStepPress={isAssigning ? toggleStepSelection : undefined}
                            onDetails={
                                (delivery: Delivery) => navigate({
                                    pathname: '/(tabs)/(deliveries)/[deliveryId]',
                                    params: { deliveryId: delivery.id }
                                })}
                            onEdit={(delivery: Delivery) => navigate({
                                pathname: '/(tabs)/(deliveries)/edit',
                                params: { deliveryId: delivery.id }
                            })}
                            onCopy={(delivery: Delivery) => navigate({
                                pathname: '/(tabs)/(deliveries)/copy',
                                params: { deliveryId: delivery.id }
                            })}
                            onDelete={(delivery: Delivery) => navigate({
                                pathname: '/(tabs)/(deliveries)/[deliveryId]',
                                params: { deliveryId: delivery.id }
                            })}
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
                                        params: { deliveryId: delivery.id }
                                    })
                            }}
                            canChangeDate={true}
                            showHeader={true}
                        />
                    )}
                </>
            )}
        </>
    );
}