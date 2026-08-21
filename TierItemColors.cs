// Copyright (C)
// See LICENSE file for extended copyright information.

using System;
using System.Collections.Generic;
using System.Linq;
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

    // Prefix the tooltip's item-name draw expression with curse/enchantment
    // markers. Locate the name draw call in the currently loaded vanilla.win
    // instead of hard-coding a line number, making the patch less brittle.
    private static void PatchHoverNameDraw()
    {
        string code = Msl.GetStringGMLFromFile(HoverWeaponDraw);
        List<string> lines = code.Replace("\r\n", "\n").Split('\n').ToList();

        int lineIndex = lines.FindIndex(IsLikelyItemNameDraw);
        if (lineIndex < 0)
        {
            throw new InvalidOperationException(
                $"Tier Item Colors could not locate the item-name draw call in {HoverWeaponDraw}. " +
                "Stoneshard may have changed the hover UI; update the mod hook for the current game version.");
        }

        if (!TryPrefixThirdArgument(lines[lineIndex], out string patchedLine))
        {
            throw new InvalidOperationException(
                $"Tier Item Colors found a likely item-name draw call in {HoverWeaponDraw}, " +
                "but could not safely patch its text argument.");
        }

        lines[lineIndex] = patchedLine;
        Msl.SetStringGMLInFile(string.Join("\n", lines), HoverWeaponDraw);
    }

    private static bool IsLikelyItemNameDraw(string line)
    {
        if (!line.Contains("scr_drawText(", StringComparison.Ordinal))
            return false;

        // Current Stoneshard hover code uses a name-like variable for the title.
        // Keep a couple of fallbacks so small upstream renames don't break us.
        return line.Contains("name", StringComparison.OrdinalIgnoreCase)
            || line.Contains("title", StringComparison.OrdinalIgnoreCase);
    }

    // Rewrite the third top-level argument of scr_drawText(...), which is the
    // text expression, without assuming the variable name used by vanilla.
    private static bool TryPrefixThirdArgument(string line, out string patched)
    {
        patched = line;
        const string functionName = "scr_drawText(";
        int callStart = line.IndexOf(functionName, StringComparison.Ordinal);
        if (callStart < 0)
            return false;

        int argsStart = callStart + functionName.Length;
        List<int> commas = new();
        int depth = 0;
        bool inString = false;
        char quote = '\0';

        for (int i = argsStart; i < line.Length; i++)
        {
            char c = line[i];

            if (inString)
            {
                if (c == quote && (i == 0 || line[i - 1] != '\\'))
                    inString = false;
                continue;
            }

            if (c is '\'' or '"')
            {
                inString = true;
                quote = c;
                continue;
            }

            if (c == '(')
            {
                depth++;
                continue;
            }

            if (c == ')')
            {
                if (depth == 0)
                    break;
                depth--;
                continue;
            }

            if (c == ',' && depth == 0)
                commas.Add(i);
        }

        if (commas.Count < 3)
            return false;

        int textStart = commas[1] + 1;
        int textEnd = commas[2];
        string originalText = line[textStart..textEnd].Trim();
        if (string.IsNullOrWhiteSpace(originalText))
            return false;

        string replacement = $" (scr_tic_enchantment_prefix(owner) + {originalText})";
        patched = line[..textStart] + replacement + line[textEnd..];
        return true;
    }
}
