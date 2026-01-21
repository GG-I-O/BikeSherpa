import { StyleSheet } from "react-native";

const modalStyle = StyleSheet.create({
    modalContainer: {
        justifyContent: "center",
        alignItems: "center",
        padding: 16,
        borderRadius: 8
    },
    safeArea: {
        flex: 1,
        justifyContent: "center",
        alignItems: "center",
        opacity: 0.5,
    },
    title: {
        margin: 16
    },
    buttonContainer: {
        flexDirection: "row",
        justifyContent: "space-between",
        width: "75%"
    }
})
export default modalStyle;