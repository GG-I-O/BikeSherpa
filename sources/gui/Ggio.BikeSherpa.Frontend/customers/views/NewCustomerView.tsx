import formStyle from "@/style/formStyle";
import { useNewCustomerForm } from "../hooks/useNewCustomerForm";

import CustomerForm from "../components/CustomerForm";
import InputCustomer from "../models/InputCustomer";

export default function NewCustomerView() {
    const { control, errors, handleSubmit } = useNewCustomerForm();

    return (
        <CustomerForm<InputCustomer>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Ajouter le client" />
    );
}