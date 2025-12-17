import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import { DataTable, IconButton, useTheme } from "react-native-paper";
import useCustomerViewModel from "../viewModel/CustomerViewModel";
import { observe } from "@legendapp/state";
import { useEffect, useState } from "react";
import Customer from "../models/Customer";

export default function CustomerListView() {
    const theme = useTheme();
    const backgroundColor = theme.colors.background;

    const viewModel = useCustomerViewModel();
    const customerStore$ = viewModel.getCustomerList$();
    const [customerList, setCustomerList] = useState<Customer[]>([]);

    useEffect(() => {
        return observe(() => {
            const record = customerStore$.get() ?? {};
            setCustomerList(Object.values(record));
        });
    }, [customerStore$]);

    return (
        <ScrollView style={{ backgroundColor, height: '100%' }}>
            <DataTable style={{ backgroundColor }}>
                <DataTable.Header>
                    <DataTable.Title style={[datatableStyle.column]}>Nom</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>Code</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>Num Tél</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>Adresse</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>SIRET</DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column, datatableStyle.width80]}>Actions</DataTable.Title>
                </DataTable.Header>

                {customerList.map((customer) => (
                    <DataTable.Row key={customer.id}>
                        <DataTable.Cell>{customer.name}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{customer.code}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{customer.phoneNumber}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{`${customer.address?.streetInfo ?? ''} ${customer.address?.postcode ?? ''} ${customer.address?.city ?? ''}`.trim()}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column]}>{customer.siret}</DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column, datatableStyle.width40]}>
                            <IconButton icon="account-edit" />
                        </DataTable.Cell>
                        <DataTable.Cell style={[datatableStyle.column, datatableStyle.width40]}>
                            <IconButton icon="trash-can-outline" />
                        </DataTable.Cell>
                    </DataTable.Row>
                ))}
            </DataTable>
        </ScrollView>
    );
}
