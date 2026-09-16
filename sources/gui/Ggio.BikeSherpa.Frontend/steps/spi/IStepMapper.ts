import Delivery from "@/deliveries/models/Delivery";
import {Step} from "@/steps/models/Step";
import {StepToDisplay} from "@/steps/models/StepToDisplay";
import {StepDisplayForCourier} from "@/steps/models/StepDisplayForCourier";

export default interface IStepMapper {
    StepToStepToDisplay(delivery: Delivery, step: Step): StepToDisplay;
    StepToStepDisplayForCourier(delivery: Delivery, step: Step): StepDisplayForCourier;
}