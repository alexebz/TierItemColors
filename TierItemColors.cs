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
    private const string LootColor = "gml_GlobalScript_scr_loot_color";

    public override void PatchMod()
    {
        Msl.AddFunction(ModFiles.GetCode("scr_tic_tier_color.gml"), "scr_tic_tier_color");
        Msl.AddFunction(ModFiles.GetCode("scr_tic_enchantment_prefix.gml"), "scr_tic_enchantment_prefix");

        PatchLootColor();

        // Prefix the already-resolved title before vanilla wraps/measures it, so
        // curse/enchantment symbols are included in tooltip layout calculations.
        Msl.LoadGML(HoverWeaponRefresh)
            .MatchFrom("titleWidth = minWidth -")
            .InsertAbove("title = scr_tic_enchantment_prefix(owner) + title")
            .Save();
    }

    private static void PatchLootColor()
    {
        string code = Msl.GetStringGMLFromFile(LootColor);
        string[] lines = code.Replace("\r\n", "\n").Split('\n');

        int targetLine = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("__is_undefined", StringComparison.Ordinal)
                && lines[i].Contains("_color", StringComparison.Ordinal))
            {
                targetLine = i;
                break;
            }
        }

        if (targetLine < 0)
        {
            throw new InvalidOperationException(
                $"Tier Item Colors could not locate the _color undefined check in {LootColor}. " +
                "The Stoneshard loot-color implementation may have changed.");
        }

        string indent = lines[targetLine][..(lines[targetLine].Length - lines[targetLine].TrimStart().Length)];
        string injected =
            indent + "var _ticColor = scr_tic_tier_color()\n" +
            indent + "if (_ticColor != noone)\n" +
            indent + "    _color = _ticColor\n";

        string patched = string.Join("\n", lines, 0, targetLine)
            + (targetLine > 0 ? "\n" : "")
            + injected
            + string.Join("\n", lines, targetLine, lines.Length - targetLine);

        Msl.SetStringGMLInFile(patched, LootColor);
    }
}
