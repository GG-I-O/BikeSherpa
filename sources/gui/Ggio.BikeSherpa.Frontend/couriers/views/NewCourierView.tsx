import { useNewCourierFormViewModel } from "../viewModels/useNewCourierFormViewModel";

import CourierForm from "../components/CourierForm";
import InputCourier from "../models/InputCourier";

export default function NewCourierView() {
    const { control, errors, handleSubmit } = useNewCourierFormViewModel();

    return (
        <CourierForm<InputCourier>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Ajouter le livreur" />
    );
}