import {Control, FieldValues, Path, useController} from 'react-hook-form';
import {SuggestiveInputProps, ThemedSuggestiveInput} from "@/components/themed/ThemedSuggestiveInput";

interface RHFSuggestiveInputProps<TForm extends FieldValues, T, V> extends Omit<SuggestiveInputProps<T, V>, 'value' | 'onChange'> {
    name: string;
    control: Control<TForm>;
}

export function ThemedRHFSuggestiveInput<TForm extends FieldValues = FieldValues, T = any, V = any>(
    { name, control, ...props }: RHFSuggestiveInputProps<TForm, T, V>) {
    const { field } = useController(
        { 
            control,
            name: name as Path<TForm>
        });

    return (
        <ThemedSuggestiveInput
            {...props}
            value={field.value}
            onChange={field.onChange}
        />
    );
}