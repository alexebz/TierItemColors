# Tier Item Colors

A ModShardLauncher mod for **Stoneshard 0.9.4.25** that makes item tier the primary visual language for normal equipment names while preserving a distinct look for Unique items.

## Tier palette

| Tier | Color | Hex | RGB |
| --- | --- | --- | --- |
| T1 | Bone | `#C8C0AF` | `200, 192, 175` |
| T2 | Moss | `#7FA66A` | `127, 166, 106` |
| T3 | Steel Blue | `#5D8EAD` | `93, 142, 173` |
| T4 | Burnished Red | `#B56A5A` | `181, 106, 90` |
| T5 | Antique Gold | `#D6A64A` | `214, 166, 74` |

Normal and cursed equipment use their tier color.

**Unique items keep Stoneshard's vanilla purple (`#8248BC`).**

## Item-state markers

Item state is communicated with symbols while preserving the selected name color:

- Normal: `Footman Sword`
- One enchantment: `✦ Footman Sword`
- Two enchantments: `✦✦ Footman Sword`
- Cursed: `☠ Footman Sword`
- Cursed + one enchantment: `☠✦ Footman Sword`
- Cursed + two enchantments: `☠✦✦ Footman Sword`

Unique items can still receive the same enchantment/curse markers, but their name remains purple.

The markers are only shown after an item is identified, so unidentified equipment does not reveal hidden curse or enchantment information.

## Visual language

- **Tier color** → progression tier for normal/cursed equipment
- **Purple** → Unique item
- **☠** → Cursed
- **✦ / ✦✦** → one/two enchantments

## Implementation

Stoneshard 0.9.4.25 centralizes item-name color selection in `scr_loot_color(id)`. Tier Item Colors hooks that function so the same tier color is used consistently by tooltips, ground labels, shops, logs, and other UI that relies on vanilla loot coloring.

Vanilla equipment exposes its tier through the item instance variable `LVL`. Some modded/enchantment-generated items also persist the tier into `data["LVL"]`, so the mod supports both representations. Items whose `quality` is Unique keep vanilla purple instead of receiving a tier color.

Curse state is read from `data["is_cursed"]`. Vanilla enchantment attributes are stored as `Char0`, `Char1`, etc. The mod uses those fields to build the `☠` and `✦` name prefix without altering the underlying item stats, quality, curse, or enchantment data.

The tooltip prefix is inserted in `gml_Object_o_hoverWeapon_Other_20` before vanilla wraps and measures `title`, so symbols are included in tooltip layout calculations.

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
        └── Codes/
            ├── scr_tic_tier_color.gml
            └── scr_tic_enchantment_prefix.gml
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
