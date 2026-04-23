import {inject, injectable} from "inversify";
import {IStepServices} from "@/steps/spi/IStepServices";
import {batch, Observable} from "@legendapp/state";
import {ILogger} from "@/spi/LogsSPI";
import {IStorageContext} from "@/spi/StorageSPI";
import Delivery from "@/deliveries/models/Delivery";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import {hateoasRel} from "@/models/HateoasLink";
import deliveryOperationAction from "@/steps/constants/deliveryOperationAction";

@injectable()
export default class StepServices implements IStepServices {
    private logger: ILogger;
    private storage: IStorageContext<Delivery>;
    private readonly deliveryStore$: Observable<Record<string, Delivery>>;

    public constructor(
        @inject(ServicesIdentifiers.Logger) logger: ILogger,
        @inject(DeliveryServiceIdentifier.Storage) deliveryStorage: IStorageContext<Delivery>
    ) {
        this.logger = logger;
        this.logger = this.logger.extend("StepServices");
        this.storage = deliveryStorage;
        this.deliveryStore$ = this.storage.getStore();
    }

    public updateTime(stepId: string, hours: number, minutes: number): void {
        const deliveryList = Object.values(this.deliveryStore$.get());
        const parentDelivery = deliveryList.find(
            (delivery) => delivery.steps.some((step) => step.id === stepId)
        );

        if (!parentDelivery) {
            this.logger.error(`Parent delivery not found for step ${stepId}`);
            return;
        }

        const delivery$ = this.deliveryStore$[parentDelivery.id];

        const index = delivery$.steps.peek().findIndex(s => s.id === stepId);
        if (index === -1) {
            this.logger.error(`Step ${stepId} not found in delivery ${parentDelivery.id}`);
            return;
        }

        const step$ = delivery$.steps[index];
        
        // Test if the user got the rights to do this action
        const step = step$.get();
        if (!step.links || !step.links.some((link) => link.rel == deliveryOperationAction.patchTime)){
            this.logger.error(`Cannot patch ${stepId}`);
            return
        }
        
        const date = new Date(step$.estimatedDeliveryDate.peek());
        date.setHours(hours, minutes);

        batch(() => {
            step$.estimatedDeliveryDate.set(date.toISOString());
            step$.operationAction.set(deliveryOperationAction.patchTime);
        });
    }
}