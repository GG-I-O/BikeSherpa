import { StyleSheet } from "react-native";

const formStyle = StyleSheet.create({
    container: {
        height: '100%',
        padding: 8
    },
    elements: {
        alignItems: 'center',
        gap: 8,
        minWidth: '50%',
        maxWidth: 1000,
        marginInline: 'auto'
    },
    input: {
        width: '100%',
        height: 25
    },
    label: {
        textAlign: 'left',
    },
    button: {
        zIndex: -1
    },
    intputContainer: {
        width: '80%',
        zIndex: -1
    },
    checkboxContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'flex-start',
        gap: 8,
        width: '80%',
        zIndex: -1
    },
    addressOptionList: {
        position: 'fixed',
        marginTop: 45,
        zIndex: 99,
        borderWidth: 2,
        borderTopWidth: 0,
    },
    addressOptionContainer: {
        alignItems: 'flex-start',
        width: '100%'
    },
    addressOptionElement: {
        width: '100%',
        borderRadius: 0
    }
});

export default formStyle;