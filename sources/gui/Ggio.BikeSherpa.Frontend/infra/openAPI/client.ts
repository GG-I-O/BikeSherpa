import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const PricingStrategy = z.union([z.literal(0), z.literal(1), z.literal(2)]);
const DeliveryStatus = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
]);
const HasDomainEventsBase = z.object({});
const EntityBaseOfGuid = HasDomainEventsBase.and(z.object({ id: z.string() }));
const StepType = z.union([z.literal(0), z.literal(1)]);
const Address = z.object({
  name: z.string(),
  streetInfo: z.string(),
  complement: z.string().nullable(),
  postcode: z.string(),
  city: z.string(),
  coordinates: z.string(),
});
const City = z.object({ name: z.string() });
const DeliveryZone = z.object({ name: z.string(), cities: z.array(City) });
const DeliveryStep = EntityBaseOfGuid.and(
  z.object({
    stepType: StepType,
    order: z.number().int(),
    completed: z.boolean(),
    stepAddress: Address,
    stepZone: DeliveryZone,
    distance: z.number(),
    courierId: z.string().nullable(),
    comment: z.string().nullable(),
    attachmentFilePaths: z.array(z.string()).nullable(),
    estimatedDeliveryDate: z.string().datetime({ offset: true }),
    realDeliveryDate: z.string().datetime({ offset: true }).nullable(),
    createdAt: z.string().datetime({ offset: true }),
    updatedAt: z.string().datetime({ offset: true }),
  })
);
const DeliveryCrud = z.object({
  pricingStrategy: PricingStrategy,
  status: DeliveryStatus,
  code: z.string(),
  customerId: z.string(),
  urgency: z.string(),
  totalPrice: z.number().nullable(),
  discount: z.number().nullable(),
  reportId: z.string().nullable(),
  steps: z.array(DeliveryStep),
  details: z.array(z.string()),
  packingSize: z.string(),
  insulatedBox: z.boolean(),
  contractDate: z.string().datetime({ offset: true }),
  startDate: z.string().datetime({ offset: true }),
  createdAt: z.string().datetime({ offset: true }),
  updatedAt: z.string().datetime({ offset: true }),
  id: z.string(),
});
const Link = z.object({
  href: z.string(),
  rel: z.string(),
  method: z.string(),
});
const DeliveryDto = z.object({
  data: DeliveryCrud,
  links: z.array(Link).nullable(),
});
const UpdateDeliveryStepCompletionRequest = z.object({
  completed: z.boolean(),
});
const UpdateDeliveryStepCourierRequest = z.object({ courierId: z.string() });
const UpdateDeliveryStepOrderRequest = z.object({ order: z.number().int() });
const AddResultOfGuid = z.object({ id: z.string() });
const AddressCrud = z.object({
  name: z.string(),
  streetInfo: z.string(),
  complement: z.string().nullable(),
  postcode: z.string(),
  city: z.string(),
  coordinates: z.string(),
});
const CustomerCrud = z.object({
  name: z.string(),
  code: z.string(),
  siret: z.string().nullable(),
  vatNumber: z.string().nullable(),
  email: z.string(),
  phoneNumber: z.string(),
  address: AddressCrud,
  createdAt: z.string().datetime({ offset: true }),
  updatedAt: z.string().datetime({ offset: true }),
  id: z.string(),
});
const CustomerDto = z.object({
  data: CustomerCrud,
  links: z.array(Link).nullable(),
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
  PricingStrategy,
  DeliveryStatus,
  HasDomainEventsBase,
  EntityBaseOfGuid,
  StepType,
  Address,
  City,
  DeliveryZone,
  DeliveryStep,
  DeliveryCrud,
  Link,
  DeliveryDto,
  UpdateDeliveryStepCompletionRequest,
  UpdateDeliveryStepCourierRequest,
  UpdateDeliveryStepOrderRequest,
  AddResultOfGuid,
  AddressCrud,
  CustomerCrud,
  CustomerDto,
  CourierCrud,
  CourierDto,
};

const endpoints = makeApi([
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
  {
    method: "get",
    path: "/deliveries/:lastSync",
    alias: "GetAllDeliveriesEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "lastSync",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: z.array(DeliveryDto),
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
    path: "/delivery",
    alias: "AddDeliveryEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: DeliveryCrud,
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
    path: "/delivery/:deliveryId",
    alias: "UpdateDeliveryEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: DeliveryCrud,
      },
      {
        name: "deliveryId",
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
    path: "/delivery/:deliveryId",
    alias: "GetDeliveryEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "deliveryId",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: DeliveryDto,
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
    path: "/delivery/:deliveryId",
    alias: "DeleteDeliveryEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "deliveryId",
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
    method: "put",
    path: "/delivery/:deliveryId/cancel",
    alias: "CancelDeliveryEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "deliveryId",
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
    method: "post",
    path: "/delivery/:deliveryId/step",
    alias: "AddDeliveryStepEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: DeliveryStep,
      },
      {
        name: "deliveryId",
        type: "Path",
        schema: z.string(),
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
    method: "delete",
    path: "/delivery/:deliveryId/step/:stepId",
    alias: "DeleteDeliveryStepEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "deliveryId",
        type: "Path",
        schema: z.string(),
      },
      {
        name: "stepId",
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
    method: "put",
    path: "/delivery/:deliveryId/step/:stepId/complete",
    alias: "UpdateDeliveryStepCompletionEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ completed: z.boolean() }),
      },
      {
        name: "deliveryId",
        type: "Path",
        schema: z.string(),
      },
      {
        name: "stepId",
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
    method: "put",
    path: "/delivery/:deliveryId/step/:stepId/courier",
    alias: "UpdateDeliveryStepCourierEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ courierId: z.string() }),
      },
      {
        name: "deliveryId",
        type: "Path",
        schema: z.string(),
      },
      {
        name: "stepId",
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
    method: "put",
    path: "/delivery/:deliveryId/step/:stepId/order",
    alias: "UpdateDeliveryStepOrderEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ order: z.number().int() }),
      },
      {
        name: "deliveryId",
        type: "Path",
        schema: z.string(),
      },
      {
        name: "stepId",
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
