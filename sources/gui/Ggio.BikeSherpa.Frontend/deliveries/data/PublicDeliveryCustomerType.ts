const enum PublicDeliveryCustomerTypeEnum {
    None,
    Sender,
    Receiver
}

const PublicDeliveryCustomerTypeOptions = [
    {value: PublicDeliveryCustomerTypeEnum.None.toString(), label: 'Aucun'},
    {value: PublicDeliveryCustomerTypeEnum.Sender.toString(), label: 'Expéditeur'},
    {value: PublicDeliveryCustomerTypeEnum.Receiver.toString(), label: 'Destinataire'}
];

export { PublicDeliveryCustomerTypeEnum, PublicDeliveryCustomerTypeOptions };