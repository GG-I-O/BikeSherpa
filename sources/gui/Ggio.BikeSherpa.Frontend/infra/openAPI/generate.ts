import { generateZodClientFromOpenAPI } from "openapi-zod-client";
import { readFileSync } from "fs";
import { resolve } from "path";

const swaggerPath = resolve(__dirname, "./swagger.json");
const outputPath = resolve(__dirname, "./client.ts");
const templatePath = resolve(__dirname, "./template.hbs");

const swaggerDoc = JSON.parse(readFileSync(swaggerPath, "utf-8"));

generateZodClientFromOpenAPI({
    openApiDoc: swaggerDoc,
    distPath: outputPath,
    templatePath: templatePath,
    options: {
        withAlias: true,
        withImplicitRequiredProps: true,
        endpointDefinitionRefiner: (endpoint, operation) => {
            if (operation.tags && operation.tags.length > 0) {
                return {
                    ...endpoint,
                    tags: operation.tags,
                };
            }
            return endpoint;
        },
    },
}).then(() => {
    console.log("client.ts generated");
}).catch((error) => {
    console.error("Error generating client:", error);
    process.exit(1);
});
