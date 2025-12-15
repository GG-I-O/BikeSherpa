import { Delivery } from "@/deliveries/models/Delivery";
import datatableStyle from "@/style/datatableStyle";
import { ScrollView } from "react-native";
import { DataTable, useTheme } from "react-native-paper";
import ReportDeliveryRow from "./ReportDeliveryRow";

type Props = {
    deliveries: Delivery[],
    onRowPress?: (delivery: Delivery) => void
}

export default function ReportDeliveryList({ deliveries, onRowPress }: Props) {
    const theme = useTheme();
    const style = datatableStyle;

    return (
        <ScrollView>
            <DataTable style={{ backgroundColor: theme.colors.background }}>
                <DataTable.Header>
                    <DataTable.Title style={[style.column, style.width100]}>
                        Date
                    </DataTable.Title>
                    <DataTable.Title style={[style.column, style.minWidth100]}>
                        Code
                    </DataTable.Title>
                    <DataTable.Title style={[style.column, style.width90]}>
                        Prix
                    </DataTable.Title>
                </DataTable.Header>

                {deliveries.map((delivery, index) => (
                    <ReportDeliveryRow key={index}
                        delivery={delivery}
                        onPress={() => {
                            onRowPress ? onRowPress(delivery) : null;
                        }}
                    />
                ))}
            </DataTable>
        </ScrollView>
    );
}