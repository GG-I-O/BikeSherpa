export default class DateToolbox {
    public static getLastDayOfWeek(date: Date): Date {
        const auxDate = new Date(date); // To not overwrite date
        return new Date(auxDate.setDate(auxDate.getDate() - auxDate.getDay() + 7));
    }
    
    public static getFirstDayOfTheMonth(date: Date): Date {
        return new Date(date.getFullYear(), date.getMonth(), 1);
    }
    
    public static getLastDayOfTheMonth(date: Date): Date {
        return new Date(date.getFullYear(), date.getMonth() + 1, 0);
    }

    public static dateFilterFunction(dateFilter: Date | undefined, date: Date): boolean {
        if (!dateFilter) return true;
        
        return dateFilter.getFullYear() === date.getFullYear() &&
            dateFilter.getMonth() === date.getMonth() &&
            dateFilter.getDate() === date.getDate();
    }

    public static doubleDateFilterFunction(startDateFilter: Date, endDateFilter: Date, date: Date): boolean {
        return (
            startDateFilter.getFullYear() <= date.getFullYear() &&
            startDateFilter.getMonth() <= date.getMonth() &&
            startDateFilter.getDate() <= date.getDate()
            &&
            endDateFilter.getFullYear() >= date.getFullYear() &&
            endDateFilter.getMonth() >= date.getMonth() &&
            endDateFilter.getDate() >= date.getDate()
        );

    }
    
    public static getFormattedDateFromISO(dateString: string): string {
        const date = new Date(dateString);
        const day = date.getDate().toString().padStart(2, '0');
        const month = (date.getMonth() + 1).toString().padStart(2, '0');
        return `${day} / ${month} / ${date.getFullYear()}`;
    }
    
    public static getFormattedTimeFromISO(dateString: string): string {
        const date = new Date(dateString);
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        return `${hours}:${minutes}`;
    }
}