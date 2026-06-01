import {useState} from "react";
import PublicDeliveryCustomer from "@/deliveries/models/PublicDeliveryCustomer";

export default function usePublicNewDeliveryViewModel() {
    
    const [publicCustomer, setPublicCustomer] = useState<PublicDeliveryCustomer | null>(null);
    const [anonymous, setAnonymous] = useState<boolean>(false);
    
    const login = (customer?: PublicDeliveryCustomer) => {
        if (customer) {
            setPublicCustomer(customer);
            setAnonymous(false);
        }
        else {
            setPublicCustomer(null);
            setAnonymous(true);
        }
    }
    
    return {
        publicCustomer,
        anonymous,
        login
    }
}