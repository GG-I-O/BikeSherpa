import { Identifiable } from "@/models/Identifiable";
import * as Crypto from 'expo-crypto';
import { Step } from "@/steps/models/Step";
import Customer from "@/customers/models/Customer";
import DeliveryDetail from "./DeliveryDetail";
import { DeliveryPacking } from "./DeliveryPacking";

export class Delivery implements Identifiable<string> {
    public readonly id: string;
    public code: string;
    public customer: Customer;
    public totalPrice: number;
    public reportId: string;
    public steps: Step[];
    public details: DeliveryDetail[];
    public packing: DeliveryPacking;

    public constructor(code: string, customer: Customer, totalPrice: number, reportId: string, packing: DeliveryPacking, steps: Step[] = [], details: DeliveryDetail[] = []) {
        this.id = Crypto.randomUUID();
        this.code = code;
        this.customer = customer;
        this.totalPrice = totalPrice;
        this.reportId = reportId;
        this.packing = packing;
        this.steps = steps;
        this.details = details;
    }
}