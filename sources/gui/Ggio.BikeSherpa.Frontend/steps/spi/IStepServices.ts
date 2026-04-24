export interface IStepServices {
    updateTime(stepId: string, hours: number, minutes: number): void;
    reorderStep(stepId: string, newOrder: number): void;
}