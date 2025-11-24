import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import { DataTable, IconButton, useTheme } from "react-native-paper";

export default function CourierListView() {
    const theme = useTheme();
    const style = datatableStyle;

    return (
        <ScrollView style={{ backgroundColor: theme.colors.background, height: '100%' }}>
            <DataTable style={{ backgroundColor: theme.colors.background }}>
                <DataTable.Header>
                    <DataTable.Title style={[style.column]}>Nom</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Prénom</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Code</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width80]}>Actions</DataTable.Title>
                </DataTable.Header>

                <DataTable.Row>
                    <DataTable.Cell>MonNom</DataTable.Cell>
                    <DataTable.Cell>MonPrénom</DataTable.Cell>
                    <DataTable.Cell>MMM</DataTable.Cell>
                    <DataTable.Cell style={[style.column, style.width40]}>
                        <IconButton icon="account-edit" />
                    </DataTable.Cell>
                    <DataTable.Cell style={[style.column, style.width40]}>
                        <IconButton icon="trash-can-outline" />
                    </DataTable.Cell>
                </DataTable.Row>
            </DataTable>
        </ScrollView>
    );
}