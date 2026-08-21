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
    private const string LootColor = "gml_GlobalScript_scr_loot_color";

    public override void PatchMod()
    {
        Msl.AddFunction(ModFiles.GetCode("scr_tic_tier_color.gml"), "scr_tic_tier_color");
        Msl.AddFunction(ModFiles.GetCode("scr_tic_enchantment_prefix.gml"), "scr_tic_enchantment_prefix");

        // scr_loot_color is the shared vanilla color source used by tooltips,
        // ground labels, logs, and other item-name UI. Override its result only
        // when the current item has a recognized equipment tier (or is Unique).
        Msl.LoadGML(LootColor)
            .MatchFrom("if (__is_undefined(_color))")
            .InsertAbove(@"var _ticColor = scr_tic_tier_color()
        if (_ticColor != noone)
            _color = _ticColor")
            .Save();

        // Prefix the already-resolved title before vanilla wraps/measures it, so
        // curse/enchantment symbols are included in tooltip layout calculations.
        Msl.LoadGML(HoverWeaponRefresh)
            .MatchFrom("titleWidth = minWidth -")
            .InsertAbove("title = scr_tic_enchantment_prefix(owner) + title")
            .Save();
    }
}
