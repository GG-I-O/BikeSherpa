import ThemedCardList from "@/components/themed/ThemedCardList";
import { Step } from "../models/Step"
import StepCard from "@/steps/components/StepCard";
import { Dimensions, StyleProp, ViewStyle } from "react-native";

type Props = {
    steps: Step[],
    onCardPress?: (step: Step) => void,
    style?: StyleProp<ViewStyle>
}

export default function StepCardList({ steps, onCardPress, style }: Props) {

    const screenWidth = Dimensions.get('window').width;

    return (
        <ThemedCardList
            data={steps}
            card={({ item }) => {
                const step = item as Step;
                return (
                    <StepCard
                        step={step}
                        onPress={(step: Step) => onCardPress ? onCardPress(step) : undefined}
                    />
                );
            }}
            style={style}
            numColumns={Math.floor(screenWidth / 260)}
        />
    );
}