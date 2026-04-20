import React, {useEffect, useRef, useState} from 'react';
import {View, FlatList} from 'react-native';
import {TextInput, Text, Button, Portal, useTheme} from 'react-native-paper';
import formStyle from "@/style/formStyle";
import {FieldError} from "react-hook-form";

export type SuggestionFetcher<T> = (query: string) => Promise<T[] | null>;
export type SuggestionRenderer<T> = (item: T) => string;

export interface SuggestiveInputProps<T> {
    value: T | null;
    onChange: (value: T | null) => void;
    label?: string;
    required?: boolean;
    placeholder?: string;
    error?: FieldError | undefined
    fetchSuggestions: SuggestionFetcher<T>;
    renderLabel: SuggestionRenderer<T>;
    minLength?: number;
}

export function ThemedSuggestiveInput<T>(
    {
        value,
        onChange,
        label,
        required,
        placeholder,
        error,
        fetchSuggestions,
        renderLabel,
        minLength = 3
    }: SuggestiveInputProps<T>
) {
    const theme = useTheme();

    const [query, setQuery] = useState('');
    const [debouncedQuery, setDebouncedQuery] = useState('');
    const [suggestions, setSuggestions] = useState<T[] | null>([]);
    const [open, setOpen] = useState(false);

    const [inputWidth, setInputWidth] = useState(0);
    const [position, setPosition] = useState({top: 0, left: 0});

    const containerRef = useRef<View>(null);
    const timerRef = useRef<number | null>(null);

    // Debounce
    const updateQuery = (text: string) => {
        setQuery(text);

        if (timerRef.current) clearTimeout(timerRef.current);

        timerRef.current = setTimeout(() => {
            setDebouncedQuery(text);
        }, 400);
    };

    // Fetch suggestions
    useEffect(() => {
        if (!debouncedQuery || debouncedQuery.length < minLength) {
            setSuggestions([]);
            setOpen(false);
            return;
        }

        fetchSuggestions(debouncedQuery).then((res) => {
            setSuggestions(res);
            setOpen(true);
        });
    }, [debouncedQuery, fetchSuggestions, minLength]);

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
                style={[formStyle.label, {color: theme.colors.onBackground}, theme.fonts.labelLarge]}>
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
                    value={value ? renderLabel(value) : query}
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
                        updateQuery(text);
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
                                        onChange(item);
                                        setOpen(false);
                                    }}
                                >
                                    <Text>{renderLabel(item)}</Text>
                                </Button>
                            )}
                        />
                    </View>
                </Portal>
            )}
        </View>
    );
}