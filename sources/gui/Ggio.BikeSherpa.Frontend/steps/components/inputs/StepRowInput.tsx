import {DataTable, IconButton} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import {Control, FieldError, FieldErrors} from "react-hook-form";
import StepTypeInput from "@/steps/components/inputs/StepTypeInput";
import React from "react";
import ThemedInput from "@/components/themed/ThemedInput";
import ThemedAddressInput from "@/components/themed/ThemedAddressInput";
import {View} from "react-native";
import ThemedCheckboxInput from "@/components/themed/ThemedCheckboxInput";
import ThemedDropdownInput from "@/components/themed/ThemedDropdownInput";
import useDropdown from "@/hooks/useDropdown";

type Props = {
    control: Control<any>;
    errors: FieldErrors;
    name: string;
    index: number;
    deleteRow: () => void;
    listLength?: number;
    moveRow?: (from: number, to: number) => void;
}

export default function StepRowInput({control, errors, name, index, deleteRow, moveRow, listLength}: Props) {
    const { packingSizes } = useDropdown();
    
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
                <View style={{flexDirection: 'column', width: '100%', gap: 4}}>
                    <ThemedAddressInput
                        control={control}
                        name={`${name}.${index}.stepAddress`}
                        error={errors.steps ? (errors.steps as any)[index]?.stepAddress?.streetInfo as FieldError | undefined : undefined}
                        label=""
                        placeholder="Adresse..."
                    />
                    <ThemedInput
                        testID="StepAddressName"
                        control={control}
                        name={`${name}.${index}.contactName`}
                        error={errors.steps ? (errors.steps as any)[index]?.contactName as FieldError | undefined : undefined}
                        label=""
                        placeholder="Nom..."
                    />
                    <ThemedInput
                        testID="StepAddressPhone"
                        control={control}
                        name={`${name}.${index}.contactPhone`}
                        error={errors.steps ? (errors.steps as any)[index]?.contactPhone as FieldError | undefined : undefined}
                        label=""
                        placeholder="Téléphone..."
                    />
                </View>
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column, datatableStyle.width90]}>
                <ThemedCheckboxInput
                    testID="StepNotBilledInput"
                    control={control}
                    name={`${name}.${index}.notBilled`}
                    label=""
                />
            </DataTable.Cell>
            <DataTable.Cell style={[datatableStyle.column]}>
                <ThemedDropdownInput
                    testID="StepPackingSizeInput"
                    control={control}
                    name={`${name}.${index}.packingSize`}
                    error={errors.steps ? (errors.steps as any)[index]?.packingSize as FieldError | undefined : undefined}
                    label="Taille"
                    options={packingSizes}
                    required
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
    );
}