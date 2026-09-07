import {DataTable, Text, useTheme} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import AppStyle from "@/constants/AppStyle";
import {DeliveryReport} from "@/reports/models/DeliveryReport";
import {View} from "react-native";

type Props = {
    report: DeliveryReport,
}

export default function ReportDataTableRow({report}: Props) {
    const theme = useTheme();

    const style = datatableStyle;

    return (
        <View style={{marginBottom: 32}}>
            <DataTable.Row
                key={report.deliveryLabel}
                testID="ReportDataTableRow"
                style={{backgroundColor: theme.colors.background}}
            >
                <DataTable.Cell style={[style.column]} textStyle={AppStyle.textStyle.h3}>
                    {report.deliveryLabel}
                </DataTable.Cell>
                <DataTable.Cell style={[style.column, style.width80]} textStyle={AppStyle.textStyle.h3}>
                    {`${report.deliveryPrice} €`}
                </DataTable.Cell>
            </DataTable.Row>

            {report.details.map((detail, index) => (
                <View key={`${report.deliveryLabel}-${index}`} style={{marginLeft: 32}}>
                    <DataTable.Row
                        testID="ReportDataTableDetailRow"
                        style={[style.smallHeight, {backgroundColor: theme.colors.background}]}
                    >
                        <DataTable.Cell style={[style.column]}>
                            {detail.description}
                        </DataTable.Cell>
                        <DataTable.Cell style={[style.column, style.width80]} textStyle={{width: '100%'}}>
                            {`${detail.price} €`}
                        </DataTable.Cell>
                    </DataTable.Row>

                    {!!detail.address?.fullAddress && detail.address.fullAddress !== "" && (
                        <DataTable.Row
                            key={`${report.deliveryLabel}-${index}-address`}
                            style={[style.smallHeight, {backgroundColor: theme.colors.background}]}
                        >
                            <Text style={{marginLeft: 86}}>{detail.address.fullAddress}</Text>
                        </DataTable.Row>
                    )}
                </View>
            ))}
        </View>
    );
}