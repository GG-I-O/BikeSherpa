import { Address } from '@/models/Address';
import { fakerFR as faker } from '@faker-js/faker';

export function createRandomAddress(name: string): Address {
    const fakeStreetInfo = faker.location.streetAddress();
    const fakePostcode = faker.location.zipCode();
    const fakeCity = faker.location.city();
    return {
        name: name,
        fullAddress: `${fakeStreetInfo} ${fakePostcode} ${fakeCity}`,
        streetInfo: fakeStreetInfo,
        complement: null,
        postcode: fakePostcode,
        city: fakeCity
    };
}