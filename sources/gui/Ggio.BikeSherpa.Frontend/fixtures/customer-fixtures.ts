import Customer, { CustomerDto } from '@/customers/models/Customer';
import InputCustomer from '@/customers/models/InputCustomer';
import { hateoasRel, Link } from '@/models/HateoasLink';
import { fakerFR as faker } from '@faker-js/faker';
import { createRandomAddress } from './address-fixtures';

export enum linkType { "none", "update", "delete", "get", "getAndUpdate", "updateAndDelete" }

export function createRandomInputCustomer(): InputCustomer {
  const fakeCompanyName = faker.company.name();
  const fakeAddress = createRandomAddress(fakeCompanyName);
  return {
    name: fakeCompanyName,
    address: fakeAddress,
    code: faker.string.alpha(3).toUpperCase(),
    phoneNumber: faker.phone.number(),
    email: faker.internet.email()
  };
}

export function createRandomCustomerDto(): CustomerDto {
  const inputCustomer = createRandomInputCustomer();
  return {
    data: {
      id: faker.string.uuid(),
      ...inputCustomer,
      siret: null,
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
        href: `https://api.example.com/customers/${fakeId}`,
        rel: hateoasRel.get,
        method: "GET"
      }];
    case "getAndUpdate":
      return [{
        href: `https://api.example.com/customers/${fakeId}`,
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

export function createRandomCustomer(alreadyCreated: boolean, links: linkType): Customer {
  const inputCustomer = createRandomInputCustomer();
  let customer: Customer = {
    id: faker.string.uuid(),
    ...inputCustomer
  }
  if (alreadyCreated) {
    customer = {
      ...customer,
      createdAt: faker.date.past().toString(),
      updatedAt: faker.date.anytime().toString()
    }
  }

  let customerLinks: Link[] | undefined;

  if (links !== linkType.none) {
    customerLinks = generateLinks(customer.id, linkType[links]);
  }

  if (customerLinks) {
    customer = {
      ...customer,
      links: customerLinks
    }
  }

  return customer;
}