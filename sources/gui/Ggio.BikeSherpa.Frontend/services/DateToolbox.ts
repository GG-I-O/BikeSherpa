export default class DateToolbox {
    public static getLastDayOfWeek(date: Date): Date {
        const auxDate = new Date(date); // To not overwrite date
        return new Date(auxDate.setDate(auxDate.getDate() - auxDate.getDay() + 7));
    }

    public static dateFilterFunction(dateFilter: Date | undefined, date: Date): boolean {
        if (!dateFilter) return true;
        
        return dateFilter.getFullYear() === date.getFullYear() &&
            dateFilter.getMonth() === date.getMonth() &&
            dateFilter.getDate() === date.getDate();
        
    }
    
    public static getFormattedDateFromISO(dateString: string): string {
        const date = new Date(dateString);
        return `${date.getDate()} / ${date.getMonth() + 1} / ${date.getFullYear()}`;
    }
    
    public static getFormattedTimeFromISO(dateString: string): string {
        const date = new Date(dateString);
        const hours = date.getHours().toString().padStart(2, '0');
        const minutes = date.getMinutes().toString().padStart(2, '0');
        return `${hours}:${minutes}`;
    }
}