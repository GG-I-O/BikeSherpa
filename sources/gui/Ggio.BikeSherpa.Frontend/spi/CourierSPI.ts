import Courier from "@/couriers/models/Courier";
import InputCourier from "@/couriers/models/InputCourier";
import { Observable } from "@legendapp/state";

export interface ICourierService {
    getCourierList$(): Observable<Record<string, Courier>>;
    getCourier$(courierId: string): Observable<Courier>;
    getCourier(courierId: string): Courier;
    createCourier(courier: InputCourier): void;
    updateCourier(courier: Courier): void;
    deleteCourier(courierId: string): void;
}