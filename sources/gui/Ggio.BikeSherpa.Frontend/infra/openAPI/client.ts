import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const UrgencyDto = z.object({
  label: z.string(),
  value: z.string(),
  lastHourToOrder: z.number().int(),
});
const PricingStrategy = z.union([z.literal(0), z.literal(1), z.literal(2)]);
const PricingStrategyDto = z.object({
  label: z.string(),
  value: PricingStrategy,
});
const WorkHourDto = z.object({
  startDate: z.string().datetime({ offset: true }),
  endDate: z.string().datetime({ offset: true }),
});
const PackingSizeDto = z.object({ label: z.string(), value: z.string() });
const GeoPoint = z.object({ longitude: z.number(), latitude: z.number() });
const Address = z.object({
  name: z.string(),
  streetInfo: z.string(),
  complement: z.string().nullable(),
  postcode: z.string(),
  city: z.string(),
  coordinates: GeoPoint,
  phone: z.string().nullable(),
});
const DeliveryReportDetail = z.object({
  description: z.string(),
  address: Address.nullable(),
  price: z.number(),
  quantity: z.number().int(),
  courierName: z.string().nullable(),
});
const DeliveryReport = z.object({
  deliveryCode: z.string(),
  deliveryDate: z.string().datetime({ offset: true }),
  deliveryPrice: z.number(),
  deliveryPriceWithVat: z.number(),
  details: z.array(DeliveryReportDetail),
});
const Report = z.object({
  customerName: z.string(),
  startDate: z.string().datetime({ offset: true }),
  endDate: z.string().datetime({ offset: true }),
  totalPrice: z.number(),
  totalPriceWithVat: z.number(),
  deliveries: z.array(DeliveryReport),
});
const StepType = z.union([z.literal(0), z.literal(1)]);
const DeliveryStepCrud = z.object({
  packingSize: z.string(),
  stepZone: z.string(),
  stepType: StepType,
  order: z.number().int(),
  completed: z.boolean(),
  stepAddress: Address,
  distance: z.number(),
  courierId: z.string().nullable(),
  comment: z.string().nullable(),
  courierComment: z.string().nullable(),
  attachmentFilePaths: z.array(z.string()).nullable(),
  notBilled: z.boolean(),
  estimatedDeliveryDate: z.string().datetime({ offset: true }),
  realDeliveryDate: z.string().datetime({ offset: true }).nullable(),
  createdAt: z.string().datetime({ offset: true }),
  updatedAt: z.string().datetime({ offset: true }),
  id: z.string(),
});
const Link = z.object({
  href: z.string(),
  rel: z.string(),
  method: z.string(),
});
const DeliveryStepDto = z.object({
  data: DeliveryStepCrud,
  links: z.array(Link).nullable(),
});
const DeliveryStatus = z.union([
  z.literal(0),
  z.literal(1),
  z.literal(2),
  z.literal(3),
  z.literal(4),
]);
const DeliveryCrud = z.object({
  steps: z.array(DeliveryStepDto),
  urgency: z.string(),
  limitDate: z.string().datetime({ offset: true }).nullable(),
  pricingStrategy: PricingStrategy,
  status: DeliveryStatus,
  code: z.string(),
  customerId: z.string(),
  totalPrice: z.number().nullable(),
  discount: z.number().nullable(),
  discountReason: z.string().nullable(),
  extraCost: z.number().nullable(),
  extraCostReason: z.string().nullable(),
  customerReference: z.string().nullable(),
  details: z.array(z.string()),
  insulatedBox: z.boolean(),
  startDate: z.string().datetime({ offset: true }),
  contractDate: z.string().datetime({ offset: true }),
  needEstimate: z.boolean(),
  createdAt: z.string().datetime({ offset: true }),
  updatedAt: z.string().datetime({ offset: true }),
  id: z.string(),
});
const DeliveryDto = z.object({
  data: DeliveryCrud,
  links: z.array(Link).nullable(),
});
const UpdateDeliveryStepCompletionRequest = z.object({
  completed: z.boolean(),
});
const UpdateDeliveryStepCourierRequest = z.object({ courierId: z.string() });
const UpdateDeliveryStepOrderRequest = z.object({
  increment: z.number().int(),
});
const UpdateDeliveryStepTimeRequest = z.object({
  date: z.string().datetime({ offset: true }),
});
const CalculateDeliveryPriceResult = z.object({
  price: z.number(),
  priceWithVat: z.number(),
  totalDistance: z.number(),
});
const OperationBase = z.object({
  path: z.string(),
  op: z.string(),
  from: z.string(),
});
const Operation = OperationBase.and(z.object({ value: z.unknown() }));
const OperationOfDeliveryStep = Operation.and(z.object({}));
const JsonPatchDocumentOfDeliveryStep = z.object({
  operations: z.array(OperationOfDeliveryStep),
});
const AddressCrud = z.object({
  name: z.string(),
  streetInfo: z.string(),
  complement: z.string().nullable(),
  postcode: z.string(),
  city: z.string(),
  coordinates: GeoPoint,
  phone: z.string().nullable(),
});
const DeliveryType = z.union([z.literal(0), z.literal(1)]);
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
  defaultDeliveryType: DeliveryType.nullable(),
  id: z.string(),
});
const AddDeliveryByCustomerRequest = z.object({
  customer: CustomerCrud,
  delivery: DeliveryCrud,
});
const AddResultOfGuid = z.object({ id: z.string() });
const AttachmentRequest = z.object({ file: z.instanceof(File) });
const CustomerDto = z.object({
  data: CustomerCrud,
  links: z.array(Link).nullable(),
});
const CheckCustomerResponse = z.object({
  defaultDeliveryType: DeliveryType.nullable(),
  customerName: z.string(),
});
const ProblemDetails = z.record(z.unknown().nullable());
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
  UrgencyDto,
  PricingStrategy,
  PricingStrategyDto,
  WorkHourDto,
  PackingSizeDto,
  GeoPoint,
  Address,
  DeliveryReportDetail,
  DeliveryReport,
  Report,
  StepType,
  DeliveryStepCrud,
  Link,
  DeliveryStepDto,
  DeliveryStatus,
  DeliveryCrud,
  DeliveryDto,
  UpdateDeliveryStepCompletionRequest,
  UpdateDeliveryStepCourierRequest,
  UpdateDeliveryStepOrderRequest,
  UpdateDeliveryStepTimeRequest,
  CalculateDeliveryPriceResult,
  OperationBase,
  Operation,
  OperationOfDeliveryStep,
  JsonPatchDocumentOfDeliveryStep,
  AddressCrud,
  DeliveryType,
  CustomerCrud,
  AddDeliveryByCustomerRequest,
  AddResultOfGuid,
  AttachmentRequest,
  CustomerDto,
  CheckCustomerResponse,
  ProblemDetails,
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
    method: "post",
    path: "/customers/check",
    alias: "CheckCustomerEndpoint",
    tags: ["customer"],
    requestFormat: "json",
    parameters: [
      {
        name: "code",
        type: "Query",
        schema: z.string(),
      },
      {
        name: "email",
        type: "Query",
        schema: z.string(),
      },
    ],
    response: CheckCustomerResponse,
    errors: [
      {
        status: 404,
        description: `Not Found`,
        schema: z.record(z.unknown().nullable()),
      },
    ],
  },
  {
    method: "put",
    path: "/deliveries/:deliveryId/pending",
    alias: "ValidateDeliveryEndpoint",
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
        status: 403,
        description: `Forbidden`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "put",
    path: "/deliveries/:deliveryId/renew",
    alias: "RenewDeliveryEndpoint",
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
    path: "/deliveries/by_customer",
    alias: "AddDeliveryByCustomerEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: AddDeliveryByCustomerRequest,
      },
    ],
    response: z.void(),
  },
  {
    method: "get",
    path: "/deliveries/dailyDeliveries/:date",
    alias: "GetAllDailyDeliveriesEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "date",
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
    path: "/deliveries/price",
    alias: "CalculateDeliveryPriceEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: DeliveryCrud,
      },
    ],
    response: CalculateDeliveryPriceResult,
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
        schema: DeliveryStepCrud,
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
    method: "patch",
    path: "/delivery/:deliveryId/step/:stepId",
    alias: "PatchDeliveryStepEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: JsonPatchDocumentOfDeliveryStep,
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
    method: "post",
    path: "/delivery/:deliveryId/step/:stepId/attachment",
    alias: "AddDeliveryStepAttachmentEndpoint",
    tags: ["delivery"],
    requestFormat: "form-data",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ file: z.instanceof(File) }),
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
      {
        status: 404,
        description: `Not Found`,
        schema: z.void(),
      },
    ],
  },
  {
    method: "put",
    path: "/delivery/:deliveryId/step/:stepId/changeOrder",
    alias: "UpdateDeliveryStepOrderEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ increment: z.number().int() }),
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
    path: "/delivery/:deliveryId/step/:stepId/changeTime",
    alias: "UpdateDeliveryStepTimeEndpoint",
    tags: ["delivery"],
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: z.object({ date: z.string().datetime({ offset: true }) }),
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
    method: "delete",
    path: "/delivery/:deliveryId/step/:stepId/courier",
    alias: "DeleteDeliveryStepCourierEndpoint",
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
    method: "post",
    path: "/delivery/:deliveryId/step/:stepId/courier/:courierId",
    alias: "AddDeliveryStepCourierEndpoint",
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
    path: "/general/packingSizes",
    alias: "GetAllPackingSizesEndpoint",
    tags: ["general"],
    requestFormat: "json",
    response: z.array(PackingSizeDto),
  },
  {
    method: "get",
    path: "/general/parameters/lastHourToOrder",
    alias: "GetParameterLastHourToOrderEndpoint",
    tags: ["general"],
    requestFormat: "json",
    response: z.number().int(),
  },
  {
    method: "get",
    path: "/general/parameters/vatRate",
    alias: "GetParameterVatRateEndpoint",
    tags: ["general"],
    requestFormat: "json",
    response: z.number(),
  },
  {
    method: "get",
    path: "/general/parameters/workHours",
    alias: "GetParameterWorkHoursEndpoint",
    tags: ["general"],
    requestFormat: "json",
    response: WorkHourDto,
  },
  {
    method: "get",
    path: "/general/pricingStrategies",
    alias: "GetAllPricingStrategiesEndpoint",
    tags: ["general"],
    requestFormat: "json",
    response: z.array(PricingStrategyDto),
  },
  {
    method: "get",
    path: "/general/urgencies",
    alias: "GetAllUrgenciesEndpoint",
    tags: ["general"],
    requestFormat: "json",
    response: z.array(UrgencyDto),
  },
  {
    method: "get",
    path: "/reports/courier/:courierId",
    alias: "GetCourierReport",
    tags: ["report"],
    requestFormat: "json",
    parameters: [
      {
        name: "courierId",
        type: "Path",
        schema: z.string(),
      },
      {
        name: "startDate",
        type: "Query",
        schema: z.string().datetime({ offset: true }),
      },
      {
        name: "endDate",
        type: "Query",
        schema: z.string().datetime({ offset: true }),
      },
    ],
    response: z.string(),
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
    path: "/reports/customer/:customerId",
    alias: "GetCustomerReport",
    tags: ["report"],
    requestFormat: "json",
    parameters: [
      {
        name: "customerId",
        type: "Path",
        schema: z.string(),
      },
      {
        name: "startDate",
        type: "Query",
        schema: z.string().datetime({ offset: true }),
      },
      {
        name: "endDate",
        type: "Query",
        schema: z.string().datetime({ offset: true }),
      },
    ],
    response: Report,
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
