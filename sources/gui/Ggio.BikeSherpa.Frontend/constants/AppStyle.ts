import { Dimensions, StyleSheet } from "react-native";

export default class AppStyle {
    public static formStyle = StyleSheet.create({
        container: {
            minHeight: '100%',
            paddingBlock: 16,
            paddingInline: Dimensions.get('window').width / 10,
            gap: 16
        }
    });

    public static textStyle = StyleSheet.create({
        h1: {
            fontSize: 30
        },
        h2: {
            fontSize: 24
        },
        h3: {
            fontSize: 20
        }
    })
}