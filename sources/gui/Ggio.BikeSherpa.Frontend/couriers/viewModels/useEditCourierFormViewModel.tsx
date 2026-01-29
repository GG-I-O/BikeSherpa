import { useForm } from "react-hook-form";
import Courier from "../models/Courier";
import { zodResolver } from "@hookform/resolvers/zod";
import { IOCContainer } from "@/bootstrapper/constants/IOCContainer";
import { ICourierService } from "@/spi/CourierSPI";
import { ServicesIdentifiers } from "@/bootstrapper/constants/ServicesIdentifiers";
import { useEffect, useState } from "react";
import { observe } from "@legendapp/state";
import EditCourierFormViewModel from "./EditCourierFormViewModel";

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
    } = useForm<Courier>({
        defaultValues: courier,
        resolver: zodResolver(editCourierFormViewModel.getEditCourierSchema(courier, courierList))
    });

    return { control, handleSubmit: handleSubmit(editCourierFormViewModel.onSubmit), errors, setValue };
}