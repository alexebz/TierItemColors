# Tier Item Colors

A ModShardLauncher mod for **Stoneshard 0.9.4.25** that makes item tier the primary visual language for equipment names and adds subtle matching accents to equipment tooltips.

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

## Tooltip frame accents

Equipment hover tooltips keep Stoneshard's original frame, background, and layout.

Tier Item Colors overlays only short **L-shaped accents at the frame corners** using the same color as the item title:

- T1 → Bone corners
- T2 → Moss corners
- T3 → Steel Blue corners
- T4 → Burnished Red corners
- T5 → Antique Gold corners
- Unique → vanilla purple corners

The full frame is intentionally **not** recolored. Only the corner/edge accents change color so the result stays close to Stoneshard's native UI style.

The old enchantment/cursed marker icons have been removed completely. The mod no longer adds state icons to hover tooltips or ground loot.

## Implementation

Stoneshard 0.9.4.25 centralizes item-name color selection in `scr_loot_color(id)`. Tier Item Colors hooks that function so the same tier color is used consistently by tooltips, ground labels, shops, logs, and other UI that relies on vanilla loot coloring.

The mod does **not** infer tier from runtime `LVL`. During patching it reads the current `gml_GlobalScript_table_weapons` and `gml_GlobalScript_table_armor` directly from the loaded `vanilla.win`. In both tables, column 2 is the equipment tier. The generated resolver accepts both the table's item name and item id as aliases for runtime `data["idName"]`, which makes it compatible with vanilla equipment instances.

Items whose `quality` is Unique keep vanilla purple instead of receiving a tier color.

Tooltip corner accents are drawn by `scr_tic_draw_hover_corners` from `gml_Object_o_hoverWeapon_Other_21`. The helper receives vanilla's already-resolved `titleColor`, so the corner accents always match the title color without maintaining a second palette lookup.

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
        └── Codes/
            └── scr_tic_draw_hover_corners.gml
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
