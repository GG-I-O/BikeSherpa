export default class DateToolbox {
    public static getLastDayOfWeek(date: Date): Date {
        const auxDate = new Date(date); // To not overwrite date
        return new Date(auxDate.setDate(auxDate.getDate() - auxDate.getDay() + 7));
    }

    public static dateFilterFunction(dateFilter: string, date: Date): boolean {
        // Today
        const todayStart = new Date();
        todayStart.setHours(0, 0, 0, 0);
        const todayEnd = new Date(todayStart);
        todayEnd.setHours(23, 59, 59, 0);

        // Tomorrow
        const tomorrowStart = new Date(todayStart);
        tomorrowStart.setDate(tomorrowStart.getDate() + 1);
        const tomorrowEnd = new Date(tomorrowStart);
        tomorrowEnd.setHours(23, 59, 59, 0);

        // Week
        const endOfWeek = DateToolbox.getLastDayOfWeek(todayEnd);
        
        switch (dateFilter) {
            case '1':
                return date.valueOf() >= todayStart.valueOf() && date.valueOf() <= todayEnd.valueOf();
            case '2':
                return date.valueOf() >= tomorrowStart.valueOf() && date.valueOf() <= tomorrowEnd.valueOf();
            case '7':
                return date.valueOf() >= todayStart.valueOf() && date.valueOf() <= endOfWeek.valueOf();
            default:
                return true;
        }
    }
    
    public static getFormattedDateFromISO(dateString: string): string {
        const date = new Date(dateString);
        return `${date.getDate()} / ${date.getMonth() + 1} / ${date.getFullYear()}`;
    }
    
    public static getFormattedTimeFromISO(dateString: string): string {
        const date = new Date(dateString);
        return `${date.getHours()}:${date.getMinutes()}`;
    }
}