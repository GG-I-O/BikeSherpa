import {inject, injectable} from "inversify";
import {IDeliveryStorageMiddleware} from "@/deliveries/spi/IDeliveryStorageMiddleware";
import Delivery from "../models/Delivery";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import deliveryOperationAction from "@/steps/constants/deliveryOperationAction";
import {IDeliveryCustomBackendClientFacade} from "@/deliveries/spi/IDeliveryCustomBackendClientFacade";
import {IBackendClient} from "@/spi/BackendClientSPI";
import JsonPatchDocument from "@/models/JsonPatchDocument";

@injectable()
export default class DeliveryStorageMiddleware implements IDeliveryStorageMiddleware {
    private backendClientFacade: IBackendClient<Delivery>;
    private customClientFacade: IDeliveryCustomBackendClientFacade;
    private updateStepState: { deliveryId: string, stepId: string, state: string }[] = [];

    constructor(
        @inject(DeliveryServiceIdentifier.BackendClientFacade) backendClientFacade: IBackendClient<Delivery>,
        @inject(DeliveryServiceIdentifier.CustomBackendClientFacade) customClientFacade: IDeliveryCustomBackendClientFacade
    ) {
        this.backendClientFacade = backendClientFacade;
        this.customClientFacade = customClientFacade;
    }

    public addUpdateStepState(deliveryId: string, stepId: string, state: string): void {
        this.updateStepState.push({deliveryId, stepId, state});
    }

    public async update(delivery: Delivery): Promise<void> {
        let completeUpdate: boolean = true;
        for (let i = 0; i < this.updateStepState.length; i++) {
            if (this.updateStepState[i].deliveryId !== delivery.id)
                continue;
            
            const step = delivery.steps.find(step => step.id === this.updateStepState[i].stepId)
            if (!step)
                throw new Error(`Step with ID ${this.updateStepState[i].stepId} not found in delivery ${delivery.id}`);

            switch (this.updateStepState[i].state) {
                case deliveryOperationAction.patchTime:
                    let patchTimeJson = new JsonPatchDocument();
                    patchTimeJson.addOperation(
                        "/estimatedDeliveryDate",
                        "replace",
                        new Date(step.estimatedDeliveryDate).toISOString()
                    );
                    await this.customClientFacade.PatchStepEndpoint(step, patchTimeJson);
                    break;
                case deliveryOperationAction.patchOrder:
                    let patchOrderJson = new JsonPatchDocument();
                    patchOrderJson.addOperation(
                        "/order",
                        "replace",
                        step.order
                    );
                    await this.customClientFacade.PatchStepEndpoint(step, patchOrderJson);
                    break;
                case deliveryOperationAction.postCourier:
                    await this.customClientFacade.PostStepCourierEndpoint(step);
                    break;
                case deliveryOperationAction.deleteCourier:
                    await this.customClientFacade.DeleteStepCourierEndpoint(step);
                    break;
                default:
                    throw new Error(`Unsupported update action: ${this.updateStepState[i].state}`);
            }
        }

        if (completeUpdate)
            await this.backendClientFacade.UpdateEndpoint(delivery);
        
        this.updateStepState = this.updateStepState.filter(state => state.deliveryId === delivery.id);
    }

}