import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import { DataTable, IconButton, useTheme } from "react-native-paper";
import useCourierListViewModel from "../viewModels/useCourierListViewModel";
import ThemedConfirmationModal from "@/components/themed/ThemedConfirmationModal";
import { useState } from "react";

export default function CourierListView() {
    const theme = useTheme();
    const backgroundColor = theme.colors.background;
    const { courierList, displayEditForm, deleteCourier, setCourierToDelete } = useCourierListViewModel();
    const [displayConfirmationModal, setDisplayConfirmationModal] = useState(false);

    return (
        <ScrollView testID="courierListView" style={{ backgroundColor, height: '100%' }}>
            <DataTable style={{ backgroundColor }}>
                <DataTable.Header>
                    <DataTable.Title style={[datatableStyle.column]}>Prénom</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>Nom</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>Code</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>Num Tél</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>E-mail</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>Adresse</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column, datatableStyle.width80]}>Actions</DataTable.Title>
                </DataTable.Header>

                {courierList.map((courier, index) => (
                    <DataTable.Row testID={`courierList${index}`} key={index}>
                        <DataTable.Cell style={[datatableStyle.column]}>{courier.firstName}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{courier.lastName}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{courier.code}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{courier.phoneNumber}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{courier.email}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{`${courier.address?.streetInfo ?? ''} ${courier.address?.postcode ?? ''} ${courier.address?.city ?? ''}`.trim()}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column, datatableStyle.width40]}>
                            <IconButton testID={`editButton${index}`} icon="account-edit" onPress={() => displayEditForm(courier.id)} />
                        </DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column, datatableStyle.width40]}>
                            <IconButton testID={`deleteButton${index}`} icon="trash-can-outline" onPress={() => {
                                setDisplayConfirmationModal(true);
                                setCourierToDelete(courier.id);
                            }
                            } />
                        </DataTable.Cell>
                    </DataTable.Row>
                ))}
            </DataTable>
            <ThemedConfirmationModal
                visible={displayConfirmationModal}
                title="Supprimer le livreur ?"
                confirmButton={() => {
                    deleteCourier();
                    setDisplayConfirmationModal(false);
                }}
                cancelButton={() => {
                    setDisplayConfirmationModal(false);
                }} />
        </ScrollView>
    );
}