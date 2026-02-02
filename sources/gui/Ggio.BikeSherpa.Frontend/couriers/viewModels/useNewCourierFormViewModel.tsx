import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import InputCourier from "../models/InputCourier";
import { ICourierService } from "@/spi/CourierSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { useEffect, useState } from "react";
import Courier from "../models/Courier";
import { observe } from "@legendapp/state";
import NewCourierFormViewModel from "./NewCourierFormViewModel";

export function useNewCourierFormViewModel() {
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const courierStore$ = courierServices.getCourierList$();
    const [courierList, setCourierList] = useState<Courier[]>([]);
    const newCourierViewModel = new NewCourierFormViewModel(courierServices);

    useEffect(() => {
        return observe(() => {
            const record = courierStore$.get() ?? {};
            setCourierList(Object.values(record));
        });
    }, [courierStore$]);

    const {
        control,
        handleSubmit,
        formState: { errors },
        reset
    } = useForm<InputCourier>({
        defaultValues: {
            firstName: "",
            lastName: "",
            code: "",
            phoneNumber: "",
            email: "",
            address: {
                name: "",
                streetInfo: "",
                complement: "",
                postcode: "",
                city: ""
            },
        },
        resolver: zodResolver(newCourierViewModel.getNewCourierSchema(courierList))
    });

    newCourierViewModel.setResetCallback(reset);

    return {
        control,
        handleSubmit: handleSubmit(newCourierViewModel.onSubmit),
        errors
    };
}