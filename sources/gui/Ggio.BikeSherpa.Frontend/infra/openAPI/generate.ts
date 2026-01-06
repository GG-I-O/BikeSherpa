import { generateZodClientFromOpenAPI } from "openapi-zod-client";
import { readFileSync } from "fs";
import { resolve } from "path";
import https from "https";

// Allow SSL
process.env.NODE_TLS_REJECT_UNAUTHORIZED = '0';

const source = process.argv[2] || resolve(__dirname, "./swagger.json");
const isUrl = source.startsWith("http");
const outputPath = resolve(__dirname, "./client.ts");
const templatePath = resolve(__dirname, "./template.hbs");

async function fetchSwagger(url: string): Promise<any> {
  return new Promise((resolve, reject) => {
    const agent = new https.Agent({
      rejectUnauthorized: false, // Allow self-signed certificates
    });

    https.get(url, { agent }, (res) => {
      let data = "";
      
      res.on("data", (chunk) => {
        data += chunk;
      });
      
      res.on("end", () => {
        try {
          resolve(JSON.parse(data));
        } catch (error) {
          reject(new Error(`Failed to parse JSON: ${error}`));
        }
      });
    }).on("error", (error) => {
      reject(new Error(`Failed to fetch from ${url}: ${error.message}`));
    });
  });
}

async function generate() {
  try {
    console.log(`Loading OpenAPI spec from: ${source}`);
    
    const swaggerDoc = isUrl
      ? await fetchSwagger(source)
      : JSON.parse(readFileSync(source, "utf-8"));

    await generateZodClientFromOpenAPI({
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
    });

    console.log("client.ts generated successfully");
  } catch (error) {
    console.error("Error generating client:", error);
    process.exit(1);
  }
}

generate();