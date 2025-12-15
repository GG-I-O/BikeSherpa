import DeliveryTypeIcon from "@/deliveries/components/DeliveryTypeIcon";
import { Delivery } from "@/deliveries/models/Delivery";
import datatableStyle from "@/style/datatableStyle";
import { DataTable } from "react-native-paper";

type Props = {
    delivery: Delivery
}

export default function ReportDeliveryRow({ delivery }: Props) {
    const style = datatableStyle;
    const pricePrecision = 100;

    return <>
        <DataTable.Row>
            <DataTable.Cell style={[style.column, style.width100]}>
                {delivery.steps ? delivery.steps[0].getContractDate() : "Date inconnue"}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.minWidth100]}>
                {delivery.code}
            </DataTable.Cell>
            <DataTable.Cell style={[style.column, style.width90]}>
                {delivery.steps ? Math.floor(Math.random() * delivery.steps.length * (10 * pricePrecision - 1 * pricePrecision) + 1 * pricePrecision) / (1 * pricePrecision) : Math.floor(Math.random() * (10 * pricePrecision - 1 * pricePrecision) + 1 * pricePrecision) / (1 * pricePrecision)} €
            </DataTable.Cell>
        </DataTable.Row>

        {delivery.steps ? delivery.steps.map((step) => <>
            <DataTable.Row style={{ marginLeft: 110 }}>
                <DataTable.Cell style={[style.column, style.width40]}>
                    <DeliveryTypeIcon type={step.type} />
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.width60]}>
                    {step.getContractTime()}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.minWidth100]}>
                    {step.address.complement} {step.address.streetInfo}<br />
                    {step.address.postcode} {step.address.city}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.minWidth100]}>
                    {step.comment}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.width90]}>
                    {step.type === 0 ? "" : Math.floor(Math.random() * (10 * pricePrecision - 1 * pricePrecision) + 1 * pricePrecision) / (1 * pricePrecision) + " €"}
                </DataTable.Cell>
            </DataTable.Row></>
        ) : ""}
    </>
}