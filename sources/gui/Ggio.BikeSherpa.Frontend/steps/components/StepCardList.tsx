import ThemedCardList from "@/components/themed/ThemedCardList";
import StepCard from "@/steps/components/StepCard";
import { Dimensions, StyleProp, ViewStyle } from "react-native";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

type Props = {
    steps: StepToDisplay[],
    onCardPress?: (step: StepToDisplay) => void,
    style?: StyleProp<ViewStyle>
}

export default function StepCardList({ steps, onCardPress, style }: Props) {

    const screenWidth = Dimensions.get('window').width;

    return (
        <ThemedCardList
            data={steps}
            card={({ item }) => {
                const step = item as StepToDisplay;
                return (
                    <StepCard
                        step={step}
                        onPress={(step: StepToDisplay) => onCardPress ? onCardPress(step) : undefined}
                    />
                );
            }}
            style={style}
            numColumns={Math.floor(screenWidth / 260)}
        />
    );
}