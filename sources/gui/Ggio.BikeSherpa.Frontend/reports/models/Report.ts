import { Delivery } from "@/deliveries/models/Delivery";

export default class Report {
    public id: string;
    public customer: string;
    public reportNumber: string;
    public reportDate: Date;
    public deliveries: Delivery[];

    constructor(id: string,
        customer: string,
        reportNumber: string,
        reportDate: Date,
        deliveries: Delivery[]) {
        this.id = id;
        this.customer = customer;
        this.reportNumber = reportNumber;
        this.reportDate = reportDate;
        this.deliveries = deliveries;
    }

    static fromPlainObject(obj: any, allDeliveries: Delivery[]): Report {
        const deliveries = obj.deliveryIds
            ? obj.deliveryIds
                .map((id: string) => allDeliveries.find(c => c.id === id))
                .filter((c: Delivery | undefined) => c !== undefined)
            : [];

        return new Report(
            obj.id,
            obj.customer,
            obj.reportNumber,
            obj.reportDate instanceof Date ? obj.reportDate : new Date(obj.reportDate),
            deliveries
        );
    }
}