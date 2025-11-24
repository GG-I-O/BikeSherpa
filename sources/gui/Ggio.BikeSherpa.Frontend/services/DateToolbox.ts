export default class DateToolbox {
    public static formatDate(date: Date): string {
        return `${date.getDate().toString().padStart(2, '0')}/${(date.getMonth() + 1).toString().padStart(2, '0')}/${date.getFullYear()}`;
    }

    public static formatTime(date: Date): string {
        return `${date.getHours().toString().padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}`;
    }

    public static getLastDayOfWeek(date: Date): Date {
        const auxDate = new Date(date); // To not overwrite date
        return new Date(auxDate.setDate(auxDate.getDate() - auxDate.getDay() + 7));
    }
}