import React, {createContext, useCallback, useContext, useMemo, useState} from "react";
import {StyleProp, View, ViewStyle} from "react-native";

type InputGroupContextValue = {
    labelWidth: number;
    reportLabelWidth: (width: number) => void;
};

const InputGroupContext = createContext<InputGroupContextValue | null>(null);

export const useInputGroup = () => useContext(InputGroupContext);

type Props = {
    children: React.ReactNode;
    maxLabelWidth?: number;
    style?: StyleProp<ViewStyle>;
};

export default function InputGroup({children, maxLabelWidth = 160, style}: Props) {
    const [labelWidth, setLabelWidth] = useState(0);

    const reportLabelWidth = useCallback((width: number) => {
        const next = Math.min(Math.ceil(width), maxLabelWidth);
        setLabelWidth(prev => (next > prev ? next : prev));
    }, [maxLabelWidth]);

    const value = useMemo(() => ({labelWidth, reportLabelWidth}), [labelWidth, reportLabelWidth]);

    return (
        <InputGroupContext.Provider value={value}>
            <View style={[{width: "100%"}, style]}>{children}</View>
        </InputGroupContext.Provider>
    );
}