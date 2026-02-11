import CourierForm from "../components/CourierForm";
import Courier from "../models/Courier";
import { useEditCourierFormViewModel } from "../viewModels/useEditCourierFormViewModel";
import { useLocalSearchParams } from "expo-router";

export default function EditCourierView() {
    const { courierId } = useLocalSearchParams<{ courierId: string }>();
    const { control, errors, handleSubmit } = useEditCourierFormViewModel(courierId);

    return (
        <CourierForm<Courier>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Modifier le client" />
    );
}