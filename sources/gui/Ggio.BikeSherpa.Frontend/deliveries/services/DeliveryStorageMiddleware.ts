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
import {ILogger} from "@/spi/LogsSPI";
import {ServicesIdentifiers} from "@/bootstrapper/constants/ServicesIdentifiers";

@injectable()
export default class DeliveryStorageMiddleware implements IDeliveryStorageMiddleware {
    private backendClientFacade: IBackendClient<Delivery>;
    private customClientFacade: IDeliveryCustomBackendClientFacade;

    private dateForGetAllMyDeliveries: string | null = null;
    private dateForGetAllUnassignedDeliveries: string | null = null;
    private updateDeliveryState: { deliveryId: string, state: string }[] = [];
    private updateStepState: { deliveryId: string, stepId: string, state: string }[] = [];
    private attachmentUploadQueue: { stepId: string, file: UploadableFile }[] = [];
    private signatureUploadQueue: { stepId: string, signature: UploadableFile, receiver: string }[] = [];

    private readonly logger: ILogger;
    
    constructor(
        @inject(DeliveryServiceIdentifier.BackendClientFacade) backendClientFacade: IBackendClient<Delivery>,
        @inject(DeliveryServiceIdentifier.CustomBackendClientFacade) customClientFacade: IDeliveryCustomBackendClientFacade,
        @inject(ServicesIdentifiers.Logger) logger: ILogger
    ) {
        this.backendClientFacade = backendClientFacade;
        this.customClientFacade = customClientFacade;
        this.logger = logger.extend("DeliveryStorageMiddleware");
    }

    public setDateForGetAllMyDeliveries(date: string | null) {
        this.dateForGetAllMyDeliveries = date;
        this.dateForGetAllUnassignedDeliveries = null;
    }

    public setDateForGetAllUnassignedDeliveries(date: string | null) {
        this.dateForGetAllMyDeliveries = null;
        this.dateForGetAllUnassignedDeliveries = date;
    }

    public async getAll(date?: string): Promise<Delivery[]> {
        if (this.dateForGetAllMyDeliveries) {
            return await this.customClientFacade.GetAllMyDeliveriesEndpoint(this.dateForGetAllMyDeliveries);
        }
        if (this.dateForGetAllUnassignedDeliveries) {
            return await this.customClientFacade.GetAllUnassignedDeliveriesEndpoint(this.dateForGetAllUnassignedDeliveries);
        }
        return await this.backendClientFacade.GetAllEndpoint(date);
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

    public addSignatureToUploadQueue(stepId: string, signature: UploadableFile, receiver: string) {
        this.signatureUploadQueue.push({stepId, signature, receiver});
    }

    public async update(delivery: Delivery): Promise<void> {
        let completeUpdate: boolean = true;

        try {
            // Call an endpoint for every update to do on step in updateStepState
            for (let i = 0; i < this.updateStepState.length; i++) {
                if (this.updateStepState[i].deliveryId !== delivery.id)
                    continue;

                const step = delivery.steps.find(step => step.id === this.updateStepState[i].stepId)
                if (!step) {
                    this.logger.error(`Step with ID ${this.updateStepState[i].stepId} not found in delivery ${delivery.id}`);
                    return;
                }

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
                    case deliveryStepOperationAction.assignMyself:
                        await this.customClientFacade.PutStepAssignMyself(step);
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
                    case deliveryStepOperationAction.putSignature:
                        for (let i = 0; i < this.signatureUploadQueue.length; i++) {
                            if (this.signatureUploadQueue[i].stepId === step.id)
                                await this.customClientFacade.PutSignatureEndpoint(step, this.signatureUploadQueue[i].signature, this.signatureUploadQueue[i].receiver);
                        }
                        this.signatureUploadQueue = this.signatureUploadQueue.filter(file => file.stepId !== step.id);
                        break;
                    default:
                        this.logger.error(`Unsupported update action: ${this.updateStepState[i].state}`);
                        return;
                }
            }

            // Call an endpoint for every update to do on delivery in updateDeliveryState
            for (let i = 0; i < this.updateDeliveryState.length; i++) {
                if (this.updateDeliveryState[i].deliveryId !== delivery.id)
                    continue;

                completeUpdate = false;
                switch (this.updateDeliveryState[i].state) {
                    case deliveryOperationAction.putWaiting:
                        await this.customClientFacade.PutWaitingDeliveryEndpoint(delivery);
                        break;
                    case deliveryOperationAction.putValidate:
                        await this.customClientFacade.PutValidateDeliveryEndpoint(delivery);
                        break;
                    default:
                        this.logger.error(`Unsupported update action: ${this.updateStepState[i].state}`);
                        return;
                }
            }

            // If no update found, it means we need a complete update
            if (completeUpdate)
                await this.backendClientFacade.UpdateEndpoint(delivery);

        } catch (e) {
            this.logger.error(e);
            throw new Error(e as string);
        } finally {
            // Clear state already processed
            this.updateStepState = this.updateStepState.filter(state => state.deliveryId !== delivery.id);
            this.updateDeliveryState = this.updateDeliveryState.filter(state => state.deliveryId !== delivery.id);
        }
    }

}