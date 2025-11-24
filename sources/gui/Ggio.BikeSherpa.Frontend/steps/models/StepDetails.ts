import { StepNotice } from "./StepNotice";

// colisage, delai de prévenance, type de prise en charge, volume total, nb address à générer, num d'étape dans la course
type StepDetails = {
    size: number;
    notice: StepNotice;
    isotherm: boolean;
    precise: boolean;
    return: boolean;
}

export default StepDetails;