#!/usr/bin/env node
/**
 * PMDO MCP Server
 *
 * Provides tools for Claude to interact with PMD: Origins game data:
 * - Query game data (monsters, skills, items, zones, etc.)
 * - Search and explore the codebase with tree-sitter AST parsing
 * - Browse data categories with XML doc extraction
 * - Generate spawn entries and zone configurations
 *
 * Uses tree-sitter for proper AST-based C# parsing.
 */

import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";
import * as fs from "fs";
import * as path from "path";
import { fileURLToPath } from "url";
import Parser from "web-tree-sitter";
type Language = Parser.Language;
type SyntaxNode = Parser.SyntaxNode;

// Get __dirname equivalent for ES modules
const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// =============================================================================
// PROJECT PATHS
// =============================================================================

function findProjectRoot(): string {
  const candidates = [
    path.resolve(__dirname, "../.."),
    path.resolve(__dirname, ".."),
    path.resolve(process.cwd()),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(path.join(dir, "PMDOData.sln")) ||
        fs.existsSync(path.join(dir, "DataGenerator"))) {
      return dir;
    }
  }

  return process.cwd();
}

function findDataGeneratorDir(): string {
  const root = findProjectRoot();
  return path.join(root, "DataGenerator/Data");
}

function findDocsDir(): string {
  const root = findProjectRoot();
  const candidates = [
    path.join(root, "docs/claude"),
    path.join(root, "docs"),
  ];

  for (const dir of candidates) {
    if (fs.existsSync(dir)) {
      return dir;
    }
  }

  return path.join(root, "docs");
}

function findDumpAssetDir(): string {
  const root = findProjectRoot();
  return path.join(root, "DumpAsset/Data");
}

export const PROJECT_ROOT = findProjectRoot();
const DATA_GEN_DIR = findDataGeneratorDir();
const DOCS_DIR = findDocsDir();
const DUMP_ASSET_DIR = findDumpAssetDir();

// Map data categories to DumpAsset folder names
const DUMP_ASSET_FOLDERS: Record<string, string> = {
  monsters: "Monster",
  items: "Item",
  skills: "Skill",
  zones: "Zone",
  intrinsics: "Intrinsic",
  statuses: "Status",
  elements: "Element"
};

// =============================================================================
// TREE-SITTER INITIALIZATION
// =============================================================================

let csharpParser: Parser | null = null;
let csharpLanguage: Language | null = null;

async function initializeParser(): Promise<void> {
  if (csharpParser) return;

  console.error("[MCP DEBUG] Initializing parser...");
  console.error(`[MCP DEBUG] __dirname: ${__dirname}`);
  console.error(`[MCP DEBUG] PROJECT_ROOT: ${PROJECT_ROOT}`);
  console.error(`[MCP DEBUG] DATA_GEN_DIR: ${DATA_GEN_DIR}`);
  console.error(`[MCP DEBUG] DATA_GEN_DIR exists: ${fs.existsSync(DATA_GEN_DIR)}`);

  await Parser.init();
  csharpParser = new Parser();

  // Load C# grammar from node_modules
  const wasmCandidates = [
    path.resolve(__dirname, "../node_modules/tree-sitter-c-sharp/tree-sitter-c_sharp.wasm"),
    path.resolve(__dirname, "../../node_modules/tree-sitter-c-sharp/tree-sitter-c_sharp.wasm"),
    path.resolve(process.cwd(), "mcp-server/node_modules/tree-sitter-c-sharp/tree-sitter-c_sharp.wasm"),
  ];

  let wasmPath: string | null = null;
  for (const candidate of wasmCandidates) {
    if (fs.existsSync(candidate)) {
      wasmPath = candidate;
      break;
    }
  }

  if (!wasmPath) {
    throw new Error("Could not find tree-sitter-c_sharp.wasm");
  }

  console.error(`[MCP DEBUG] WASM path: ${wasmPath}`);
  csharpLanguage = await Parser.Language.load(wasmPath);
  csharpParser.setLanguage(csharpLanguage);
  console.error("[MCP DEBUG] Parser initialized successfully");
}

// =============================================================================
// TREE-SITTER HELPERS
// =============================================================================

function findNodesByType(node: SyntaxNode, type: string, results: SyntaxNode[] = []): SyntaxNode[] {
  if (node.type === type) results.push(node);
  for (let i = 0; i < node.childCount; i++) {
    const child = node.child(i);
    if (child) findNodesByType(child, type, results);
  }
  return results;
}

function getDocComments(node: SyntaxNode): string {
  const comments: string[] = [];
  let prev = node.previousSibling;

  while (prev && prev.type === "comment") {
    comments.unshift(prev.text);
    prev = prev.previousSibling;
  }

  return comments.join("\n");
}

function parseDocComment(docText: string): { summary: string; remarks: string; inheritdoc: boolean } {
  const summaryMatch = docText.match(/<summary>\s*([\s\S]*?)\s*<\/summary>/);
  const summary = summaryMatch
    ? summaryMatch[1].replace(/^\s*\/\/\/\s*/gm, "").trim()
    : "";

  const remarksMatch = docText.match(/<remarks>\s*([\s\S]*?)\s*<\/remarks>/);
  const remarks = remarksMatch
    ? remarksMatch[1].replace(/^\s*\/\/\/\s*/gm, "").trim()
    : "";

  const inheritdoc = docText.includes("<inheritdoc");

  return { summary, remarks, inheritdoc };
}

function getFieldText(node: SyntaxNode, fieldName: string): string {
  const field = node.childForFieldName(fieldName);
  return field ? field.text : "";
}

// =============================================================================
// DATA CATEGORIES (PMDO-specific)
// =============================================================================

const DATA_CATEGORIES = {
  monsters: {
    files: ["MonsterInfo.cs"],
    description: "Pokemon species data - stats, types, abilities, evolutions",
    searchPatterns: ["MonsterData", "MonsterFormData", "DexColor", "BodyShape"]
  },
  items: {
    files: ["ItemInfo.cs"],
    description: "Game items - consumables, held items, TMs, orbs, exclusives",
    searchPatterns: ["ItemData", "GetItemData"]
  },
  skills: {
    files: ["Skills/SkillInfo.cs", "Skills/SkillsPMD.cs", "Skills/SkillsGen5Plus.cs"],
    description: "Pokemon moves/skills - power, accuracy, effects, animations",
    searchPatterns: ["SkillData", "GetSkillData"]
  },
  zones: {
    files: [
      "Zones/ZoneInfo.cs",
      "Zones/ZoneInfoHelpers.cs",
      "Zones/ZoneInfoTables.cs",
      "Zones/ZoneInfoPostgame.cs",
      "Zones/ZoneInfoOptional.cs",
      "Zones/ZoneInfoChallenge.cs",
      "Zones/ZoneInfoRogue.cs",
      "Zones/ZoneInfoBase.cs"
    ],
    description: "Dungeon zones - floor layouts, spawn tables, item pools",
    searchPatterns: ["ZoneData", "GetZoneData", "FillZone", "GetTeamMob"]
  },
  intrinsics: {
    files: ["IntrinsicInfo.cs"],
    description: "Pokemon abilities/intrinsics",
    searchPatterns: ["IntrinsicData", "GetIntrinsicData"]
  },
  statuses: {
    files: ["StatusInfo.cs"],
    description: "Status conditions - poison, sleep, stat changes",
    searchPatterns: ["StatusData", "GetStatusData"]
  },
  elements: {
    files: ["ElementInfo.cs"],
    description: "Type chart and element definitions",
    searchPatterns: ["ElementData", "GetElementData"]
  }
} as const;

type DataCategory = keyof typeof DATA_CATEGORIES;

// =============================================================================
// DATA ENTRY PARSING
// =============================================================================

interface DataEntry {
  index: number;
  id: string;
  name: string;
  description: string;
  category: DataCategory;
  properties: Record<string, string>;
  filePath: string;
  lineNumber: number;
}

interface ClassDoc {
  name: string;
  namespace: string;
  baseClass: string;
  summary: string;
  remarks: string;
  properties: Array<{ name: string; type: string; summary: string }>;
  methods: Array<{ name: string; signature: string; summary: string }>;
  filePath: string;
  isPartial: boolean;
}

async function parseClassFile(filePath: string): Promise<ClassDoc[]> {
  try {
    await initializeParser();
    if (!csharpParser) return [];

    let content = fs.readFileSync(filePath, "utf-8");

    // Strip UTF-8 BOM if present
    if (content.charCodeAt(0) === 0xFEFF) {
      content = content.slice(1);
    }

    const tree = csharpParser.parse(content);
    if (!tree) return [];

    const results: ClassDoc[] = [];

    // Extract namespace
    const namespaceDecls = findNodesByType(tree.rootNode, "namespace_declaration");
    let namespace = "";
    if (namespaceDecls.length > 0) {
      namespace = getFieldText(namespaceDecls[0], "name");
    }
    // Also check for file-scoped namespace
    const fileScopedNs = findNodesByType(tree.rootNode, "file_scoped_namespace_declaration");
    if (fileScopedNs.length > 0) {
      namespace = getFieldText(fileScopedNs[0], "name");
    }

    // Find all class declarations
    const classDecls = findNodesByType(tree.rootNode, "class_declaration");

    for (const classNode of classDecls) {
      const className = getFieldText(classNode, "name");
      if (!className) continue;

      // Check if partial
      let isPartial = false;
      for (let i = 0; i < classNode.childCount; i++) {
        const child = classNode.child(i);
        if (child?.type === "modifier" && child.text === "partial") {
          isPartial = true;
          break;
        }
      }

      // Get base class from base_list
      const baseLists = findNodesByType(classNode, "base_list");
      let baseClass = "";
      if (baseLists.length > 0) {
        const baseList = baseLists[0];
        for (let i = 0; i < baseList.childCount; i++) {
          const child = baseList.child(i);
          if (child && child.type !== ":") {
            baseClass = child.text.split(",")[0].trim();
            break;
          }
        }
      }

      // Get doc comments for the class
      const classDoc = getDocComments(classNode);
      const { summary, remarks } = parseDocComment(classDoc);

      // Extract fields
      const properties: ClassDoc["properties"] = [];
      const fieldDecls = findNodesByType(classNode, "field_declaration");

      for (const field of fieldDecls) {
        // Check if public
        let isPublic = false;
        for (let i = 0; i < field.childCount; i++) {
          const child = field.child(i);
          if (child?.type === "modifier" && child.text === "public") {
            isPublic = true;
            break;
          }
        }
        if (!isPublic) continue;

        const fieldDoc = getDocComments(field);
        const { summary: fieldSummary } = parseDocComment(fieldDoc);

        const varDecl = findNodesByType(field, "variable_declaration")[0];
        let fieldType = "unknown";
        if (varDecl) {
          const typeNode = varDecl.childForFieldName("type");
          if (typeNode) {
            fieldType = typeNode.text;
          } else {
            for (let i = 0; i < varDecl.childCount; i++) {
              const child = varDecl.child(i);
              if (child && child.type !== "variable_declarator" && child.type !== "," && child.type !== ";") {
                fieldType = child.text;
                break;
              }
            }
          }
        }

        const variableDeclarators = findNodesByType(field, "variable_declarator");
        for (const declarator of variableDeclarators) {
          const varName = getFieldText(declarator, "name") || declarator.text.split("=")[0].trim();
          properties.push({
            name: varName,
            type: fieldType,
            summary: fieldSummary || ""
          });
        }
      }

      // Extract methods
      const methods: ClassDoc["methods"] = [];
      const methodDecls = findNodesByType(classNode, "method_declaration");

      for (const method of methodDecls) {
        // Check if public
        let isPublic = false;
        for (let i = 0; i < method.childCount; i++) {
          const child = method.child(i);
          if (child?.type === "modifier" && child.text === "public") {
            isPublic = true;
            break;
          }
        }
        if (!isPublic) continue;

        const methodDoc = getDocComments(method);
        const { summary: methodSummary, inheritdoc } = parseDocComment(methodDoc);

        const methodName = getFieldText(method, "name");
        const returnType = getFieldText(method, "type") || "void";
        const paramsNode = method.childForFieldName("parameters");
        const params = paramsNode ? paramsNode.text : "()";

        const signature = `${returnType} ${methodName}${params}`;

        methods.push({
          name: methodName,
          signature,
          summary: inheritdoc ? "(inherited documentation)" : (methodSummary || "")
        });
      }

      results.push({
        name: className,
        namespace,
        baseClass,
        summary,
        remarks,
        properties,
        methods,
        filePath,
        isPartial
      });
    }

    return results;
  } catch (err) {
    console.error(`[MCP DEBUG] Error parsing ${filePath}:`, err);
    return [];
  }
}

async function findClassesInCategory(category: DataCategory): Promise<ClassDoc[]> {
  const categoryInfo = DATA_CATEGORIES[category];
  const classes: ClassDoc[] = [];

  for (const file of categoryInfo.files) {
    const filePath = path.join(DATA_GEN_DIR, file);
    if (fs.existsSync(filePath)) {
      const fileDocs = await parseClassFile(filePath);
      classes.push(...fileDocs);
    }
  }

  return classes;
}

// =============================================================================
// DATA ENTRY EXTRACTION
// =============================================================================

interface GameDataEntry {
  index: number;
  id: string;
  name: string;
  rawName: string;
  description: string;
  sprite?: string;
  price?: number;
  isUnreleased: boolean;
  properties: Record<string, string>;
  lineNumber: number;
}

async function extractDataEntries(category: DataCategory): Promise<GameDataEntry[]> {
  await initializeParser();
  if (!csharpParser) return [];

  const entries: GameDataEntry[] = [];
  const categoryInfo = DATA_CATEGORIES[category];

  for (const file of categoryInfo.files) {
    const filePath = path.join(DATA_GEN_DIR, file);
    if (!fs.existsSync(filePath)) continue;

    let content = fs.readFileSync(filePath, "utf-8");
    if (content.charCodeAt(0) === 0xFEFF) {
      content = content.slice(1);
    }

    const lines = content.split("\n");

    // Parse item/skill/monster entries based on pattern matching
    // Look for patterns like: item.Name = new LocalText("Apple");
    let currentIndex = -1;
    let currentEntry: Partial<GameDataEntry> | null = null;

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];

      // Detect index assignment: if (ii == 0) or else if (ii == 1)
      const indexMatch = line.match(/(?:if|else if)\s*\(\s*ii\s*==\s*(\d+)\s*\)/);
      if (indexMatch) {
        // Save previous entry
        if (currentEntry && currentEntry.name) {
          entries.push(currentEntry as GameDataEntry);
        }

        currentIndex = parseInt(indexMatch[1], 10);
        currentEntry = {
          index: currentIndex,
          id: "",
          name: "",
          rawName: "",
          description: "",
          isUnreleased: false,
          properties: {},
          lineNumber: i + 1
        };
        continue;
      }

      if (!currentEntry) continue;

      // Extract name: item.Name = new LocalText("Apple");
      const nameMatch = line.match(/\.Name\s*=\s*new\s+LocalText\s*\(\s*"([^"]+)"\s*\)/);
      if (nameMatch) {
        currentEntry.rawName = nameMatch[1];
        // Check for unreleased marker (**)
        if (nameMatch[1].startsWith("**")) {
          currentEntry.isUnreleased = true;
          currentEntry.name = nameMatch[1].substring(2);
        } else if (nameMatch[1].startsWith("-") || nameMatch[1].startsWith("=")) {
          currentEntry.name = nameMatch[1].substring(1);
        } else {
          currentEntry.name = nameMatch[1];
        }
        // Generate ID from name
        currentEntry.id = currentEntry.name.toLowerCase().replace(/[^a-z0-9]+/g, "_").replace(/^_|_$/g, "");
      }

      // Extract description
      const descMatch = line.match(/\.Desc\s*=\s*new\s+LocalText\s*\(\s*"([^"]+)"\s*\)/);
      if (descMatch) {
        currentEntry.description = descMatch[1];
      }

      // Extract sprite
      const spriteMatch = line.match(/\.Sprite\s*=\s*"([^"]+)"/);
      if (spriteMatch) {
        currentEntry.sprite = spriteMatch[1];
      }

      // Extract price
      const priceMatch = line.match(/\.Price\s*=\s*(\d+)/);
      if (priceMatch) {
        currentEntry.price = parseInt(priceMatch[1], 10);
      }

      // Extract fileName assignment
      const fileNameMatch = line.match(/fileName\s*=\s*"([^"]+)"/);
      if (fileNameMatch) {
        currentEntry.id = fileNameMatch[1];
      }
    }

    // Save last entry
    if (currentEntry && currentEntry.name) {
      entries.push(currentEntry as GameDataEntry);
    }
  }

  return entries;
}

// =============================================================================
// DUMP ASSET EXTRACTION (for monsters, elements, etc.)
// =============================================================================

async function extractDumpAssetEntries(category: DataCategory): Promise<GameDataEntry[]> {
  const folderName = DUMP_ASSET_FOLDERS[category];
  if (!folderName) return [];

  const indexPath = path.join(DUMP_ASSET_DIR, folderName, "index.idx");
  if (!fs.existsSync(indexPath)) {
    console.error(`[MCP DEBUG] Index file not found: ${indexPath}`);
    return [];
  }

  try {
    let content = fs.readFileSync(indexPath, "utf-8");
    // Strip UTF-8 BOM if present
    if (content.charCodeAt(0) === 0xFEFF) {
      content = content.slice(1);
    }

    const data = JSON.parse(content);
    const entries: GameDataEntry[] = [];

    if (!data.Object || typeof data.Object !== "object") {
      console.error(`[MCP DEBUG] Invalid index structure in ${indexPath}`);
      return [];
    }

    // The Object is a dictionary: { "id": { Name: {...}, Released: bool, SortOrder: num, ... } }
    let index = 0;
    for (const [id, entry] of Object.entries(data.Object)) {
      if (id === "$type") continue; // Skip the type annotation

      const entryObj = entry as Record<string, unknown>;
      const nameObj = entryObj.Name as Record<string, unknown> | undefined;
      const name = nameObj?.DefaultText as string || id;
      const released = entryObj.Released as boolean ?? true;
      const sortOrder = entryObj.SortOrder as number ?? index;

      // For monsters, get description from Title if available
      let description = "";
      if (category === "monsters") {
        // Read individual JSON file to get Title (description)
        const monsterFile = path.join(DUMP_ASSET_DIR, folderName, `${id}.json`);
        if (fs.existsSync(monsterFile)) {
          try {
            let monsterContent = fs.readFileSync(monsterFile, "utf-8");
            if (monsterContent.charCodeAt(0) === 0xFEFF) {
              monsterContent = monsterContent.slice(1);
            }
            const monsterData = JSON.parse(monsterContent);
            const titleObj = monsterData.Object?.Title as Record<string, unknown> | undefined;
            description = titleObj?.DefaultText as string || "";
          } catch {
            // Ignore parse errors for individual files
          }
        }
      }

      entries.push({
        index: sortOrder,
        id,
        name,
        rawName: name,
        description,
        isUnreleased: !released,
        properties: {},
        lineNumber: 0
      });

      index++;
    }

    // Sort by SortOrder/index
    entries.sort((a, b) => a.index - b.index);

    return entries;
  } catch (err) {
    console.error(`[MCP DEBUG] Error parsing ${indexPath}:`, err);
    return [];
  }
}

// Categories that should use DumpAsset instead of C# source parsing
const DUMP_ASSET_CATEGORIES: Set<DataCategory> = new Set(["monsters", "elements"]);

// Unified extraction function that chooses the right approach
async function getDataEntries(category: DataCategory): Promise<GameDataEntry[]> {
  if (DUMP_ASSET_CATEGORIES.has(category)) {
    return extractDumpAssetEntries(category);
  }
  return extractDataEntries(category);
}

// =============================================================================
// SEARCH HELPERS
// =============================================================================

function levenshteinDistance(a: string, b: string): number {
  const matrix: number[][] = [];

  for (let i = 0; i <= b.length; i++) {
    matrix[i] = [i];
  }
  for (let j = 0; j <= a.length; j++) {
    matrix[0][j] = j;
  }

  for (let i = 1; i <= b.length; i++) {
    for (let j = 1; j <= a.length; j++) {
      if (b.charAt(i - 1) === a.charAt(j - 1)) {
        matrix[i][j] = matrix[i - 1][j - 1];
      } else {
        matrix[i][j] = Math.min(
          matrix[i - 1][j - 1] + 1,
          matrix[i][j - 1] + 1,
          matrix[i - 1][j] + 1
        );
      }
    }
  }

  return matrix[b.length][a.length];
}

// =============================================================================
// MCP SERVER SETUP
// =============================================================================

const server = new McpServer({
  name: "pmdo-mcp-server",
  version: "1.0.0"
});

// =============================================================================
// TOOLS - Data browsing and search
// =============================================================================

server.tool(
  "pmdo_search",
  `Search for PMDO game data entries across all categories by name.

Searches items, skills, monsters, zones, etc. Returns matches ranked by relevance.
Use this when looking for specific game content.

Categories: ${Object.keys(DATA_CATEGORIES).join(", ")}`,
  {
    query: z.string()
      .min(1)
      .describe("Search query (e.g., 'apple', 'thunderbolt', 'pikachu')"),
    category: z.enum(Object.keys(DATA_CATEGORIES) as [DataCategory, ...DataCategory[]])
      .optional()
      .describe("Optional category to limit search"),
    limit: z.number()
      .min(1)
      .max(50)
      .default(20)
      .describe("Maximum results to return"),
    include_unreleased: z.boolean()
      .default(false)
      .describe("Include unreleased/WIP entries marked with **")
  },
  async ({ query, category, limit, include_unreleased }) => {
    const queryLower = query.toLowerCase();
    const results: Array<{
      name: string;
      id: string;
      index: number;
      category: DataCategory;
      description: string;
      isUnreleased: boolean;
      score: number;
    }> = [];

    const categoriesToSearch = category ? [category] : Object.keys(DATA_CATEGORIES) as DataCategory[];

    for (const cat of categoriesToSearch) {
      const entries = await getDataEntries(cat);

      for (const entry of entries) {
        if (!include_unreleased && entry.isUnreleased) continue;

        const nameLower = entry.name.toLowerCase();
        const descLower = (entry.description || "").toLowerCase();
        const idLower = entry.id.toLowerCase();

        let score = 1000;

        if (nameLower === queryLower || idLower === queryLower) {
          score = 0;
        } else if (nameLower.startsWith(queryLower) || idLower.startsWith(queryLower)) {
          score = 10;
        } else if (nameLower.includes(queryLower) || idLower.includes(queryLower)) {
          score = 20;
        } else if (descLower.includes(queryLower)) {
          score = 50;
        } else {
          const distance = levenshteinDistance(queryLower, nameLower);
          if (distance <= 3) {
            score = 100 + distance;
          } else {
            continue;
          }
        }

        results.push({
          name: entry.name,
          id: entry.id,
          index: entry.index,
          category: cat,
          description: entry.description || "(no description)",
          isUnreleased: entry.isUnreleased,
          score
        });
      }
    }

    const sorted = results.sort((a, b) => a.score - b.score).slice(0, limit);

    if (sorted.length === 0) {
      return {
        content: [{
          type: "text",
          text: `No entries found matching '${query}'. Try a different search term or use pmdo_list_data to browse by category.`
        }]
      };
    }

    const lines = [
      `# Search Results: "${query}"`,
      "",
      `Found ${sorted.length} matching entries:`,
      "",
      "| Name | ID | Index | Category | Description |",
      "|------|-----|-------|----------|-------------|"
    ];

    for (const result of sorted) {
      const desc = result.description.length > 40
        ? result.description.substring(0, 37) + "..."
        : result.description;
      const unreleased = result.isUnreleased ? " (WIP)" : "";
      lines.push(`| ${result.name}${unreleased} | ${result.id} | ${result.index} | ${result.category} | ${desc} |`);
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

server.tool(
  "pmdo_list_data",
  `List PMDO game data entries by category.

Categories:
${Object.entries(DATA_CATEGORIES).map(([k, v]) => `- ${k}: ${v.description}`).join("\n")}`,
  {
    category: z.enum(Object.keys(DATA_CATEGORIES) as [DataCategory, ...DataCategory[]])
      .describe("Category of data to list"),
    limit: z.number()
      .int()
      .min(1)
      .max(100)
      .default(50)
      .describe("Maximum results to return"),
    offset: z.number()
      .int()
      .min(0)
      .default(0)
      .describe("Number of results to skip for pagination"),
    include_unreleased: z.boolean()
      .default(false)
      .describe("Include unreleased/WIP entries marked with **")
  },
  async ({ category, limit, offset, include_unreleased }) => {
    const entries = await getDataEntries(category);
    const filtered = include_unreleased ? entries : entries.filter(e => !e.isUnreleased);

    const total = filtered.length;
    const paged = filtered.slice(offset, offset + limit);
    const hasMore = offset + limit < total;

    if (paged.length === 0) {
      return {
        content: [{
          type: "text",
          text: `No entries found in category '${category}'.`
        }]
      };
    }

    const lines = [
      `# ${category} Data`,
      "",
      `**Description:** ${DATA_CATEGORIES[category].description}`,
      `**Total:** ${total} entries (showing ${paged.length})`,
      "",
      "| Index | ID | Name | Description |",
      "|-------|-----|------|-------------|"
    ];

    for (const entry of paged) {
      const desc = entry.description
        ? (entry.description.length > 50 ? entry.description.substring(0, 47) + "..." : entry.description)
        : "(no description)";
      const unreleased = entry.isUnreleased ? " (WIP)" : "";
      lines.push(`| ${entry.index} | ${entry.id} | ${entry.name}${unreleased} | ${desc} |`);
    }

    if (hasMore) {
      lines.push("");
      lines.push(`*More results available. Use offset=${offset + limit} to see next page.*`);
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

server.tool(
  "pmdo_get_entry",
  `Get detailed information about a specific PMDO game data entry.

Retrieves full details including all properties from the source code.`,
  {
    category: z.enum(Object.keys(DATA_CATEGORIES) as [DataCategory, ...DataCategory[]])
      .describe("Category of data"),
    id: z.string()
      .min(1)
      .describe("Entry ID or name to look up")
  },
  async ({ category, id }) => {
    const entries = await getDataEntries(category);
    const idLower = id.toLowerCase();

    const entry = entries.find(e =>
      e.id.toLowerCase() === idLower ||
      e.name.toLowerCase() === idLower ||
      e.index.toString() === id
    );

    if (!entry) {
      // Try fuzzy match
      const suggestions = entries
        .map(e => ({ entry: e, score: levenshteinDistance(idLower, e.name.toLowerCase()) }))
        .filter(x => x.score <= 3)
        .sort((a, b) => a.score - b.score)
        .slice(0, 5);

      let errorMsg = `Entry '${id}' not found in ${category}.\n\n`;

      if (suggestions.length > 0) {
        errorMsg += "**Did you mean one of these?**\n\n";
        for (const suggestion of suggestions) {
          errorMsg += `- \`${suggestion.entry.name}\` (${suggestion.entry.id}, index ${suggestion.entry.index})\n`;
        }
      }

      return {
        content: [{ type: "text", text: errorMsg }]
      };
    }

    const lines = [
      `# ${entry.name}`,
      "",
      `**Category:** ${category}`,
      `**ID:** ${entry.id}`,
      `**Index:** ${entry.index}`,
      `**Status:** ${entry.isUnreleased ? "Unreleased/WIP" : "Released"}`,
      ""
    ];

    if (entry.description) {
      lines.push("## Description", "", entry.description, "");
    }

    if (entry.sprite) {
      lines.push(`**Sprite:** ${entry.sprite}`);
    }

    if (entry.price !== undefined) {
      lines.push(`**Price:** ${entry.price}`);
    }

    lines.push("");
    lines.push(`**Source:** DataGenerator/Data/${DATA_CATEGORIES[category].files[0]}:${entry.lineNumber}`);

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

// =============================================================================
// TOOLS - Class documentation
// =============================================================================

server.tool(
  "pmdo_list_classes",
  `List classes in a PMDO data category with their XML documentation.

Returns class names, summaries, and key methods from the source files.`,
  {
    category: z.enum(Object.keys(DATA_CATEGORIES) as [DataCategory, ...DataCategory[]])
      .describe("Category of classes to list")
  },
  async ({ category }) => {
    const classes = await findClassesInCategory(category);
    const categoryInfo = DATA_CATEGORIES[category];

    if (classes.length === 0) {
      return {
        content: [{
          type: "text",
          text: `No classes found in category '${category}'.`
        }]
      };
    }

    const lines = [
      `# ${category} Classes`,
      "",
      `**Description:** ${categoryInfo.description}`,
      `**Files:** ${categoryInfo.files.map(f => `\`${f}\``).join(", ")}`,
      `**Count:** ${classes.length}`,
      ""
    ];

    for (const cls of classes) {
      const partial = cls.isPartial ? " (partial)" : "";
      lines.push(`## ${cls.name}${partial}`);
      if (cls.summary) {
        lines.push(cls.summary);
      }
      if (cls.baseClass) {
        lines.push(`- **Base:** \`${cls.baseClass}\``);
      }
      if (cls.methods.length > 0) {
        const methodNames = cls.methods.slice(0, 5).map(m => m.name).join(", ");
        const more = cls.methods.length > 5 ? `, ... (+${cls.methods.length - 5} more)` : "";
        lines.push(`- **Methods:** ${methodNames}${more}`);
      }
      lines.push("");
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

server.tool(
  "pmdo_get_class_docs",
  `Get detailed XML documentation for a specific PMDO class.

Extracts from C# source files:
- Class summary and remarks
- Public fields and properties
- Public methods with signatures`,
  {
    class_name: z.string()
      .min(1)
      .describe("Name of the class to get documentation for")
  },
  async ({ class_name }) => {
    let foundClass: ClassDoc | null = null;

    for (const category of Object.keys(DATA_CATEGORIES) as DataCategory[]) {
      const classes = await findClassesInCategory(category);
      const found = classes.find(c => c.name.toLowerCase() === class_name.toLowerCase());
      if (found) {
        foundClass = found;
        break;
      }
    }

    if (!foundClass) {
      return {
        content: [{
          type: "text",
          text: `Class '${class_name}' not found. Use pmdo_list_classes to see available classes.`
        }]
      };
    }

    const partial = foundClass.isPartial ? " (partial class)" : "";
    const lines = [
      `# ${foundClass.name}${partial}`,
      "",
      `**Namespace:** \`${foundClass.namespace}\``,
    ];

    if (foundClass.baseClass) {
      lines.push(`**Base Class:** \`${foundClass.baseClass}\``);
    }
    lines.push(`**File:** \`${foundClass.filePath.replace(PROJECT_ROOT, "")}\``);
    lines.push("");

    if (foundClass.summary) {
      lines.push("## Summary", "", foundClass.summary, "");
    }

    if (foundClass.remarks) {
      lines.push("## Remarks", "", foundClass.remarks, "");
    }

    if (foundClass.properties.length > 0) {
      lines.push("## Fields/Properties", "");
      for (const prop of foundClass.properties) {
        lines.push(`### \`${prop.name}\` : \`${prop.type}\``);
        if (prop.summary) lines.push(prop.summary);
        lines.push("");
      }
    }

    if (foundClass.methods.length > 0) {
      lines.push("## Methods", "");
      for (const method of foundClass.methods) {
        lines.push(`### \`${method.signature}\``);
        if (method.summary) lines.push(method.summary);
        lines.push("");
      }
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

// =============================================================================
// TOOLS - Scaffolding
// =============================================================================

// Validate that an ID contains only safe characters for C# code generation
// Returns null if valid, or an error message if invalid
function validateGameId(id: string, fieldName: string): string | null {
  if (!id) return null; // Empty is allowed for optional fields
  // Allow lowercase letters, numbers, and underscores (standard PMDO ID format)
  if (!/^[a-z0-9_]+$/.test(id)) {
    return `${fieldName} contains invalid characters. Only lowercase letters, numbers, and underscores are allowed.`;
  }
  return null;
}

// Sanitize an ID for safe use in generated code (as a fallback)
function sanitizeGameId(id: string): string {
  return id.toLowerCase().replace(/[^a-z0-9_]/g, '_');
}

server.tool(
  "pmdo_scaffold_spawn",
  `Generate a monster spawn entry for a PMDO zone file.

Creates a properly formatted GetTeamMob call for adding monsters to dungeon spawn tables.`,
  {
    species: z.string()
      .min(1)
      .describe("Pokemon species ID (lowercase, e.g., 'pikachu', 'mr_mime')"),
    ability: z.string()
      .default("")
      .describe("Ability ID (empty for default ability)"),
    moves: z.array(z.string())
      .max(4)
      .default([])
      .describe("Move IDs (up to 4)"),
    level: z.number()
      .int()
      .min(1)
      .max(100)
      .describe("Pokemon level"),
    level_variance: z.number()
      .int()
      .min(0)
      .default(2)
      .describe("Level variance (+/-)"),
    tactic: z.enum(["wander_dumb", "wander_normal", "slow_patrol", "weird_tree"])
      .default("wander_dumb")
      .describe("AI behavior tactic"),
    floor_start: z.number()
      .int()
      .min(0)
      .default(0)
      .describe("First floor to spawn on"),
    floor_end: z.number()
      .int()
      .min(1)
      .describe("Last floor to spawn on (exclusive)"),
    weight: z.number()
      .int()
      .min(1)
      .default(10)
      .describe("Spawn weight (higher = more common)")
  },
  async ({ species, ability, moves, level, level_variance, tactic, floor_start, floor_end, weight }) => {
    // Validate floor range
    if (floor_start >= floor_end) {
      return {
        content: [{
          type: "text",
          text: `Error: floor_start (${floor_start}) must be less than floor_end (${floor_end}).`
        }]
      };
    }

    // Validate all IDs for safe characters
    const validationErrors: string[] = [];
    const speciesError = validateGameId(species, "species");
    if (speciesError) validationErrors.push(speciesError);

    const abilityError = validateGameId(ability, "ability");
    if (abilityError) validationErrors.push(abilityError);

    for (let i = 0; i < moves.length; i++) {
      const moveError = validateGameId(moves[i], `moves[${i}]`);
      if (moveError) validationErrors.push(moveError);
    }

    if (validationErrors.length > 0) {
      return {
        content: [{
          type: "text",
          text: `Validation errors:\n${validationErrors.map(e => `- ${e}`).join('\n')}`
        }]
      };
    }

    const moveSlots = [...moves, "", "", "", ""].slice(0, 4);
    // Clamp level range to valid Pokemon level bounds (1-100)
    const minLevel = Math.max(1, level - level_variance);
    const maxLevel = Math.min(100, level + level_variance);
    const levelRange = level_variance > 0
      ? `new RandRange(${minLevel}, ${maxLevel})`
      : `new RandRange(${level})`;

    // Check if species exists in game data (warning only)
    const warnings: string[] = [];
    try {
      const monsters = await getDataEntries("monsters");
      const speciesExists = monsters.some(m => m.id === species);
      if (!speciesExists) {
        warnings.push(`Warning: Species '${species}' not found in game data. Verify the ID is correct.`);
      }
    } catch {
      // Silently ignore lookup errors
    }

    const code = `// ${species.charAt(0).toUpperCase() + species.slice(1)} spawn entry
poolSpawn.Spawns.Add(
    GetTeamMob("${species}", "${ability}", "${moveSlots[0]}", "${moveSlots[1]}", "${moveSlots[2]}", "${moveSlots[3]}",
        ${levelRange}, "${tactic}"),
    new IntRange(${floor_start}, ${floor_end}),
    ${weight}
);`;

    let response = `Generated spawn entry:\n\n\`\`\`csharp\n${code}\n\`\`\`\n\nAdd this to your zone's TeamSpawnZoneStep section.`;
    if (warnings.length > 0) {
      response += `\n\n${warnings.join('\n')}`;
    }

    return {
      content: [{
        type: "text",
        text: response
      }]
    };
  }
);

server.tool(
  "pmdo_scaffold_item_spawn",
  `Generate an item spawn entry for a PMDO zone file.

Creates a properly formatted item spawn for zone item tables.`,
  {
    item_id: z.string()
      .min(1)
      .describe("Item ID (e.g., 'berry_oran', 'food_apple', 'seed_reviver')"),
    floor_start: z.number()
      .int()
      .min(0)
      .default(0)
      .describe("First floor to spawn on"),
    floor_end: z.number()
      .int()
      .min(1)
      .describe("Last floor to spawn on (exclusive)"),
    weight: z.number()
      .int()
      .min(1)
      .default(10)
      .describe("Spawn weight (higher = more common)"),
    category: z.string()
      .default("necessities")
      .describe("Item category (necessities, special, etc.)")
  },
  async ({ item_id, floor_start, floor_end, weight, category }) => {
    // Validate floor range
    if (floor_start >= floor_end) {
      return {
        content: [{
          type: "text",
          text: `Error: floor_start (${floor_start}) must be less than floor_end (${floor_end}).`
        }]
      };
    }

    // Validate IDs for safe characters
    const validationErrors: string[] = [];
    const itemError = validateGameId(item_id, "item_id");
    if (itemError) validationErrors.push(itemError);

    const categoryError = validateGameId(category, "category");
    if (categoryError) validationErrors.push(categoryError);

    if (validationErrors.length > 0) {
      return {
        content: [{
          type: "text",
          text: `Validation errors:\n${validationErrors.map(e => `- ${e}`).join('\n')}`
        }]
      };
    }

    // Check if item exists in game data (warning only)
    const warnings: string[] = [];
    try {
      const items = await getDataEntries("items");
      const itemExists = items.some(i => i.id === item_id);
      if (!itemExists) {
        warnings.push(`Warning: Item '${item_id}' not found in game data. Verify the ID is correct.`);
      }
    } catch {
      // Silently ignore lookup errors
    }

    const code = `// Item spawn: ${item_id}
${category}.Spawns.Add(new InvItem("${item_id}"), new IntRange(${floor_start}, ${floor_end}), ${weight});`;

    let response = `Generated item spawn:\n\n\`\`\`csharp\n${code}\n\`\`\`\n\nAdd this to your zone's ItemSpawnZoneStep section under the appropriate category.`;
    if (warnings.length > 0) {
      response += `\n\n${warnings.join('\n')}`;
    }

    return {
      content: [{
        type: "text",
        text: response
      }]
    };
  }
);

// =============================================================================
// TOOLS - Data type statistics
// =============================================================================

server.tool(
  "pmdo_stats",
  `Get statistics about PMDO game data.

Returns counts for each data category.`,
  {},
  async () => {
    const stats: Record<string, { total: number; released: number; unreleased: number }> = {};

    for (const category of Object.keys(DATA_CATEGORIES) as DataCategory[]) {
      const entries = await getDataEntries(category);
      const released = entries.filter(e => !e.isUnreleased).length;
      const unreleased = entries.filter(e => e.isUnreleased).length;
      stats[category] = { total: entries.length, released, unreleased };
    }

    const lines = [
      "# PMDO Data Statistics",
      "",
      "| Category | Released | Unreleased | Total |",
      "|----------|----------|------------|-------|"
    ];

    for (const [category, data] of Object.entries(stats)) {
      lines.push(`| ${category} | ${data.released} | ${data.unreleased} | ${data.total} |`);
    }

    return {
      content: [{ type: "text", text: lines.join("\n") }]
    };
  }
);

// =============================================================================
// SERVER STARTUP
// =============================================================================

async function main() {
  console.error(`PMDO MCP Server v1.0.0`);
  console.error(`Project root: ${PROJECT_ROOT}`);
  console.error(`Data generator dir: ${DATA_GEN_DIR}`);

  // Initialize parser at startup
  try {
    console.error("Initializing tree-sitter parser...");
    await initializeParser();
    console.error("Parser initialized successfully");
  } catch (err) {
    console.error("Parser initialization failed:", err);
    // Continue without parser - some features won't work
  }

  const transport = new StdioServerTransport();
  await server.connect(transport);
  console.error("MCP server running via stdio");
}

main().catch((error) => {
  console.error("Server error:", error);
  process.exit(1);
});
