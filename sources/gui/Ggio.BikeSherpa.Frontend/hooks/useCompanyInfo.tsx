import {IOCContainer} from "@/bootstrapper/constants/IOCContainer";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {ICompanyInfoService} from "@/spi/ICompanyInfoService";
import {useEffect, useState} from "react";

export default function useCompanyInfo() {
    const companyInfoService = IOCContainer.get<ICompanyInfoService>(ServicesIdentifiers.CompanyInfoService);

    const [label, setLabel] = useState<string|undefined>(undefined);
    const [phone, setPhone] = useState<string|undefined>(undefined);
    
    useEffect(() => {
        if (!label)
            companyInfoService.getCompanyLabel().then(label => setLabel(label));
    },[companyInfoService, setLabel]);

    useEffect(() => {
        if (!phone)
            companyInfoService.getCompanyPhone().then(phone => setPhone(phone));
    },[companyInfoService, setPhone]);
    
    return {
        label,
        phone
    }
}