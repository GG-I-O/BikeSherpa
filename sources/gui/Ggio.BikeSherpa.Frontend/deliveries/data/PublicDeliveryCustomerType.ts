const enum PublicDeliveryCustomerTypeEnum {
    Sender,
    Receiver,
    None
}

const PublicDeliveryCustomerTypeOptions = [
    {value: PublicDeliveryCustomerTypeEnum.Sender.toString(), label: 'Expéditeur'},
    {value: PublicDeliveryCustomerTypeEnum.Receiver.toString(), label: 'Destinataire'},
    {value: PublicDeliveryCustomerTypeEnum.None.toString(), label: "Tiers"}
];

export { PublicDeliveryCustomerTypeEnum, PublicDeliveryCustomerTypeOptions };