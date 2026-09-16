export interface IColorServiceSpi {
     stringToColor(stringValue : string) : {light: string, dark: string} ;
}