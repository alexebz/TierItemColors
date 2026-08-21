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

Stoneshard stores the equipment tier/level in the item's `data["LVL"]` field and the currently selected quality color in `data["Colour"]`. Tier Item Colors replaces `Colour` with the configured tier color for non-Unique tiered items. Items whose `quality` is Unique keep vanilla purple.

Curse state is read from `data["is_cursed"]`. Vanilla enchantment attributes are stored as `Char0`, `Char1`, etc. The mod uses those fields to build the `☠` and `✦` name prefix without altering the underlying item stats, quality, curse, or enchantment data.

The item-name hook is located at patch time inside `gml_Object_o_hoverWeapon_Other_21` rather than using a hard-coded line number. This is intended to make the mod more resilient to small Stoneshard UI changes.

## Build

Place this project under ModShardLauncher's `ModSources` directory and compile it through ModShardLauncher. The generated `.sml` will appear under `Mods`.

Target game version: **0.9.4.25**.
