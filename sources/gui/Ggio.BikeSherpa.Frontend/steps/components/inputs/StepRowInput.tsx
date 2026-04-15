import {DataTable, IconButton} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import {Control} from "react-hook-form";
import StepTypeInput from "@/steps/components/inputs/StepTypeInput";
import React from "react";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedDateInput from "@/components/themed/ThemedDateInput";
import ThemedTimeInput from "@/components/themed/ThemedTimeInput";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";

type Props = {
    control: Control<any>;
    name: string;
    index: number;
    deleteRow: () => void;
}

export default function StepRowInput({control, name, index, deleteRow}: Props) {
    return (
        <DataTable.Row style={{padding: 0}}>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.width60]}>
                <IconButton icon="trash-can-outline" onPress={() => deleteRow()}/>
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.width60]}>
                <StepTypeInput
                    control={control}
                    name={`${name}.${index}.stepType`}
                    testID="StepTypeInput"
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column]}>
                <ThemedAddressInput
                    control={control}
                    name={`${name}.${index}.stepAddress`}
                    label=""
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.width180]}>
                <ThemedInput
                    testID="StepCommentInput"
                    control={control}
                    name={`${name}.${index}.comment`}
                    label=""
                    placeholder="Commentaire..."
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.widthDatePicker]}>
                <ThemedDateInput
                    testID="stepEstimatedDateInput"
                    control={control}
                    name={`${name}.${index}.estimatedDeliveryDate`}
                    label=""
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.width40]}>
                <ThemedTimeInput
                    testID="stepEstimatedDateInput"
                    control={control}
                    name={`${name}.${index}.estimatedDeliveryDate`}
                    label=""
                />
            </DataTable.Cell>
        </DataTable.Row>
    );
}