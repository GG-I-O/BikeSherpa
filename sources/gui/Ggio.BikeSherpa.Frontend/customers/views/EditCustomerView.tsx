import CustomerForm from "../components/CustomerForm";
import Customer from "../models/Customer";
import { useEditCustomerFormViewModel } from "../viewModels/useEditCustomerFormViewModel";
import { useLocalSearchParams } from "expo-router";

export default function EditCustomerView() {
    const { customerId } = useLocalSearchParams<{ customerId: string }>();
    const { control, errors, handleSubmit } = useEditCustomerFormViewModel(customerId);

    return (
        <CustomerForm<Customer>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Modifier le client" />
    );
}