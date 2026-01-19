import Customer, { CustomerDto } from '@/customers/models/Customer';
import InputCustomer from '@/customers/models/InputCustomer';
import { hateoasRel } from '@/models/HateoasLink';
import { fakerFR as faker } from '@faker-js/faker';
import { createRandomAddress } from './address-fixtures';

export function createRandomCustomerWithUpdateAndDeleteLinks(): Customer {
  const fakeCompanyName = faker.company.name();
  const fakeAddress = createRandomAddress(fakeCompanyName);

  return {
    id: faker.string.uuid(),
    name: fakeCompanyName,
    address: fakeAddress,
    code: faker.string.alpha(3).toUpperCase(),
    phoneNumber: faker.phone.number(),
    email: faker.internet.email(),
    createdAt: faker.date.past().toString(),
    updatedAt: faker.date.anytime().toString(),
    links: [{
      href: "",
      rel: hateoasRel.update,
      method: ""
    },
    {
      href: "",
      rel: hateoasRel.delete,
      method: ""
    }]
  };
}

export function createRandomNewCustomerWithGetAndUpdateLinks(): Customer {
  const fakeId = faker.string.uuid();
  const fakeCompanyName = faker.company.name();
  const fakeAddress = createRandomAddress(fakeCompanyName);
  return {
    id: fakeId,
    name: fakeCompanyName,
    address: fakeAddress,
    code: faker.string.alpha(3).toUpperCase(),
    phoneNumber: faker.phone.number(),
    email: faker.internet.email(),
    links: [{
      href: `https://api.example.com/customers/${fakeId}`,
      rel: hateoasRel.get,
      method: "GET"
    },
    {
      href: "",
      rel: hateoasRel.update,
      method: ""
    }]
  };
}

export function createRandomCustomerWithUpdateLinks(): Customer {
  const fakeCompanyName = faker.company.name();
  const fakeAddress = createRandomAddress(fakeCompanyName);
  return {
    id: faker.string.uuid(),
    name: fakeCompanyName,
    address: fakeAddress,
    code: faker.string.alpha(3).toUpperCase(),
    phoneNumber: faker.phone.number(),
    email: faker.internet.email(),
    createdAt: faker.date.past().toString(),
    updatedAt: faker.date.anytime().toString(),
    links: [
      {
        href: "",
        rel: hateoasRel.update,
        method: ""
      }]
  };
}

export function createRandomCustomerWithoutLinks(): Customer {
  const fakeCompanyName = faker.company.name();
  const fakeAddress = createRandomAddress(fakeCompanyName);
  return {
    id: faker.string.uuid(),
    name: fakeCompanyName,
    address: fakeAddress,
    code: faker.string.alpha(3).toUpperCase(),
    phoneNumber: faker.phone.number(),
    email: faker.internet.email(),
    createdAt: faker.date.past().toString(),
    updatedAt: faker.date.anytime().toString(),
    links: []
  };
}

export function createRandomCustomerDto(): CustomerDto {
  const fakeCompanyName = faker.company.name();
  const fakeAddress = createRandomAddress(fakeCompanyName);
  return {
    data: {
      id: faker.string.uuid(),
      name: fakeCompanyName,
      address: fakeAddress,
      code: faker.string.alpha(3).toUpperCase(),
      siret: null,
      phoneNumber: faker.phone.number(),
      email: faker.internet.email(),
      createdAt: faker.date.past().toString(),
      updatedAt: faker.date.anytime().toString()
    },
    links: null
  };
}

export function createRandomInputCustomer(): InputCustomer {
  const fakeCompanyName = faker.company.name();
  const fakeAddress = createRandomAddress(fakeCompanyName);
  return {
    name: fakeCompanyName,
    address: fakeAddress,
    code: faker.string.alpha(3),
    phoneNumber: faker.phone.number(),
    email: faker.internet.email()
  };
}