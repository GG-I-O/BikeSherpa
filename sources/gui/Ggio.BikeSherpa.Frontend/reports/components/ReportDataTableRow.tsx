import { DataTable, useTheme } from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import {Report} from "@/reports/models/Report";
import AppStyle from "@/constants/AppStyle";
import {DeliveryReport} from "@/reports/models/DeliveryReport";

type Props = {
    report: DeliveryReport,
}

export default function ReportDataTableRow({ report }: Props) {
    const theme = useTheme();

    const style = datatableStyle;

    return (
        <>
            <DataTable.Row
                key={report.deliveryCode}
                testID="ReportDataTableRow"
                style={{ backgroundColor: theme.colors.background }}
            >
                <DataTable.Cell style={[style.column]} textStyle={AppStyle.textStyle.h3}>
                    {`${report.deliveryCode} - ${report.deliveryDate} - ${report.deliveryTime}`}
                </DataTable.Cell>
            </DataTable.Row>
            {report.details.map((detail, index) => (
                <DataTable.Row
                    key={`${report.deliveryCode}-${index}`}
                    testID="ReportDataTableDetailRow"
                    style={[
                        style.smallHeight,
                        { backgroundColor: theme.colors.background, marginLeft: 32 }
                    ]}
                >
                    <DataTable.Cell style={[style.column]}>
                        {detail.description}
                    </DataTable.Cell>
                    <DataTable.Cell style={[style.column, style.width80]} textStyle={{width: '100%'}}>
                        {detail.price} €
                    </DataTable.Cell>
                    <DataTable.Cell style={[style.column, style.width80]} textStyle={{width: '100%', textAlign: 'center'}}>
                        {detail.quantity}
                    </DataTable.Cell>
                </DataTable.Row>
            ))}
            <DataTable.Row
                key={`${report.deliveryCode}-total`}
                testID="ReportDataTableRowTotal"
                style={{ backgroundColor: theme.colors.background }}
            >
                <DataTable.Cell style={[style.column]} textStyle={AppStyle.textStyle.h3}>
                    Total :
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.width80]} textStyle={AppStyle.textStyle.h3}>
                    {report.deliveryPrice} €
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.width80]}>
                    {""}
                </DataTable.Cell>
            </DataTable.Row>
        </>
    );
}