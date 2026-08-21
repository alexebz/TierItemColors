# Tier Item Colors

A ModShardLauncher mod for **Stoneshard 0.9.4.25** that makes item tier the dominant visual language for equipment names.

## Tier palette

| Tier | Color | Hex | RGB |
| --- | --- | --- | --- |
| T1 | Bone | `#C8C0AF` | `200, 192, 175` |
| T2 | Moss | `#7FA66A` | `127, 166, 106` |
| T3 | Steel Blue | `#5D8EAD` | `93, 142, 173` |
| T4 | Burnished Red | `#B56A5A` | `181, 106, 90` |
| T5 | Antique Gold | `#D6A64A` | `214, 166, 74` |

Unique and cursed equipment use the same color as other items of their tier.

## Enchantment markers

Enchantments are shown with a prefix while preserving the tier color:

- No enchantment: `Footman Sword`
- One enchantment: `✦ Footman Sword`
- Two enchantments: `✦✦ Footman Sword`

The markers are only shown after an item is identified, so unidentified equipment does not reveal hidden enchantment count.

## Implementation

Stoneshard stores the equipment tier/level in the item's `data["LVL"]` field and the currently selected quality color in `data["Colour"]`. Tier Item Colors replaces `Colour` with the configured tier color while leaving quality, curse, unique, and enchantment data intact.

Vanilla enchantment attributes are stored as `Char0`, `Char1`, etc. The mod counts the first two `CharN` entries to build the `✦` prefix.

The item-name hook is located at patch time inside `gml_Object_o_hoverWeapon_Other_21` rather than using a hard-coded line number. This is intended to make the mod more resilient to small Stoneshard UI changes.

## Build

Place this project under ModShardLauncher's `ModSources` directory and compile it through ModShardLauncher. The generated `.sml` will appear under `Mods`.

Target game version: **0.9.4.25**.
