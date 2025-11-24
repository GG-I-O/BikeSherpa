import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import { DataTable, IconButton, useTheme } from "react-native-paper";

export default function CustomerListView() {
    const theme = useTheme();
    const style = datatableStyle;

    return (
        <ScrollView style={{ backgroundColor: theme.colors.background, height: '100%' }}>
            <DataTable style={{ backgroundColor: theme.colors.background }}>
                <DataTable.Header>
                    <DataTable.Title style={[style.column]}>Nom</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Code</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Adresse</DataTable.Title>
                    <DataTable.Title style={[style.column]}>SIRET</DataTable.Title>
                    <DataTable.Title style={[style.column]}>Commentaire</DataTable.Title>
                    <DataTable.Title style={[style.column, style.width80]}>Actions</DataTable.Title>
                </DataTable.Header>

                <DataTable.Row>
                    <DataTable.Cell>NomSociété</DataTable.Cell>
                    <DataTable.Cell style={[style.column]}>NSO</DataTable.Cell>
                    <DataTable.Cell style={[style.column]}>10 avenue de la république, 38100 Grenoble</DataTable.Cell>
                    <DataTable.Cell style={[style.column]}>362 521 879 00034</DataTable.Cell>
                    <DataTable.Cell style={[style.column]}>Commentaire élogieux.</DataTable.Cell>
                    <DataTable.Cell style={[style.column, style.typeColumn]}>
                        <IconButton icon="account-edit" />
                    </DataTable.Cell>
                    <DataTable.Cell style={[style.column, style.typeColumn]}>
                        <IconButton icon="trash-can-outline" />
                    </DataTable.Cell>
                </DataTable.Row>
            </DataTable>
        </ScrollView>
    );
}