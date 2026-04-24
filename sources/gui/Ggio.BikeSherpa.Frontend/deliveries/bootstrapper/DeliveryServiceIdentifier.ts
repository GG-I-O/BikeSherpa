export const DeliveryServiceIdentifier = {
    Storage: Symbol.for('DeliveryStorageContext'),
    Services: Symbol.for('DeliveryServices'),
    BackendClientFacade: Symbol.for('DeliveryBackendClientFacade'),
    CustomBackendClientFacade: Symbol.for('DeliveryCustomBackendClientFacade'),
    DropdownOptionsService: Symbol.for('DeliveryDropdownOptionsService'),
    StorageMiddleware: Symbol.for('DeliveryStorageMiddleware')
}