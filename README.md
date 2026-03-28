# DevCrystal TaskForge

> *Why alt-tab to your todo list when you can pull a glowing crystal from your inventory and hurl it at a wall?*

A tModLoader mod for Terraria that transforms your Omnispindle task backlog into in-world items, spells, and lore. Your development workflow lives inside the game now. Tasks fall from the sky as meteors. Urgent work glows purple in your hotbar. Completing a task detonates a particle storm and syncs back to the server.

The lab and the sandbox are the same place.

---

## What's Built

### Items

| Item | Rarity | What It Does |
|------|--------|-------------|
| **Task Crystal** | Priority-mapped | A todo in physical form. Hover for description, project, and priority. Right-click to complete — triggers visual effects, plays a priority-scaled sound, and sends `PUT /api/todos/{id}/complete` to Omnispindle. The crystal then vanishes. |
| **TaskForge Wand** | Green | Casts the task creation spell. Opens the chat command prompt for `/createtask`. Recipe: Mana Crystal + 3 Fallen Stars + Diamond @ Anvil. |
| **Divination Crystal Lens** | Blue | Scans your inventory for task crystals, creates a glow effect around matching ones. Hooks into `/searchcrystals` for full-text search against both local inventory and the MCP server. |
| **Dev Wisdom Codex** | Orange | The Knowledge Grimoire. Records lessons learned into the Omnispindle lessons system via `/addlesson`. Pulses with a golden sine-wave glow in inventory. Powered by developer tears and coffee. |

### Chat Commands

```
/createtask [priority] [project] [description]
    Creates a task crystal and POSTs to Omnispindle if connected.
    Priority: low | medium | high | urgent | critical
    Project:  madness_interactive | omnispindle | terraria | devcrystal | inventorium
    Example:  /createtask high omnispindle Fix the MQTT reconnect loop

/listtasks
    Scans your inventory and lists all task crystals with priority-colored output.

/searchcrystals [query]
    Searches inventory crystals by description, project, or priority.
    Also queries GET /api/todos/search?query=... on the MCP server.
    Matching crystals glow.

/addlesson [language] [topic] [lesson]
    Records a lesson to Omnispindle's lessons system.
    Languages: csharp, python, javascript, rust, general
```

### Priority System

Every task crystal carries its priority as rarity, color, and sound:

| Priority | Rarity | Color | Completion Sound |
|----------|--------|-------|-----------------|
| Low | White | Light gray | Gentle chime |
| Medium | Blue | Light blue | Standard |
| High | Orange | Orange | Standard |
| Urgent | Red | Red | Sharp |
| Critical | Purple | Purple | Epic |

Particle counts scale with urgency. A critical task completion is an event.

---

## Omnispindle Integration

The mod runs an `MCPClient` that speaks directly to the Omnispindle HTTP API:

```
GET  /health                        — connection check on load
POST /api/todos                     — create task (from /createtask)
PUT  /api/todos/{id}/complete       — complete task (from crystal right-click)
GET  /api/todos/search?query=...    — search tasks (from /searchcrystals)
GET  /api/projects                  — list projects
```

The client initializes after `PostSetupContent()` and tests the connection asynchronously. If Omnispindle isn't running, every operation degrades gracefully — task crystals still work locally, completions are logged, syncs are attempted and reported. The game never stalls on a network call.

Configuration lives in the tModLoader mod config UI (client-side):

```
MCP Server Host: localhost
MCP Server Port: 8000
Enable MCP:      true

Task Meteors:         true   (urgent tasks rain from the sky)
Meteor Frequency:     30s
Completion Effects:   true
Agent NPCs:           true   (planned — AI agents as world NPCs)
```

---

## Architecture

```
DevCrystalTaskForge.cs        — Mod entry point, MCPClient lifecycle
├── MCPClient                 — HttpClient wrapper, async REST calls, graceful fallback
└── MCPResponse               — Success/failure wrapper

Content/Items/
├── TaskCrystal.cs            — ModItem: todo as inventory item, async completion
├── TaskForgeWand.cs          — ModItem: spell wand, crafting recipe
├── DivinationLens.cs         — ModItem: crystal search + inventory highlight
└── KnowledgeGrimoire.cs      — ModItem: lesson recording, pulsing glow effect

Common/
├── Commands/
│   ├── TaskCommands.cs       — /createtask, /listtasks, /searchcrystals
│   ├── ProjectCommands.cs    — project listing commands
│   └── LessonCommands.cs     — /addlesson and lesson search
└── Config/
    └── ModConfig.cs          — tModLoader client config: host, port, gameplay toggles
```

---

## Installation

1. Install [tModLoader](https://tmodloader.net/) (compatible with Terraria 1.4.x)
2. Clone this repo into your tModLoader Mods folder:
   ```
   ~/.local/share/Terraria/tModLoader/Mods/DevCrystalTaskForge/
   ```
3. Build with `dotnet build` or via tModLoader's in-game build system
4. Enable the mod in the tModLoader mod browser
5. Launch Terraria, open Mod Configuration, set your Omnispindle host/port
6. Enter a world and forge your first crystal

---

## Roadmap

### Phase 1 — Foundation (current)
- [x] HTTP MCP client with graceful fallback
- [x] Task Crystal item — priority rarity, completion effects, MCP sync
- [x] TaskForge Wand — crafting recipe, spell casting
- [x] Divination Lens — inventory scan, crystal highlight, server search
- [x] Knowledge Grimoire — lesson recording interface, pulsing glow
- [x] Chat commands: `/createtask`, `/listtasks`, `/searchcrystals`
- [x] tModLoader config system (host, port, gameplay toggles)

### Phase 2 — Live World
- [ ] Task Meteor events — urgent tasks physically fall from the sky
- [ ] AI Agent NPCs — each Omnispindle agent becomes a named world NPC
- [ ] NPC dialogue — natural language task creation through conversation
- [ ] Real-time updates — subscribe to task changes, world reacts

### Phase 3 — World State
- [ ] Project Sanctuaries — buildings that grow with project completion percentage
- [ ] Persistent world data — development context survives session changes
- [ ] Multiplayer sync — shared development worlds for the whole team
- [ ] Achievement system — unlock spells through completion streaks

### Phase 4 — Ecosystem
- [ ] Additional MCP server support (SwarmDesk, custom servers)
- [ ] Plugin architecture for custom behaviors
- [ ] Cogwyrm2 mobile companion integration
- [ ] Analytics and productivity overlays

---

## Development

Built on the tModLoader framework. Requires .NET 6+ and the tModLoader SDK.

```bash
# Install .NET if needed
./dotnet-install.sh

# Build
dotnet build

# Or use tModLoader's in-game build/reload
```

See `GETTING_STARTED.md` for first-time setup details.

---

## Why

Because the best interfaces are the ones you'd use anyway. Terraria is already where the thinking happens — the building, the exploring, the systems-within-systems. The task backlog shouldn't live in another window. It should be a glowing crystal in your hotbar that you right-click when you're done with it, and it explodes in golden sparks, and somewhere in a MongoDB database a todo flips to `completed`.

The lab is everywhere now.

---

*Part of the [Madness Interactive](https://madnessinteractive.cc) workshop ecosystem.*
*Connects to: Omnispindle (task management) | SwarmDesk (agent coordination)*
