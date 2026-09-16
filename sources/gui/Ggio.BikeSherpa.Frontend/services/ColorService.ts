import {IColorServiceSpi} from "@/spi/ColorServiceSpi";
import {injectable} from "inversify";

@injectable()
export default class ColorService implements IColorServiceSpi {
    private cyrb53(str: string, seed = 0): number {
        let h1 = 0xdeadbeef ^ seed;
        let h2 = 0x41c6ce57 ^ seed;
        for (let i = 0; i < str.length; i++) {
            const ch = str.charCodeAt(i);
            h1 = Math.imul(h1 ^ ch, 2654435761);
            h2 = Math.imul(h2 ^ ch, 1597334677);
        }
        h1 = Math.imul(h1 ^ (h1 >>> 16), 2246822507);
        h1 ^= Math.imul(h2 ^ (h2 >>> 13), 3266489909);
        h2 = Math.imul(h2 ^ (h2 >>> 16), 2246822507);
        h2 ^= Math.imul(h1 ^ (h1 >>> 13), 3266489909);
        return 4294967296 * (2097151 & h2) + (h1 >>> 0);
    }

    public stringToColor(str: string): {light: string, dark: string} {
        const hue = this.cyrb53(str) % 360;
        return {
            light: `hsl(${hue}, 60%, 85%)`,
            dark:  `hsl(${hue}, 35%, 25%)`,
        };
    }
}