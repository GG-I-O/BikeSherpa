import {inject, injectable} from "inversify";
import {IStepServices} from "@/steps/spi/IStepServices";
import {Observable} from "@legendapp/state";
import {ILogger} from "@/spi/LogsSPI";
import {IStorageContext} from "@/spi/StorageSPI";
import Delivery from "@/deliveries/models/Delivery";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import deliveryOperationAction from "@/steps/constants/deliveryOperationAction";
import {Step} from "@/steps/models/Step";
import {IDeliveryStorageMiddleware} from "@/deliveries/spi/IDeliveryStorageMiddleware";
import {hateoasRel} from "@/models/HateoasLink";

@injectable()
export default class StepServices implements IStepServices {
    private logger: ILogger;
    private readonly deliveryStorageMiddleware: IDeliveryStorageMiddleware
    private storage: IStorageContext<Delivery>;
    private readonly deliveryStore$: Observable<Record<string, Delivery>>;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(DeliveryServiceIdentifier.Storage) deliveryStorage: IStorageContext<Delivery>,
        @inject(DeliveryServiceIdentifier.StorageMiddleware) deliveryStorageMiddleware: IDeliveryStorageMiddleware
    ) {
        this.logger = logger;
        this.logger = this.logger.extend("StepServices");
        this.deliveryStorageMiddleware = deliveryStorageMiddleware;
        this.storage = deliveryStorage;
        this.deliveryStore$ = this.storage.getStore();
    }

    public updateTime(stepId: string, hours: number, minutes: number): void {
        const observables = this.getDeliveryFromStep(stepId);
        if (!observables.delivery$) {
            this.logger.error(`UpdateStepTime : Parent delivery not found for step ${stepId}`);
            return;
        }
        if (!observables.step$) {
            this.logger.error(`UpdateStepTime : Step ${stepId} not found in delivery ${observables.delivery$.peek().id}`);
            return;
        }

        // Test if the user got the rights to do this action
        const step = observables.step$.get();
        if (!step.links || !step.links.some((link) => link.rel === hateoasRel.patch)) {
            this.logger.error(`Cannot patch ${stepId}`);
            return
        }

        const date = new Date(observables.step$.estimatedDeliveryDate.peek());
        date.setHours(hours, minutes);

        this.deliveryStorageMiddleware.addUpdateStepState(
            observables.delivery$.peek().id,
            observables.step$.peek().id,
            deliveryOperationAction.patchTime
        );

        observables.step$!.estimatedDeliveryDate.set(date.toISOString());
    }

    public reorderStep(stepId: string, newOrder: number): void {
        const observables = this.getDeliveryFromStep(stepId);
        if (!observables.delivery$) {
            this.logger.error(`ReorderStep : Parent delivery not found for step ${stepId}`);
            return;
        }
        if (!observables.step$) {
            this.logger.error(`ReorderStep: Step ${stepId} not found in delivery ${observables.delivery$.peek().id}`);
            return;
        }

        // Test if the user got the rights to do this action
        const step = observables.step$.get();
        if (!step.links || !step.links.some((link) => link.rel === hateoasRel.patch)) {
            this.logger.error(`Cannot patch ${stepId}`);
            return
        }

        this.deliveryStorageMiddleware.addUpdateStepState(
            observables.delivery$.peek().id,
            observables.step$.peek().id,
            deliveryOperationAction.patchOrder
        );

        observables.step$!.order.set(newOrder);
    }

    private getDeliveryFromStep(stepId: string): {
        delivery$: Observable<Delivery> | undefined,
        step$: Observable<Step> | undefined
    } {
        let observables: {
            delivery$: Observable<Delivery> | undefined,
            step$: Observable<Step> | undefined
        } = {
            delivery$: undefined,
            step$: undefined
        }

        const deliveryList = Object.values(this.deliveryStore$.get());
        const parentDelivery = deliveryList.find(
            (delivery) => delivery.steps.some((step) => step.id === stepId)
        );
        if (!parentDelivery)
            return observables;
        observables.delivery$ = this.deliveryStore$[parentDelivery.id];

        const index = observables.delivery$.steps.peek().findIndex(s => s.id === stepId);
        if (index === -1)
            return observables;
        observables.step$ = observables.delivery$.steps[index];

        return observables;
    }
    
    public assignCourier(stepId: string, courierId: string) {
        const observables = this.getDeliveryFromStep(stepId);
        if (!observables.delivery$) {
            this.logger.error(`AssignCourier : Parent delivery not found for step ${stepId}`);
            return;
        }
        if (!observables.step$) {
            this.logger.error(`AssignCourier: Step ${stepId} not found in delivery ${observables.delivery$.peek().id}`);
            return;
        }

        // Test if the user got the rights to do this action
        const step = observables.step$.get();
        if (!step.links || !step.links.some((link) => link.rel === hateoasRel.stepCourier.post)) {
            this.logger.error(`Cannot post courier for step ${stepId}`);
            return
        }

        this.deliveryStorageMiddleware.addUpdateStepState(
            observables.delivery$.peek().id,
            observables.step$.peek().id,
            deliveryOperationAction.postCourier
        );

        observables.step$!.courierId.set(courierId);
    }
    
    public unassignCourier(stepId: string) {
        const observables = this.getDeliveryFromStep(stepId);
        if (!observables.delivery$) {
            this.logger.error(`UnassignCourier : Parent delivery not found for step ${stepId}`);
            return;
        }
        if (!observables.step$) {
            this.logger.error(`UnassignCourier: Step ${stepId} not found in delivery ${observables.delivery$.peek().id}`);
            return;
        }

        // Test if the user got the rights to do this action
        const step = observables.step$.get();
        if (!step.links || !step.links.some((link) => link.rel === hateoasRel.stepCourier.delete)) {
            this.logger.error(`Cannot delete courier for step ${stepId}`);
            return
        }

        this.deliveryStorageMiddleware.addUpdateStepState(
            observables.delivery$.peek().id,
            observables.step$.peek().id,
            deliveryOperationAction.deleteCourier
        );

        observables.step$!.courierId.set(null);
    }
}