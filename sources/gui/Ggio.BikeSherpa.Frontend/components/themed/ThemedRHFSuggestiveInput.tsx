import { Control, useController } from 'react-hook-form';
import {SuggestiveInputProps, ThemedSuggestiveInput} from "@/components/themed/ThemedSuggestiveInput";

interface RHFSuggestiveInputProps<T, V> extends Omit<SuggestiveInputProps<T, V>, 'value' | 'onChange'> {
    name: string;
    control: Control<any>;
}

export function ThemedRHFSuggestiveInput<T, V>({ name, control, ...props }: RHFSuggestiveInputProps<T, V>) {
    const { field } = useController({ name, control });

    return (
        <ThemedSuggestiveInput
            {...props}
            value={field.value}
            onChange={field.onChange}
        />
    );
}