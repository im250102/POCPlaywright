import { Server } from "@modelcontextprotocol/sdk/server/index.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import {
  ListToolsRequestSchema,
  CallToolRequestSchema,
} from "@modelcontextprotocol/sdk/types.js";
import { chromium } from "playwright";

const server = new Server(
  {
    name: "playwright-mcp",
    version: "1.0.0",
  },
  {
    capabilities: { tools: {} },
  }
);

server.setRequestHandler(ListToolsRequestSchema, async () => ({
  tools: [
    {
      name: "navigate",
      description: "Navega a una URL y captura el título y contenido",
      inputSchema: {
        type: "object",
        properties: {
          url: { type: "string", description: "URL a navegar" },
        },
        required: ["url"],
      },
    },
    {
      name: "screenshot",
      description: "Toma una captura de pantalla de una URL",
      inputSchema: {
        type: "object",
        properties: {
          url: { type: "string", description: "URL a capturar" },
          selector: {
            type: "string",
            description: "Selector CSS opcional para capturar un elemento específico",
          },
        },
        required: ["url"],
      },
    },
    {
      name: "click_and_wait",
      description: "Hace clic en un elemento y espera navegación",
      inputSchema: {
        type: "object",
        properties: {
          url: { type: "string", description: "URL inicial" },
          selector: { type: "string", description: "Selector CSS del elemento a clickear" },
        },
        required: ["url", "selector"],
      },
    },
    {
      name: "fill_form",
      description: "Llena un campo de formulario",
      inputSchema: {
        type: "object",
        properties: {
          url: { type: "string", description: "URL" },
          selector: { type: "string", description: "Selector CSS del campo" },
          value: { type: "string", description: "Valor a escribir" },
        },
        required: ["url", "selector", "value"],
      },
    },
  ],
}));

server.setRequestHandler(CallToolRequestSchema, async (request) => {
  const { name, arguments: args } = request.params;
  const browser = await chromium.launch({ headless: true });

  try {
    const context = await browser.newContext();
    const page = await context.newPage();

    switch (name) {
      case "navigate": {
        await page.goto(args.url, { waitUntil: "networkidle" });
        return {
          content: [
            {
              type: "text",
              text: JSON.stringify(
                { title: await page.title(), url: page.url() },
                null,
                2
              ),
            },
          ],
        };
      }

      case "screenshot": {
        await page.goto(args.url, { waitUntil: "networkidle" });
        if (args.selector) {
          await page.locator(args.selector).screenshot({ path: "captura.png" });
        } else {
          await page.screenshot({ path: "captura.png", fullPage: true });
        }
        return {
          content: [
            { type: "text", text: `Captura guardada como captura.png` },
          ],
        };
      }

      case "click_and_wait": {
        await page.goto(args.url, { waitUntil: "networkidle" });
        await page.click(args.selector);
        await page.waitForLoadState("networkidle");
        return {
          content: [
            {
              type: "text",
              text: JSON.stringify(
                { title: await page.title(), url: page.url() },
                null,
                2
              ),
            },
          ],
        };
      }

      case "fill_form": {
        await page.goto(args.url, { waitUntil: "networkidle" });
        await page.fill(args.selector, args.value);
        return {
          content: [{ type: "text", text: "Campo llenado exitosamente" }],
        };
      }

      default:
        throw new Error(`Herramienta desconocida: ${name}`);
    }
  } finally {
    await browser.close();
  }
});

async function main() {
  const transport = new StdioServerTransport();
  await server.connect(transport);
}

main().catch((err) => {
  console.error("Error en MCP Playwright:", err);
  process.exit(1);
});
