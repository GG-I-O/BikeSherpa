import {DataTable, IconButton, Text} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import {Control} from "react-hook-form";
import StepTypeInput from "@/steps/components/inputs/StepTypeInput";
import React from "react";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedDateInput from "@/components/themed/ThemedDateInput";
import ThemedTimeInput from "@/components/themed/ThemedTimeInput";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import {View} from "react-native";

type Props = {
    control: Control<any>;
    name: string;
    index: number;
    deleteRow: () => void;
    listLength?: number;
    moveRow?: (from: number, to: number) => void;
}

export default function StepRowInput({control, name, index, deleteRow, moveRow, listLength}: Props) {
    return (
        <DataTable.Row style={{padding: 0}}>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.width130]}>
                <View style={{display: 'flex', flexDirection: 'row', alignItems: 'center'}}>
                    <IconButton
                        icon="trash-can-outline"
                        onPress={() => deleteRow()}
                    />
                    {moveRow && listLength &&
                        <View style={{flexDirection: "column", gap: 0}}>
                            <IconButton
                                style={{margin: 0}}
                                icon="arrow-up-bold"
                                onPress={() => moveRow ? moveRow(index, index - 1) : undefined}
                                disabled={index === 0}
                            />
                            <IconButton
                                style={{margin: 0}}
                                icon="arrow-down-bold"
                                onPress={() => moveRow ? moveRow(index, index + 1) : undefined}
                                disabled={index === listLength - 1}
                            />
                        </View>
                    }
                </View>
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
                    placeholder="Adresse..."
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column]}>
                <ThemedInput
                    testID="StepCommentInput"
                    control={control}
                    name={`${name}.${index}.comment`}
                    label=""
                    placeholder="Commentaire..."
                />
            </DataTable.Cell>
        </DataTable.Row>
    )
        ;
}