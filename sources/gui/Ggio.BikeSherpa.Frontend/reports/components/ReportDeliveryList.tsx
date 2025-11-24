import { Delivery } from "@/deliveries/models/Delivery";
import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import { DataTable, useTheme } from "react-native-paper";
import useDeliveryViewModel from "@/deliveries/viewModel/DeliveryViewModel";
import ReportDeliveryRow from "./ReportDeliveryRow";

type Props = {
    deliveries: Array<Delivery>,
    onRowPress: (delivery: Delivery) => void
}

export default function ReportDeliveryList({ deliveries, onRowPress }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    return (
        <ScrollView>
            <DataTable style={{ backgroundColor: theme.colors.background }}>
                <DataTable.Header>
                    <DataTable.Title style={[style.column, style.courseColumn]}>
                        Date
                    </DataTable.Title>
                    <DataTable.Title style={[style.column, style.descriptionColumn]}>
                        Code
                    </DataTable.Title>
                    <DataTable.Title style={[style.column, style.dateColumn]}>
                        Prix
                    </DataTable.Title>
                </DataTable.Header>

                {deliveries.map((delivery) => (
                    <ReportDeliveryRow
                        delivery={delivery}
                        onPress={() => {
                            onRowPress(delivery);
                        }}
                    />
                ))}
            </DataTable>
        </ScrollView>
    );
}