import PriceDetail from "../../models/PriceDetail";

const defaultDeliveryDetails: PriceDetail[] = [
    {
        label: "Remise",
        price: 0
    },
    {
        label: "Prévenance",
        price: 0
    },
    {
        label: "Kilométrage",
        price: 0
    },
    {
        label: "Zonage : Grenoble",
        price: 5
    },
    {
        label: "Zonage : périphérie",
        price: 8
    },
    {
        label: "Isotherme",
        price: 0
    },
    {
        label: "Retour",
        price: 0
    },
    {
        label: "Mutualisation",
        price: 0
    },
    {
        label: "Créneau : éco",
        price: -2
    },
    {
        label: "Créneau : standard",
        price: 0
    },
    {
        label: "Créneau : urgent",
        price: 3
    },
    {
        label: "Prix fixe",
        price: 0
    },
]

export default defaultDeliveryDetails;