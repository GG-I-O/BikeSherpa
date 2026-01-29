import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const AddressCrud = z.object({
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
  address: AddressCrud,
  createdAt: z.string().datetime({ offset: true }),
  updatedAt: z.string().datetime({ offset: true }),
  id: z.string(),
});
const Link = z.object({
  href: z.string(),
  rel: z.string(),
  method: z.string(),
});
const CustomerDto = z.object({
  data: CustomerCrud,
  links: z.array(Link).nullable(),
});
const AddResultOfGuid = z.object({ id: z.string() });
const CourseCrud = z.object({
  startDate: z.string().datetime({ offset: true }),
  id: z.string(),
});
const CourierCrud = z.object({
  firstName: z.string(),
  lastName: z.string(),
  code: z.string(),
  email: z.string(),
  phoneNumber: z.string(),
  address: AddressCrud,
  createdAt: z.string().datetime({ offset: true }),
  updatedAt: z.string().datetime({ offset: true }),
  id: z.string(),
});
const CourierDto = z.object({
  data: CourierCrud,
  links: z.array(Link).nullable(),
});

export const schemas = {
  AddressCrud,
  CustomerCrud,
  Link,
  CustomerDto,
  AddResultOfGuid,
  CourseCrud,
  CourierCrud,
  CourierDto,
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
    path: "/courier",
    alias: "AddCourierEndpoint",
    tags: ["courier"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CourierCrud,
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
    method: "put",
    path: "/courier/:courierId",
    alias: "UpdateCourierEndpoint",
    tags: ["courier"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CourierCrud,
      },
      {
        name: "courierId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
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
    path: "/courier/:courierId",
    alias: "GetCourierEndpoint",
    tags: ["courier"],
    requestFormat: "json",
    parameters: [
      {
        name: "courierId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: CourierDto,
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
    method: "delete",
    path: "/courier/:courierId",
    alias: "DeleteCourierEndpoint",
    tags: ["courier"],
    requestFormat: "json",
    parameters: [
      {
        name: "courierId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
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
    path: "/couriers/:lastSync",
    alias: "GetAllCouriersEndpoint",
    tags: ["courier"],
    requestFormat: "json",
    parameters: [
      {
        name: "lastSync",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(CourierDto),
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
    path: "/customer",
    alias: "AddCustomerEndpoint",
    tags: ["customer"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: CustomerCrud,
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
    method: "put",
    path: "/customer/:customerId",
    alias: "UpdateCustomerEndpoint",
    tags: ["customer"],
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
    response: z.void(),
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
    path: "/customer/:customerId",
    alias: "GetCustomerEndpoint",
    tags: ["customer"],
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
    method: "delete",
    path: "/customer/:customerId",
    alias: "DeleteCustomerEndpoint",
    tags: ["customer"],
    requestFormat: "json",
    parameters: [
      {
        name: "customerId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.void(),
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
    path: "/customers/:lastSync",
    alias: "GetAllCustomersEndpoint",
    tags: ["customer"],
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

// Helper function to get the first tag from an endpoint by alias
export function getTagByAlias(alias: string): string | undefined {
  const endpoint = endpoints.find((e) => "alias" in e && e.alias === alias);
  return endpoint && "tags" in endpoint && Array.isArray(endpoint.tags)
    ? endpoint.tags[0]
    : undefined;
}
