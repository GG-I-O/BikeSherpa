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
}