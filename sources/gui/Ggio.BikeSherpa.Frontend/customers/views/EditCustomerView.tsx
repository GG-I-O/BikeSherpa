import CustomerForm from "../components/CustomerForm";
import { useEditCustomerFormViewModel } from "../viewModels/useEditCustomerFormViewModel";
import { useLocalSearchParams } from "expo-router";
import {CustomerFormValues} from "@/customers/viewModels/zod/customerFormBaseSchema";

export default function EditCustomerView() {
    const { customerId } = useLocalSearchParams<{ customerId: string }>();
    const { control, errors, handleSubmit } = useEditCustomerFormViewModel(customerId);

    return (
        <CustomerForm<CustomerFormValues>
            control={control}
            errors={errors}
            handleSubmit={handleSubmit}
            buttonName="Modifier le client" />
    );
}