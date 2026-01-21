import { useNewCustomerFormViewModel } from "../viewModels/useNewCustomerFormViewModel";

import CustomerForm from "../components/CustomerForm";
import InputCustomer from "../models/InputCustomer";

export default function NewCustomerView() {
    const { control, errors, handleSubmit } = useNewCustomerFormViewModel();

    return (
        <CustomerForm<InputCustomer>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Ajouter le client" />
    );
}