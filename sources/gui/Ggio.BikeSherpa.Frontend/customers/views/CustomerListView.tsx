import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import { DataTable, IconButton, useTheme } from "react-native-paper";
import useCustomerViewModel from "../viewModel/CustomerViewModel";
import { use$ } from "@legendapp/state/react";

export default function CustomerListView() {
    const theme = useTheme();
    const style = datatableStyle;

    const viewModel = useCustomerViewModel();
    const customerList$ = viewModel.getCustomerList$();
    const customerRecord = use$(customerList$);
    const customerList = Object.values(customerRecord);

    return (
        <ScrollView style={{ backgroundColor: theme.colors.background, height: '100%' }}>
            <DataTable style={{ backgroundColor: theme.colors.background }}>
                <DataTable.Header>
                    <DataTable.Title style={[style.column]}>Nom</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Code</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Num Tél</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Adresse</DataTable.Title>
                    <DataTable.Title style={[style.column]}>SIRET</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width80]}>Actions</DataTable.Title>
                </DataTable.Header>

                {customerList.map((customer, index) => (
                    <DataTable.Row key={index}>
                        <DataTable.Cell>{customer.name}</DataTable.Cell>
                        <DataTable.Cell style={[style.column]}>{customer.code}</DataTable.Cell>
                        <DataTable.Cell style={[style.column]}>{customer.phoneNumber}</DataTable.Cell>
                        <DataTable.Cell style={[style.column]}>{`${customer.address.streetInfo} ${customer.address.postcode} ${customer.address.city}`}</DataTable.Cell>
                        <DataTable.Cell style={[style.column]}>{customer.siret}</DataTable.Cell>
                        <DataTable.Cell style={[style.column, style.width40]}>
                            <IconButton icon="account-edit" />
                        </DataTable.Cell>
                        <DataTable.Cell style={[style.column, style.width40]}>
                            <IconButton icon="trash-can-outline" />
                        </DataTable.Cell>
                    </DataTable.Row>
                ))}
            </DataTable>
        </ScrollView>
    );
}