# DevCrystal TaskForge

Terraria mod for MCP server integration - where task management meets crystal magic! 🔮⚒️

## Overview

DevCrystal TaskForge transforms your Terraria world into a living, interactive interface for your development workflow. By integrating with MCP (Model Context Protocol) servers, this mod brings task management, project tracking, and AI agent interaction directly into your favorite sandbox game.

**Never leave your creative flow to check a todo list again.**

## Core Concept

- **Tasks as Enchanted Items** - Your todos become craftable/discoverable items in your inventory
- **AI Agents as NPCs** - Each AI agent becomes a unique character you can interact with
- **Projects as Buildable Structures** - Major projects manifest as buildings that grow with completion
- **Spell Casting = MCP Actions** - Magic items trigger real development actions
- **Real-time World Events** - Your development activity creates visual effects and events in-game

## Features

### 🔮 Task Management
- **Task Crystals**: Each todo becomes a colored crystal item (priority determines rarity)
- **Completion Rituals**: Right-click items to mark tasks complete with magical effects
- **Task Meteors**: New urgent tasks fall from the sky as meteors containing task crystals
- **Progress Visualization**: Building structures that represent project completion percentage

### 🧙‍♂️ AI Agent Integration
- **Agent NPCs**: Each MCP agent becomes a unique NPC with personality and specialized dialogue
- **Dynamic Behavior**: NPCs move and react based on system load and task states
- **Conversation Commands**: Natural language task creation through NPC dialogue
- **Status Updates**: Agents provide real-time updates on their current work

### ⚡ MCP Actions as Spells
- **Task Creation Spell**: Opens elegant in-game UI for creating new tasks
- **Project Status Divination**: Magical overview of all active projects
- **Agent Summoning**: Call specific AI agents to your location
- **Productivity Enchantments**: Buffs and effects based on completion streaks

### 🏰 World Integration
- **Project Sanctuaries**: Each major project gets its own dedicated building/area
- **Persistent State**: Your world saves and remembers all development context
- **Team Collaboration**: Multi-player support for shared development worlds
- **Achievement System**: Unlock new spells and abilities through development milestones

## Technical Architecture

### MCP Integration
- HTTP client for REST API communication with localhost MCP servers
- Real-time MQTT/WebSocket subscriptions for live updates
- JSON serialization for seamless data exchange
- Local-only operation for security and performance

### Terraria Modding
- Built on tModLoader framework
- Custom UI overlays using ModUI system
- Persistent world data storage
- Network synchronization for multiplayer

### Supported MCP Servers
- **Omnispindle**: Todo/task management with MongoDB backend
- **SwarmDesk**: Multi-agent coordination and communication
- **Custom Servers**: Extensible architecture for any MCP-compliant server

## Installation

1. Install [tModLoader](https://tmodloader.net/)
2. Clone this repository to your tModLoader Mods folder
3. Build and enable the mod in tModLoader
4. Configure your MCP server endpoints in the mod settings
5. Enter your world and begin forging tasks with crystal magic!

## Configuration

```json
{
  "mcp_servers": {
    "omnispindle": {
      "host": "localhost",
      "port": 8000,
      "endpoints": {
        "tasks": "/api/tasks",
        "projects": "/api/projects"
      }
    }
  },
  "ui_settings": {
    "task_meteor_frequency": "medium",
    "npc_interaction_mode": "advanced",
    "spell_animations": true
  }
}
```

## Development Roadmap

### Phase 1: Foundation
- [ ] Basic HTTP MCP client implementation
- [ ] Task item creation and management
- [ ] Simple UI overlays for task operations
- [ ] Core spell system (create/complete tasks)

### Phase 2: Rich Integration
- [ ] AI Agent NPCs with dialogue systems
- [ ] Real-time MQTT integration for live updates
- [ ] Project building/structure system
- [ ] Advanced spell repertoire

### Phase 3: World Integration
- [ ] Persistent world state management
- [ ] Team collaboration features
- [ ] Achievement and progression systems
- [ ] Custom events and celebrations

### Phase 4: Ecosystem Expansion
- [ ] Support for additional MCP server types
- [ ] Plugin architecture for custom behaviors
- [ ] Mobile companion app integration
- [ ] Analytics and productivity insights

## Contributing

This is part of the Madness Engineering ecosystem. We welcome contributions that push the boundaries of what development interfaces can be.

**Mad Tinkers welcome.** 🔧⚡

## License

MIT License - Build upon this crystal foundation and forge your own magical development experiences.

---

*"Why Alt+Tab between your game and your todo list when you can manage tasks while building castles?"*
