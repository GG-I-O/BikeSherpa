import { StyleSheet } from "react-native";

const datatableStyle = StyleSheet.create({
    column: {
        marginInline: 4
    },
    headerColumn: {
        textAlign: 'center'
    },
    courseColumn: {
        minWidth: 100,
        maxWidth: 100
    },
    typeColumn: {
        minWidth: 40,
        maxWidth: 40
    },
    inputTypeColumn: {
        minWidth: 80,
        maxWidth: 80
    },
    dateColumn: {
        minWidth: 90,
        maxWidth: 90
    },
    inputDateColumn: {
        minWidth: 160,
        maxWidth: 160
    },
    timeColumn: {
        minWidth: 60,
        maxWidth: 60
    },
    inputTimeColumn: {
        minWidth: 100,
        maxWidth: 100
    },
    descriptionColumn: {
        minWidth: 100
    },
    adresseColumn: {
        minWidth: 150
    },
    buttonColumn: {
        minWidth: 60,
        maxWidth: 60
    },
    timeIncrementColumn: {
        minWidth: 130,
        maxWidth: 130
    }
});

export default datatableStyle;