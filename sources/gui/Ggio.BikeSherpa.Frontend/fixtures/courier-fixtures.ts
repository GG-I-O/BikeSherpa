import Courier, { CourierDto } from '@/couriers/models/Courier';
import InputCourier from '@/couriers/models/InputCourier';
import { hateoasRel, Link } from '@/models/HateoasLink';
import { fakerFR as faker } from '@faker-js/faker';
import { createRandomAddress } from './address-fixtures';

export enum linkType { "none", "update", "delete", "get", "getAndUpdate", "updateAndDelete" }

export function createRandomInputCourier(): InputCourier {
    const fakeFirstName = faker.person.firstName();
    const fakeLastName = faker.person.lastName();
    const courierFullName = `${fakeFirstName} ${fakeLastName}`
    const fakeAddress = createRandomAddress(courierFullName);
    return {
        firstName: fakeFirstName,
        lastName: fakeLastName,
        address: fakeAddress,
        code: faker.string.alpha(3).toUpperCase(),
        phoneNumber: faker.phone.number(),
        email: faker.internet.email()
    };
}

export function createRandomCourierDto(): CourierDto {
    const inputCourier = createRandomInputCourier();
    return {
        data: {
            id: faker.string.uuid(),
            ...inputCourier,
            createdAt: faker.date.past().toString(),
            updatedAt: faker.date.anytime().toString()
        },
        links: null
    };
}

function generateLinks(fakeId: string, links: string): Link[] | undefined {
    switch (links) {
        case "update":
            return [{
                href: "",
                rel: hateoasRel.update,
                method: ""
            }];
        case "delete":
            return [{
                href: "",
                rel: hateoasRel.delete,
                method: ""
            }];
        case "get":
            return [{
                href: `https://api.example.com/couriers/${fakeId}`,
                rel: hateoasRel.get,
                method: "GET"
            }];
        case "getAndUpdate":
            return [{
                href: `https://api.example.com/couriers/${fakeId}`,
                rel: hateoasRel.get,
                method: "GET"
            },
            {
                href: "",
                rel: hateoasRel.update,
                method: ""
            }];
        case "updateAndDelete":
            return [{
                href: "",
                rel: hateoasRel.update,
                method: ""
            },
            {
                href: "",
                rel: hateoasRel.delete,
                method: ""
            }];
        default:
            return undefined;
    }
}

export function createRandomCourier(alreadyCreated: boolean, links: linkType): Courier {
    const inputCourier = createRandomInputCourier();
    let courier: Courier = {
        id: faker.string.uuid(),
        ...inputCourier
    }
    if (alreadyCreated) {
        courier = {
            ...courier,
            createdAt: faker.date.past().toString(),
            updatedAt: faker.date.anytime().toString()
        }
    }

    let courierLinks: Link[] | undefined;

    if (links !== linkType.none) {
        courierLinks = generateLinks(courier.id, linkType[links]);
    }

    if (courierLinks) {
        courier = {
            ...courier,
            links: courierLinks
        }
    }

    return courier;
}