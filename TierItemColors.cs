// Copyright (C)
// See LICENSE file for extended copyright information.

using ModShardLauncher;
using ModShardLauncher.Mods;

namespace TierItemColors;

public class TierItemColors : Mod
{
    public override string Author => "alexebz";
    public override string Name => "Tier Item Colors";
    public override string Description => "Colors tiered item names by tier, keeps Unique items purple, and marks curses/enchantments with symbols.";
    public override string Version => "0.1.0";
    public override string TargetVersion => "0.9.4.25";

    private const string HoverWeaponRefresh = "gml_Object_o_hoverWeapon_Other_20";
    private const string InventorySlotDraw = "gml_Object_o_inv_slot_Draw_0";

    public override void PatchMod()
    {
        Msl.AddFunction(ModFiles.GetCode("scr_tic_apply_style.gml"), "scr_tic_apply_style");
        Msl.AddFunction(ModFiles.GetCode("scr_tic_enchantment_prefix.gml"), "scr_tic_enchantment_prefix");

        // Keep the item's stored Colour synced with its tier. Normal and cursed
        // equipment use tier colors; Unique equipment deliberately keeps the
        // vanilla purple color so rarity remains obvious at a glance.
        Msl.LoadGML(InventorySlotDraw)
            .MatchAll()
            .InsertAbove("scr_tic_apply_style(data)")
            .Save();

        // Other_20 resolves both the displayed title and titleColor. Style the
        // owner before vanilla calls scr_loot_color(id), then prefix the resolved
        // title before wrapping/height calculations so long names lay out correctly.
        Msl.LoadGML(HoverWeaponRefresh)
            .MatchAll()
            .InsertAbove(@"with (owner)
    scr_tic_apply_style(data)")
            .Save();

        Msl.LoadGML(HoverWeaponRefresh)
            .MatchFrom("titleWidth = minWidth -")
            .InsertAbove("title = scr_tic_enchantment_prefix(owner) + title")
            .Save();
    }
}
