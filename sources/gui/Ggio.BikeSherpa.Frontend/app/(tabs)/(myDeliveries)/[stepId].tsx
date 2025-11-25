import StepDetailView from "@/steps/views/StepDetailView"

export default function MyDeliveryStepDetails() {
    return <StepDetailView canEdit={false} canValidate={true} />
}