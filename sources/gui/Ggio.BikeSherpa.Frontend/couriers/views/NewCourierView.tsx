import { useNewCourierFormViewModel } from "../viewModels/useNewCourierFormViewModel";
import CourierForm from "../components/CourierForm";
import {CourierFormValues} from "@/couriers/viewModels/zod/courierFormBaseSchema";

export default function NewCourierView() {
    const { control, errors, handleSubmit } = useNewCourierFormViewModel();

    return (
        <CourierForm<CourierFormValues>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Ajouter le livreur" />
    );
}