import React, {useEffect, useRef, useState} from 'react';
import {View, FlatList} from 'react-native';
import {TextInput, Text, Button, Portal, useTheme} from 'react-native-paper';
import formStyle from "@/style/formStyle";
import {FieldError} from "react-hook-form";
import {useDebounce} from "@/hooks/useDebounce";
import AppStyle from "@/constants/AppStyle";

export type SuggestionFetcher<T> = (query: string) => Promise<T[] | null>;
export type SuggestionRenderer<T> = (item: T) => string;

export interface SuggestiveInputProps<T, V> {
    value: V | null;
    onChange: (value: V | null) => void;

    label?: string;
    required?: boolean;
    placeholder?: string;
    error?: FieldError | undefined

    fetchSuggestions: SuggestionFetcher<T>;
    getOptionLabel: SuggestionRenderer<T>;
    getOptionValue: (item: T) => V;

    getLabelFromValue?: (value: V) => string;

    minLength?: number;

    labelAsTitle?: boolean;
}

export function ThemedSuggestiveInput<T, V>(
    {
        value,
        onChange,
        label,
        required,
        placeholder,
        error,
        fetchSuggestions,
        getOptionLabel,
        getOptionValue,
        getLabelFromValue,
        minLength = 3,
        labelAsTitle = false,
    }: SuggestiveInputProps<T, V>
) {
    const theme = useTheme();

    const [query, setQuery] = useState('');
    const [suggestions, setSuggestions] = useState<T[] | null>([]);
    const [open, setOpen] = useState(false);

    const [inputWidth, setInputWidth] = useState(0);
    const [position, setPosition] = useState({top: 0, left: 0});

    const containerRef = useRef<View>(null);

    // Hydrate default value
    useEffect(() => {
        if (value && getLabelFromValue) {
            setQuery(getLabelFromValue(value));
        }
    }, [value, getLabelFromValue]);

    useDebounce(() => {
        if (value) {
            setSuggestions([]);
            setOpen(false);
            return;
        }

        if (!query || query.length < minLength) {
            setSuggestions([]);
            setOpen(false);
            return;
        }

        fetchSuggestions(query).then((res) => {
            setSuggestions(res);
            setOpen(true);
        });
    }, 400, [query, value]);

    // Measure position
    const measure = () => {
        containerRef.current?.measureInWindow((x, y, width, height) => {
            setPosition({
                top: y + height,
                left: x,
            });
            setInputWidth(width);
        });
    };

    return (
        <View style={formStyle.intputContainer}>
            <Text
                testID='themedSuggestiveInputLabel'
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge, labelAsTitle ? AppStyle.textStyle.h3 : undefined]}
            >
                {label}
                {required && <Text style={{color: theme.colors.error}}> *</Text>}
            </Text>
            <View
                testID="themedSuggestiveInputContainer"
                ref={containerRef}
                onLayout={(e) => {
                    setInputWidth(e.nativeEvent.layout.width);
                    measure();
                }}
            >
                <TextInput
                    testID="themedSuggestiveTextInput"
                    value={query}
                    placeholder={placeholder}
                    onFocus={measure}
                    mode='outlined'
                    style={[formStyle.input,
                        error && {borderColor: theme.colors.error},
                        {
                            backgroundColor: theme.colors.background,
                            color: theme.colors.onBackground,
                        }
                    ]}
                    onChangeText={(text) => {
                        onChange(null);
                        setQuery(text);
                    }}
                />
                {error && (<Text style={{color: theme.colors.error}}>{error.message}</Text>)}
            </View>

            {open && suggestions && suggestions.length > 0 && (
                <Portal>
                    <View
                        style={{
                            position: 'absolute',
                            top: position.top,
                            left: position.left,
                            width: inputWidth,
                            backgroundColor: theme.colors.background,
                            borderWidth: 1,
                            borderColor: theme.colors.outline,
                            zIndex: 9999,
                            elevation: 10
                        }}
                    >
                        <FlatList
                            testID="themedSuggestiveInputSuggestionsList"
                            data={suggestions}
                            keyExtractor={(_, i) => i.toString()}
                            renderItem={({item}) => (
                                <Button
                                    testID="themedSuggestiveInputSuggestionButton"
                                    onPress={() => {
                                        onChange(getOptionValue(item));
                                        setQuery(getOptionLabel(item));
                                        setOpen(false);
                                    }}
                                >
                                    <Text>{getOptionLabel(item)}</Text>
                                </Button>
                            )}
                        />
                    </View>
                </Portal>
            )}
        </View>
    );
}