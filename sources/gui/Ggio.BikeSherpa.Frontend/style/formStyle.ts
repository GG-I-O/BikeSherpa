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
    checkboxContainer: {
        flexDirection: 'row',
        alignItems: 'center',
        justifyContent: 'flex-start',
        gap: 8,
        width: '80%',
    }
});

export default formStyle;