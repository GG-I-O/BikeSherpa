export default class DateToolbox {
    public static getLastDayOfWeek(date: Date): Date {
        const auxDate = new Date(date); // To not overwrite date
        return new Date(auxDate.setDate(auxDate.getDate() - auxDate.getDay() + 7));
    }
}