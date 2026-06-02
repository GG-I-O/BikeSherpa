import {Text} from "react-native-paper";
import PublicDeliveryStepForm from "@/steps/components/PublicDeliveryStepForm";
import PublicDeliveryDetailsForm from "@/deliveries/components/PublicDeliveryDetailsForm";
import PublicDeliveryStepDetailsForm from "@/steps/components/PublicDeliveryStepDetailsForm";
import PublicDeliveryPrice from "@/deliveries/components/PublicDeliveryPrice";
import PublicDeliveryCustomerForm from "@/deliveries/components/PublicDeliveryCustomerForm";
import usePublicDeliveryFormViewModel from "@/deliveries/viewModel/usePublicDeliveryFormViewModel";

export default function PublicDeliveryAnonymousForm() {

    const viewModel = usePublicDeliveryFormViewModel();

    return (
        <>
            <PublicDeliveryStepForm
                control={viewModel.control}
                totalDistance={0}
            />
            <PublicDeliveryDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                urgencies={viewModel.urgencies}
                packingSizes={viewModel.packingSizes}
            />
            <PublicDeliveryPrice
                price={0}
                priceWithTaxes={0}
            />
            <PublicDeliveryCustomerForm
                control={viewModel.control}
                errors={viewModel.errors}
                customerType={viewModel.customerType}
            />
            <PublicDeliveryStepDetailsForm
                control={viewModel.control}
                errors={viewModel.errors}
                customerType={viewModel.customerType}
                setCustomerType={viewModel.setCustomerType}
            />
        </>
    );
}