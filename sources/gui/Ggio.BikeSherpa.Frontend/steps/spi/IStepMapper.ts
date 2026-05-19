import Delivery from "@/deliveries/models/Delivery";
import {Step} from "@/steps/models/Step";
import {StepToDisplay} from "@/steps/models/StepToDisplay";

export default interface IStepMapper {
    StepToStepToDisplay(delivery: Delivery, step: Step): StepToDisplay;
}