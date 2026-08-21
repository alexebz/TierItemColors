# Tier Item Colors

A ModShardLauncher mod for **Stoneshard 0.9.4.25** that makes item tier the primary visual language for normal equipment names while preserving a distinct look for Unique items.

## Tier palette

| Tier | Color | Hex | RGB |
| --- | --- | --- | --- |
| T1 | Bone | `#C8C0AF` | `200, 192, 175` |
| T2 | Moss | `#7EA967` | `126, 169, 103` |
| T3 | Steel Blue | `#5590B5` | `85, 144, 181` |
| T4 | Burnished Red | `#C3614C` | `195, 97, 76` |
| T5 | Antique Gold | `#F2AF2E` | `242, 175, 46` |

Normal and cursed equipment use their tier color.

**Unique items keep Stoneshard's vanilla purple (`#8248BC`).**

## Item-state markers

Item state is communicated with small Stoneshard-style sprites in equipment hover tooltips while preserving the selected name color:

- `spr_tic_enchant_minor` — one enchantment
- `spr_tic_enchant_major` — two enchantments
- `spr_tic_cursed` — cursed item

The markers are only shown after an item is identified, so unidentified equipment does not reveal hidden curse or enchantment information.

Markers are intentionally **not drawn on equipment lying on the ground**. Ground loot keeps Stoneshard's normal presentation; state icons appear only in the hover tooltip.

### Hover layout

The source sprites are 16×16 and are drawn at **1.5× UI scale** (24×24 at the base scale). They are vertically centered against the first title line and placed immediately to the left of the centered title with a 4px title gap.

For items that are both cursed and enchanted, the cursed marker is drawn first and the enchantment marker is placed beside it with a small 2px inter-icon gap.

## Visual language

- **Tier color** → progression tier for normal/cursed equipment
- **Purple** → Unique item
- **Enchant I sprite** → one displayed enchantment
- **Enchant II sprite** → two displayed enchantments
- **Cursed sprite** → cursed item

## Implementation

Stoneshard 0.9.4.25 centralizes item-name color selection in `scr_loot_color(id)`. Tier Item Colors hooks that function so the same tier color is used consistently by tooltips, ground labels, shops, logs, and other UI that relies on vanilla loot coloring.

The mod does **not** infer tier from runtime `LVL`. During patching it reads the current `gml_GlobalScript_table_weapons` and `gml_GlobalScript_table_armor` directly from the loaded `vanilla.win`. In both tables, column 2 is the equipment tier. The generated resolver accepts both the table's item name and item id as aliases for runtime `data["idName"]`, which makes it compatible with vanilla equipment instances.

Items whose `quality` is Unique keep vanilla purple instead of receiving a tier color.

Curse state is read from `data["is_cursed"]`. Enchantment markers are derived from the `enchantedAttributesArray` that vanilla already builds for the equipment tooltip.

The marker sprites are packed by MSL from the PNGs in the mod source folder and drawn separately from text, avoiding locale/font glyph limitations.

## Build / MSL discovery

MSL scans the **immediate child folders** of its `ModSources` directory and expects the `.csproj` file to be directly inside that child folder.

Clone this repository so the layout is exactly:

```text
ModShardLauncher/
├── ModShardLauncher.exe
├── ModShardLauncher.dll
├── UndertaleModLib.dll
└── ModSources/
    └── TierItemColors/
        ├── TierItemColors.csproj
        ├── TierItemColors.cs
        ├── README.md
        ├── icon.png
        ├── spr_tic_enchant_minor.png
        ├── spr_tic_enchant_major.png
        ├── spr_tic_cursed.png
        └── Codes/
            └── scr_tic_draw_hover_markers.gml
```

Until PR #1 is merged, use the feature branch explicitly:

```powershell
cd ModSources
git clone -b feat/tier-item-colors https://github.com/alexebz/TierItemColors.git
```

If you already cloned the repository:

```powershell
cd ModSources\TierItemColors
git fetch origin
git switch feat/tier-item-colors
git pull
```

Then **restart ModShardLauncher**. Open your `vanilla.win`, go to the `ModSources` (C#) page, and `Tier Item Colors` should appear with a **Compile** button. Compiling creates the `.sml` under `Mods`.

The project references `ModShardLauncher.dll` and `UndertaleModLib.dll` via `..\..\`, which matches the directory layout above.

Target game version: **0.9.4.25**.
