import { Control, useController } from 'react-hook-form';
import {SuggestiveInputProps, ThemedSuggestiveInput} from "@/components/themed/ThemedSuggestiveInput";

interface RHFSuggestiveInputProps<T> extends Omit<SuggestiveInputProps<T>, 'value' | 'onChange'> {
    name: string;
    control: Control<any>;
}

export function ThemedRHFSuggestiveInput<T>({ name, control, ...props }: RHFSuggestiveInputProps<T>) {
    const { field } = useController({ name, control });

    return (
        <ThemedSuggestiveInput
            {...props}
            value={field.value}
            onChange={field.onChange}
        />
    );
}