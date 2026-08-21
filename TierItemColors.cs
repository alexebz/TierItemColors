// Copyright (C)
// See LICENSE file for extended copyright information.

using System;
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
    private const string HoverWeaponDraw = "gml_Object_o_hoverWeapon_Other_21";
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

        // Tooltips can be refreshed independently of the inventory-slot draw,
        // so style the hovered owner as well (important for trade/inventory UI).
        Msl.LoadGML(HoverWeaponRefresh)
            .MatchAll()
            .InsertAbove(@"with (owner)
    scr_tic_apply_style(data)")
            .Save();

        PatchHoverNameDraw();
    }

    // Stoneshard 0.9.4.25 draws the equipment name as `title` using
    // scr_drawTextExt in o_hoverWeapon Other_21. Patch that exact call instead
    // of relying on the older scr_drawText/name heuristic.
    private static void PatchHoverNameDraw()
    {
        string code = Msl.GetStringGMLFromFile(HoverWeaponDraw);

        const string original =
            "scr_drawTextExt(contentX + (contentWidth / 2), contentY + _offsetY, title, titleColor, titleWidth, 1, 0, global.f_digits, textScale);";

        const string replacement =
            "scr_drawTextExt(contentX + (contentWidth / 2), contentY + _offsetY, (scr_tic_enchantment_prefix(owner) + title), titleColor, titleWidth, 1, 0, global.f_digits, textScale);";

        if (!code.Contains(original, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"Tier Item Colors could not locate the Stoneshard 0.9.4.25 title draw call in {HoverWeaponDraw}. " +
                "The hover UI may have changed again.");
        }

        Msl.SetStringGMLInFile(code.Replace(original, replacement, StringComparison.Ordinal), HoverWeaponDraw);
    }
}
