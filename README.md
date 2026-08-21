# Tier Item Colors

A ModShardLauncher mod for **Stoneshard 0.9.4.25** that makes equipment tier the primary visual language for item names and the native tooltip frame.

## Tier palette

| Tier | Color | Hex | RGB |
| --- | --- | --- | --- |
| T1 | Bone | `#C8C0AF` | `200, 192, 175` |
| T2 | Moss | `#7EA967` | `126, 169, 103` |
| T3 | Steel Blue | `#5590B5` | `85, 144, 181` |
| T4 | Burnished Red | `#C3614C` | `195, 97, 76` |
| T5 | Antique Gold | `#F2AF2E` | `242, 175, 46` |

T1 keeps the original Bone intensity. Saturation increases progressively from T2 through T5.

Normal and cursed equipment use their tier color.

**Unique items keep Stoneshard's vanilla purple (`#8248BC`).**

## Native tooltip frame tint

The mod no longer draws a second rectangle over the tooltip.

Stoneshard's real frame is drawn in `scr_hoversDrawBoard()` with the vanilla frame sprites:

- `s_wline` — top and bottom edges
- `s_hline` — left and right edges
- the board corner sprite (`arg6`) — all four corners

Tier Item Colors extends `scr_hoversDrawBoard()` with an optional tint argument. Existing callers keep the vanilla white tint. Equipment tooltips pass their already-resolved `titleColor`, so the original Stoneshard frame artwork itself is tinted to match the item's tier or Unique color.

The tooltip background and inner content remain unchanged.

The old enchantment/cursed marker icons and all custom rectangle border overlays have been removed.

## Implementation

Stoneshard 0.9.4.25 centralizes item-name color selection in `scr_loot_color(id)`. Tier Item Colors hooks that function so the same tier color is used consistently by tooltips, ground labels, shops, logs, and other UI that relies on vanilla loot coloring.

The mod does **not** infer tier from runtime `LVL`. During patching it reads the current `gml_GlobalScript_table_weapons` and `gml_GlobalScript_table_armor` directly from the loaded `vanilla.win`. In both tables, column 2 is the equipment tier. The generated resolver accepts both the table's item name and item id as aliases for runtime `data["idName"]`.

For tooltip frames, the mod patches:

- `gml_GlobalScript_scr_hoversDrawBoard` — adds the optional frame tint and applies it to the 8 native edge/corner sprite draws
- `gml_Object_o_hoverRender_Other_21` — passes `titleColor` only when `contentRender` is `o_hoverWeapon`

Other UI that uses `scr_hoversDrawBoard()` remains white because the new tint parameter defaults to vanilla white.

## Build / MSL discovery

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
        └── icon.png
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

Then restart ModShardLauncher, compile the mod, patch a clean `vanilla.win`, and launch Stoneshard.

Target game version: **0.9.4.25**.
