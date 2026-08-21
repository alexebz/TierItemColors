# Tier Item Colors

A ModShardLauncher mod for **Stoneshard 0.9.4.25** that makes item tier the primary visual language for normal equipment names while preserving a distinct look for Unique items.

## Tier palette

| Tier | Color | Hex | RGB | Saturation progression |
| --- | --- | --- | --- | --- |
| T1 | Bone | `#C8C0AF` | `200, 192, 175` | baseline |
| T2 | Moss | `#7EA967` | `126, 169, 103` | +10% |
| T3 | Steel Blue | `#5590B5` | `85, 144, 181` | +20% |
| T4 | Burnished Red | `#C3614C` | `195, 97, 76` | +30% |
| T5 | Antique Gold | `#F2AF2E` | `242, 175, 46` | +40% |

The higher tiers progressively increase saturation while keeping approximately the same hue/lightness as the original palette, so progression becomes more vivid without simply making every tier brighter.

Normal and cursed equipment use their tier color.

**Unique items keep Stoneshard's vanilla purple (`#8248BC`).**

## Item-state markers

Item state is communicated with symbols while preserving the selected name color:

- Normal: `Footman Sword`
- One enchantment: `★ Footman Sword`
- Two enchantments: `★★ Footman Sword`
- Cursed: `⚠ Footman Sword`
- Cursed + one enchantment: `⚠★ Footman Sword`
- Cursed + two enchantments: `⚠★★ Footman Sword`

Unique items can still receive the same enchantment/curse markers, but their name remains purple.

The markers are only shown after an item is identified, so unidentified equipment does not reveal hidden curse or enchantment information.

## Visual language

- **Tier color** → progression tier for normal/cursed equipment
- **Purple** → Unique item
- **⚠** → Cursed
- **★ / ★★** → one/two displayed enchantment attributes

## Implementation

Stoneshard 0.9.4.25 centralizes item-name color selection in `scr_loot_color(id)`. Tier Item Colors hooks that function so the same tier color is used consistently by tooltips, ground labels, shops, logs, and other UI that relies on vanilla loot coloring.

The mod does **not** infer tier from runtime `LVL`. During patching it reads the current `gml_GlobalScript_table_weapons` and `gml_GlobalScript_table_armor` directly from the loaded `vanilla.win`. In both tables, column 2 is the equipment tier. The generated resolver accepts both the table's item name and item id as aliases for runtime `data["idName"]`, which makes it compatible with vanilla equipment instances.

Items whose `quality` is Unique keep vanilla purple instead of receiving a tier color.

Curse state is read from `data["is_cursed"]`. Enchantment markers are derived from the `enchantedAttributesArray` that vanilla already builds in `gml_Object_o_hoverWeapon_Other_20` through `scr_hoversGetEnchantedAttributes()`. This means the number of stars matches the enchantment attributes actually displayed in the tooltip rather than relying on `Char0`/`Char1` storage details.

The selected markers use `⚠` (U+26A0 WARNING SIGN) for cursed items and `★` (U+2605 BLACK STAR) for enchantments. The GML helper builds them from Unicode codepoints at runtime to avoid MSL/UndertaleModLib source-encoding issues.

The tooltip prefix is inserted before vanilla wraps and measures `title`, so symbols are included in tooltip layout calculations.

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
        ├── icon.png
        ├── README.md
        └── Codes/
            ├── scr_tic_enchantment_prefix.gml
            └── scr_tic_debug_hover.gml
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
