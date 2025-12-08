import { makeApi, Zodios, type ZodiosOptions } from "@zodios/core";
import { z } from "zod";

const GgioBikeSherpaBackendFeaturesCoursesCourseCrud = z
  .object({ startDate: z.string().datetime({ offset: true }), id: z.string() })
  .partial();
const GgioBikeSherpaBackendModelAddResultOfGuid = z
  .object({ id: z.string() })
  .partial();

export const schemas = {
  GgioBikeSherpaBackendFeaturesCoursesCourseCrud,
  GgioBikeSherpaBackendModelAddResultOfGuid,
};

const endpoints = makeApi([
  {
    method: "post",
    path: "/api/course",
    alias: "GgioBikeSherpaBackendFeaturesCoursesAddAddCourseEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "body",
        type: "Body",
        schema: GgioBikeSherpaBackendFeaturesCoursesCourseCrud,
      },
    ],
    response: z.object({ id: z.string() }).partial(),
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
    alias: "GgioBikeSherpaBackendFeaturesCoursesGetGetEndpoint",
    requestFormat: "json",
    parameters: [
      {
        name: "id",
        type: "Path",
        schema: z.string(),
      },
    ],
    response: GgioBikeSherpaBackendFeaturesCoursesCourseCrud,
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
    alias: "GgioBikeSherpaBackendFeaturesCoursesGetAllGetAllEndpoint",
    requestFormat: "json",
    response: z.array(GgioBikeSherpaBackendFeaturesCoursesCourseCrud),
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
