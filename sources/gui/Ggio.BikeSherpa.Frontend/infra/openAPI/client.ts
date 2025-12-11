import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const Address = z.object({
  name: z.string(),
  streetInfo: z.string(),
  complement: z.string().nullable(),
  postcode: z.string(),
  city: z.string(),
});
const CustomerCrud = z.object({
  name: z.string(),
  code: z.string(),
  siret: z.string().nullable(),
  email: z.string(),
  phoneNumber: z.string(),
  address: Address,
  createdAt: z.string().datetime({ offset: true }),
  updatedAt: z.string().datetime({ offset: true }),
  id: z.string(),
});
const Link = z.object({
  href: z.string(),
  rel: z.string(),
  method: z.string(),
});
const GuidWithHateoas = z.object({
  id: z.string(),
  links: z.array(Link).nullable(),
});
const AddResultOfGuidWithHateoas = z.object({ id: GuidWithHateoas.nullable() });
const CustomerDto = z.object({
  data: CustomerCrud,
  links: z.array(Link).nullable(),
});
const CourseCrud = z.object({
  startDate: z.string().datetime({ offset: true }),
  id: z.string(),
});
const AddResultOfGuid = z.object({ id: z.string() });

export const schemas = {
  Address,
  CustomerCrud,
  Link,
  GuidWithHateoas,
  AddResultOfGuidWithHateoas,
  CustomerDto,
  CourseCrud,
  AddResultOfGuid,
};

const endpoints = makeApi([
  {
    method: "post",
    path: "/api/course",
    alias: "AddCourseEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CourseCrud,
      },
    ],
    response: z.object({ id: z.string() }),
    errors: [
      {
        status: 401,
        description: `Unauthorized`,
        schema: z.void(),
      },
      {
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "get",
    path: "/api/course/:Id",
    alias: "GetCourseEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CourseCrud,
    errors: [
      {
        status: 401,
        description: `Unauthorized`,
        schema: z.void(),
      },
      {
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "get",
    path: "/api/courses",
    alias: "GetAllCoursesEndpoint",
    requestFormat: "json",
    response: z.array(CourseCrud),
    errors: [
      {
        status: 401,
        description: `Unauthorized`,
        schema: z.void(),
      },
      {
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "post",
    path: "/api/customer",
    alias: "AddCustomerEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CustomerCrud,
      },
    ],
    response: AddResultOfGuidWithHateoas,
    errors: [
      {
        status: 401,
        description: `Unauthorized`,
        schema: z.void(),
      },
      {
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "put",
    path: "/api/customer/:customerId",
    alias: "UpdateCustomerEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CustomerCrud,
      },
      {
        name: "customerId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: AddResultOfGuidWithHateoas,
    errors: [
      {
        status: 401,
        description: `Unauthorized`,
        schema: z.void(),
      },
      {
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "get",
    path: "/api/customer/:customerId",
    alias: "GetCustomerEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "customerId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CustomerDto,
    errors: [
      {
        status: 401,
        description: `Unauthorized`,
        schema: z.void(),
      },
      {
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "get",
    path: "/api/customers/:lastSync",
    alias: "GetAllCustomersEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "lastSync",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CustomerDto),
    errors: [
      {
        status: 401,
        description: `Unauthorized`,
        schema: z.void(),
      },
      {
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
]);

export const api = new Zodios(endpoints);

export function createApiClient(baseUrl: string, options?: ZodiosOptions) {
  return new Zodios(baseUrl, endpoints, options);
}
