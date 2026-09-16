import {observable} from "@legendapp/state";

type StepDatePickerStore = {
    date: Date|undefined;
};

export const stepDatePickerStore$ = observable<StepDatePickerStore>({
    date: new Date()
});