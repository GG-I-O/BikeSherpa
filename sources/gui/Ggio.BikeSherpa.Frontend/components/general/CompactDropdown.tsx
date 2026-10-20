import {View} from "react-native";
import {Dropdown} from "react-native-paper-dropdown";

const COMPACT_INPUT_WIDTH = 84;

export default function CompactDropdown({value, options, onSelect}: {
    value: string;
    options: any[];
    onSelect: (value?: string) => void;
}) {
    return (
        <View style={{width: COMPACT_INPUT_WIDTH}}>
            <Dropdown
                value={value}
                onSelect={onSelect}
                mode="outlined"
                options={options}
                hideMenuHeader
            />
        </View>
    );
}