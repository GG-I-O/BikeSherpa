import { useNewCustomerFormViewModel } from "../viewModels/useNewCustomerFormViewModel";
import CustomerForm from "../components/CustomerForm";
import {CustomerFormValues} from "@/customers/viewModels/zod/customerFormBaseSchema";

export default function NewCustomerView() {
    const { control, errors, handleSubmit } = useNewCustomerFormViewModel();

    return (
        <CustomerForm<CustomerFormValues>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Ajouter le client" />
    );
}