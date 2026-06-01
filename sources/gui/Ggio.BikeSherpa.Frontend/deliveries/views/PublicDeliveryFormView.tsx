import PublicDeliveryAnonymousForm from "@/deliveries/components/PublicDeliveryAnonymousForm";
import PublicDeliveryAuthenticatedForm from "@/deliveries/components/PublicDeliveryAuthenticatedForm";
import {publicDeliveryStore$} from "@/deliveries/store/publicDeliveryStore";

export default function PublicDeliveryFormView() {
    const customer = publicDeliveryStore$.customer.get();
    const isAnonymous = publicDeliveryStore$.isAnonymous.get();

    if (customer && !isAnonymous)
        return <PublicDeliveryAuthenticatedForm customer={customer}/>

    return <PublicDeliveryAnonymousForm/>
}
