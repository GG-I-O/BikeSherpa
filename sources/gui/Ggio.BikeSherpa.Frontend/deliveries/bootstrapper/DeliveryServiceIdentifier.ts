export const DeliveryServiceIdentifier = {
    Storage: Symbol.for('DeliveryStorageContext'),
    Services: Symbol.for('DeliveryServices'),
    PublicServices: Symbol.for('PublicDeliveryServices'),
    Mapper: Symbol.for('DeliveryMapper'),
    BackendClientFacade: Symbol.for('DeliveryBackendClientFacade'),
    CustomBackendClientFacade: Symbol.for('DeliveryCustomBackendClientFacade'),
    DropdownOptionsService: Symbol.for('DeliveryDropdownOptionsService'),
    StorageMiddleware: Symbol.for('DeliveryStorageMiddleware')
}