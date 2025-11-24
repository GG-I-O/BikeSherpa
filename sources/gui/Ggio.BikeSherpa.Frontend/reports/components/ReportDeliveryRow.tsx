import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import { Delivery } from "@/deliveries/models/Delivery";
import datatableStyle from "@/style/datatableStyle";
import { DataTable, useTheme } from "react-native-paper";

type Props = {
    delivery: Delivery,
    onPress: () => void
}

export default function ReportDeliveryRow({ delivery, onPress }: Props) {
    const theme = useTheme();
    const style = datatableStyle;
    const pricePrecision = 100;

    return <>
        <DataTable.Row onPress={onPress}>
            <DataTable.Cell style={[style.column, style.courseColumn]}>
                {delivery.steps ? delivery.steps[0].getContractDate() : "Date inconnue"}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.descriptionColumn]}>
                {delivery.code}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.dateColumn]}>
                {delivery.steps ? Math.floor(Math.random() * delivery.steps.length * (10 * pricePrecision - 1 * pricePrecision) + 1 * pricePrecision) / (1 * pricePrecision) : Math.floor(Math.random() * (10 * pricePrecision - 1 * pricePrecision) + 1 * pricePrecision) / (1 * pricePrecision)} €
            </DataTable.Cell>
        </DataTable.Row>

        {delivery.steps ? delivery.steps.map((step) => <>
            <DataTable.Row onPress={onPress} style={{ marginLeft: 110 }}>
                <DataTable.Cell style={[style.column, style.typeColumn]}>
                    <DeliveryTypeIcon type={step.type} />
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.timeColumn]}>
                    {step.getContractTime()}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.descriptionColumn]}>
                    {step.address.complement} {step.address.streetInfo}<br />
                    {step.address.postcode} {step.address.city}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.descriptionColumn]}>
                    {step.comment}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.dateColumn]}>
                    {step.type === 0 ? "" : Math.floor(Math.random() * (10 * pricePrecision - 1 * pricePrecision) + 1 * pricePrecision) / (1 * pricePrecision) + " €"}
                </DataTable.Cell>
            </DataTable.Row></>
        ) : ""}
    </>
}