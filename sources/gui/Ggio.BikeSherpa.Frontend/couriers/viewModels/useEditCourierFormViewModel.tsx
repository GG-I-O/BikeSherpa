import { useForm } from "react-hook-form";
import Courier from "../models/Courier";
import { zodResolver } from "@hookform/resolvers/zod";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ICourierService } from "@/spi/CourierSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { useEffect, useState } from "react";
import { observe } from "@legendapp/state";
import EditCourierFormViewModel from "./EditCourierFormViewModel";
import {CourierFormValues} from "@/couriers/viewModels/zod/courierFormBaseSchema";

export function useEditCourierFormViewModel(courierId: string) {
    const courierServices = IOCContainer.get<ICourierService>(ServicesIdentifiers.CourierServices);
    const courier = courierServices.getCourier$(courierId).peek();
    const editCourierFormViewModel = new EditCourierFormViewModel(courierServices);

    const courierStore$ = courierServices.getCourierList$();
    const [courierList, setCourierList] = useState<Courier[]>([]);

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
        setValue
    } = useForm<CourierFormValues>({
        defaultValues: courier,
        resolver: zodResolver(editCourierFormViewModel.getEditCourierSchema(courier, courierList))
    });

    return { 
        control,
        handleSubmit: handleSubmit(
            (data) => {
                editCourierFormViewModel.onSubmit(courier, data);
            },
            (errors) => {
                console.error("Invalid delivery for creation");
                console.error(errors);
            }
        ),
        errors, 
        setValue 
    };
}