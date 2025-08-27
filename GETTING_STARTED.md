# Getting Started with DevCrystal TaskForge

## Quick Setup Guide

### 1. Installation Prerequisites
- ✅ tModLoader installed via Steam
- ✅ Running on macOS
- ✅ Steam integration active

### 2. Build the Mod

1. **Copy to tModLoader Directory**
   ```bash
   # Find your tModLoader mods folder (usually in Steam userdata)
   # Copy this entire project folder to:
   ~/Library/Application Support/Steam/steamapps/common/tModLoader/ModSources/DevCrystalTaskForge
   ```

2. **Open tModLoader**
   - Launch through Steam
   - Enable Developer Mode in settings
   - Go to Mod Sources → DevCrystalTaskForge → Build

3. **Enable the Mod**
   - Go to Mods menu
   - Enable "DevCrystal TaskForge"
   - Reload

### 3. First Steps

1. **Create a World**
   - Create or load a Terraria world
   - The mod will initialize automatically

2. **Craft Your First Tools**
   ```
   Task Forge Wand Recipe:
   - 1 Mana Crystal
   - 3 Fallen Stars  
   - 1 Diamond
   - Craft at Anvil
   ```

3. **Basic Commands**
   ```
   /createtask Build awesome mod features
   /listtasks
   ```

### 4. MCP Integration Setup

1. **Configure MCP Server**
   - Open Mod Configuration in-game
   - Set MCP Host/Port (default: localhost:8000)
   - Enable MCP Integration

2. **Test Connection**
   - Check console logs for connection status
   - Green message = successful connection
   - Yellow warning = connection failed (offline mode)

### 5. Basic Workflow

1. **Create Tasks**: Use `/createtask [description]` or Task Forge Wand
2. **Manage Tasks**: Task crystals appear in inventory with priority colors
3. **Complete Tasks**: Right-click task crystals for magical completion
4. **Track Progress**: Use `/listtasks` to see current workload

## Next Steps

- **Add Icon**: Create an 80x80 icon.png file for the mod
- **Test Build**: Verify compilation works in tModLoader
- **MCP Integration**: Connect to your existing MCP servers
- **Customize**: Adjust settings via in-game configuration

## Troubleshooting

**Build Errors**: Check that all dependencies are properly referenced
**MCP Connection**: Verify server is running and accessible
**Missing Items**: Ensure localization files are properly formatted

Ready to forge some crystal magic! 🔮⚒️