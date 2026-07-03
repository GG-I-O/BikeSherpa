import {IColorServiceSpi} from "@/spi/ColorServiceSpi";
import {injectable} from "inversify";

@injectable()
export default class ColorService implements IColorServiceSpi {
    public stringToColor(stringValue: string): string {
        let hash = 0;
        for (let i = 0; i < stringValue.length; i++) {
            hash = stringValue.charCodeAt(i) + ((hash << 5) - hash);
        }
        let color = '#';
        for (let i = 0; i < 3; i++) {
            const value = (hash >> (i * 8)) & 0xFF;
            color += ('00' + value.toString(16)).substr(-2);
        }
        return color;
    }
}