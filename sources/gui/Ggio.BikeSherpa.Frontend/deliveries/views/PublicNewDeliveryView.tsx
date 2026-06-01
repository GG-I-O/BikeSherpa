import React from "react";
import usePublicNewDeliveryViewModel from "@/deliveries/viewModel/usePublicNewDeliveryViewModel";
import PublicDeliveryLoginForm from "@/deliveries/components/PublicDeliveryLoginForm";
import PublicDeliveryForm from "@/deliveries/components/PublicDeliveryForm";

export default function PublicNewDeliveryView() {    
    const viewModel = usePublicNewDeliveryViewModel();
    
    if (!viewModel.publicCustomer && !viewModel.anonymous)
        return (
            <PublicDeliveryLoginForm login={viewModel.login} />
        );
    
    console.log(viewModel.publicCustomer);
    return (
      <PublicDeliveryForm />
    );
}