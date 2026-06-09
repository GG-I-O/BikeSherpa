import {inject, injectable} from "inversify";
import {IDeliveryStorageMiddleware} from "@/deliveries/spi/IDeliveryStorageMiddleware";
import Delivery from "../models/Delivery";
import {DeliveryServiceIdentifier} from "@/deliveries/bootstrapper/DeliveryServiceIdentifier";
import deliveryStepOperationAction from "@/steps/data/deliveryStepOperationAction";
import {IDeliveryCustomBackendClientFacade} from "@/deliveries/spi/IDeliveryCustomBackendClientFacade";
import {IBackendClient} from "@/spi/BackendClientSPI";
import JsonPatchDocument from "@/models/JsonPatchDocument";
import UploadableFile from "@/models/UploadableFile";
import deliveryOperationAction from "@/deliveries/data/deliveryOperationAction";

@injectable()
export default class DeliveryStorageMiddleware implements IDeliveryStorageMiddleware {
    private backendClientFacade: IBackendClient<Delivery>;
    private customClientFacade: IDeliveryCustomBackendClientFacade;

    private getAllDailyDeliveriesDate: string | null = null;
    private updateDeliveryState: { deliveryId: string, state: string }[] = [];
    private updateStepState: { deliveryId: string, stepId: string, state: string }[] = [];
    private attachmentUploadQueue: { stepId: string, file: UploadableFile }[] = [];

    constructor(
        @inject(DeliveryServiceIdentifier.BackendClientFacade) backendClientFacade: IBackendClient<Delivery>,
        @inject(DeliveryServiceIdentifier.CustomBackendClientFacade) customClientFacade: IDeliveryCustomBackendClientFacade
    ) {
        this.backendClientFacade = backendClientFacade;
        this.customClientFacade = customClientFacade;
    }

    public setGetAllDateForDailyDeliveries(date: string | null) {
        this.getAllDailyDeliveriesDate = date;
    }

    public async getAll(date?: string): Promise<Delivery[]> {
        if (!this.getAllDailyDeliveriesDate)
            return await this.backendClientFacade.GetAllEndpoint(date);
        else
            return await this.customClientFacade.GetAllDailyDeliveriesEndpoint(this.getAllDailyDeliveriesDate);
    }

    public addUpdateStepState(deliveryId: string, stepId: string, state: string): void {
        this.updateStepState.push({deliveryId, stepId, state});
    }

    public addUpdateDeliveryState(deliveryId: string, state: string) {
        this.updateDeliveryState.push({deliveryId, state});
    }

    public addAttachmentToUploadQueue(stepId: string, file: UploadableFile) {
        this.attachmentUploadQueue.push({stepId, file});
    }

    public async update(delivery: Delivery): Promise<void> {
        let completeUpdate: boolean = true;
        
        // Call an endpoint for every update to do on step in updateStepState
        for (let i = 0; i < this.updateStepState.length; i++) {
            if (this.updateStepState[i].deliveryId !== delivery.id)
                continue;

            const step = delivery.steps.find(step => step.id === this.updateStepState[i].stepId)
            if (!step)
                throw new Error(`Step with ID ${this.updateStepState[i].stepId} not found in delivery ${delivery.id}`);

            completeUpdate = false;
            switch (this.updateStepState[i].state) {
                case deliveryStepOperationAction.patchTime:
                    let patchTimeJson = new JsonPatchDocument();
                    patchTimeJson.addOperation(
                        "/estimatedDeliveryDate",
                        "replace",
                        new Date(step.estimatedDeliveryDate).toISOString()
                    );
                    await this.customClientFacade.PatchStepEndpoint(step, patchTimeJson);
                    break;
                case deliveryStepOperationAction.patchOrder:
                    let patchOrderJson = new JsonPatchDocument();
                    patchOrderJson.addOperation(
                        "/order",
                        "replace",
                        step.order
                    );
                    await this.customClientFacade.PatchStepEndpoint(step, patchOrderJson);
                    break;
                case deliveryStepOperationAction.patchComment:
                    let patchCommentJson = new JsonPatchDocument();
                    patchCommentJson.addOperation(
                        "/comment",
                        "replace",
                        step.comment
                    );
                    await this.customClientFacade.PatchStepEndpoint(step, patchCommentJson);
                    break;
                case deliveryStepOperationAction.patchCourierComment:
                    let patchCourierCommentJson = new JsonPatchDocument();
                    patchCourierCommentJson.addOperation(
                        "/courierComment",
                        "replace",
                        step.courierComment
                    );
                    await this.customClientFacade.PatchStepEndpoint(step, patchCourierCommentJson);
                    break;
                case deliveryStepOperationAction.postCourier:
                    await this.customClientFacade.PostStepCourierEndpoint(step);
                    break;
                case deliveryStepOperationAction.deleteCourier:
                    await this.customClientFacade.DeleteStepCourierEndpoint(step);
                    break;
                case deliveryStepOperationAction.putOrder:
                    await this.customClientFacade.PutStepOrderEndpoint(step, step.order >= 0 ? 1 : -1);
                    break;
                case deliveryStepOperationAction.putTime:
                    await this.customClientFacade.PutStepTimeEndpoint(step);
                    break;
                case deliveryStepOperationAction.putComplete:
                    await this.customClientFacade.PutStepCompletionEndpoint(step);
                    break;
                case deliveryStepOperationAction.postAttachment:
                    for (let i = 0; i < this.attachmentUploadQueue.length; i++) {
                        if (this.attachmentUploadQueue[i].stepId === step.id)
                            await this.customClientFacade.PostAttachmentEndpoint(step, this.attachmentUploadQueue[i].file);
                    }
                    this.attachmentUploadQueue = this.attachmentUploadQueue.filter(file => file.stepId !== step.id);
                    break;
                default:
                    throw new Error(`Unsupported update action: ${this.updateStepState[i].state}`);
            }
        }

        // Call an endpoint for every update to do on delivery in updateDeliveryState
        for (let i = 0; i < this.updateDeliveryState.length; i++) {
            if (this.updateDeliveryState[i].deliveryId !== delivery.id)
                continue;

            completeUpdate = false;
            switch (this.updateDeliveryState[i].state) {
                case deliveryOperationAction.putPending:
                    await this.customClientFacade.PutDeliveryPendingEndpoint(delivery);
                    break;
                case deliveryOperationAction.putRenew:
                    await this.customClientFacade.PutDeliveryRenewEndpoint(delivery);
                    break;
                default:
                    throw new Error(`Unsupported update action: ${this.updateStepState[i].state}`);
            }
        }

        // If no update found, it means we need a complete update
        if (completeUpdate)
            await this.backendClientFacade.UpdateEndpoint(delivery);

        // Clear state already processed
        this.updateStepState = this.updateStepState.filter(state => state.deliveryId !== delivery.id);
        this.updateDeliveryState = this.updateDeliveryState.filter(state => state.deliveryId !== delivery.id);
    }

}