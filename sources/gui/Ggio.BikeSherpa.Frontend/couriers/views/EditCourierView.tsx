import CourierForm from "../components/CourierForm";
import { useEditCourierFormViewModel } from "../viewModels/useEditCourierFormViewModel";
import { useLocalSearchParams } from "expo-router";
import {CourierFormValues} from "@/couriers/viewModels/zod/courierFormBaseSchema";

export default function EditCourierView() {
    const { courierId } = useLocalSearchParams<{ courierId: string }>();
    const { control, errors, handleSubmit } = useEditCourierFormViewModel(courierId);

    return (
        <CourierForm<CourierFormValues>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Modifier le livreur" />
    );
}