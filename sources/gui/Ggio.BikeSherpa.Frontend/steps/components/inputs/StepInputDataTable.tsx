import {Button, DataTable, Text, useTheme} from "react-native-paper";
import datatableStyle from "@/style/datatableStyle";
import StepRowInput from "./StepRowInput";
import {Control, useFieldArray} from "react-hook-form";
import {Dimensions, View} from "react-native";

type Props = {
    name: string;
    control: Control<any>
}

export default function StepInputDataTable({name, control}: Props) {
    const theme = useTheme();
    const windowWidth = Dimensions.get('window').width;

    const {fields, append, remove} = useFieldArray({
        name,
        control
    });

    const addStep = () => {
        append({
            stepType: 0,
            comment: '',
            address: {
                name: '',
                fullAddress: '',
                streetInfo: '',
                complement: '',
                postcode: '',
                city: '',
                coordinates: {
                    longitude: 0,
                    latitude: 0
                }
            },
            estimatedDeliveryDate: new Date().toISOString()
        })
    }

    return (
        <View>
            <Button
                onPress={() => addStep()}
            >
                Ajouter une étape
            </Button>
            <DataTable style={{backgroundColor: theme.colors.background, width: windowWidth * 0.9}}>
                <DataTable.Header style={{padding: 0}}>
                    <DataTable.Title style={[datatableStyle.column, datatableStyle.width60]}>
                        <Text>Actions</Text>
                    </DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column, datatableStyle.width60]}>
                        <Text>Type</Text>
                    </DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column]}>
                        <Text>Adresse</Text>
                    </DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column, datatableStyle.width180]}>
                        <Text>Commentaire</Text>
                    </DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column, datatableStyle.widthDatePicker]}>
                        <Text>Date</Text>
                    </DataTable.Title>
                    <DataTable.Title style={[datatableStyle.column, datatableStyle.width40]}>
                        <Text>Heure</Text>
                    </DataTable.Title>
                </DataTable.Header>

                {fields.map((step, index) => (
                    <StepRowInput
                        key={step.id}
                        control={control}
                        name={name}
                        index={index}
                        deleteRow={() => remove(index)}
                    />
                ))}
            </DataTable>
        </View>
    );
}