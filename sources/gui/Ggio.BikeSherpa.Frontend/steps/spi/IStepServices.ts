export interface IStepServices {
    updateTime(stepId: string, hours: number, minutes: number): void;
    updateTimeForADay(stepId: string, hours: number, minutes: number): void;
    
    reorderStep(stepId: string, newOrder: number): void;
    reorderStepForADay(stepId: string, increment: number): void;
    
    assignCourier(stepId: string, courierId: string): void;
    unassignCourier(stepId: string): void;
}