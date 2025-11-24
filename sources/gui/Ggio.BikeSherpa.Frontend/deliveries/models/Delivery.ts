import { Identifiable } from "@/data/Identifiable";
import * as Crypto from 'expo-crypto';
import { Step } from "@/steps/models/Step";
import Customer from "@/customers/models/Customer";

export class Delivery implements Identifiable<string> {
    public readonly id: string;
    public code: string;
    public customer: Customer;
    public totalPrice: number;
    public discount: number;
    public reportId: string;
    public steps: Step[];

    public constructor(code: string, customer: Customer, totalPrice: number, discount: number, reportId: string, steps: Step[] = []) {
        this.id = Crypto.randomUUID();
        this.code = code;
        this.customer = customer;
        this.totalPrice = totalPrice;
        this.discount = discount;
        this.reportId = reportId;
        this.steps = steps;
    }
}