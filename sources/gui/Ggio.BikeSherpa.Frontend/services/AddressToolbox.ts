import { Linking, Platform } from "react-native";

export default class AddressToolbox {
    public static openAdressInMaps(address: string) {
        try {
            const encodedAddress = encodeURIComponent(address);

            let url: string;

            if (Platform.OS === 'web') {
                url = `https://www.google.com/maps/search/?api=1&query=${encodedAddress}`;
            }
            else {
                url = Platform.select({
                    ios: `maps:0,0?q=${encodedAddress}`,
                    android: `geo:0,0?q=${encodedAddress}`,
                }) ?? '';
            }

            Linking.openURL(url ?? '');
        }
        catch (e) {
            console.error('Error opening maps:', e);
        }
    }
}