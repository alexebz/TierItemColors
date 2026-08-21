// Copyright (C)
// See LICENSE file for extended copyright information.

using System;
using System.Collections.Generic;
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
    private const string WeaponsTable = "gml_GlobalScript_table_weapons";
    private const string ArmorTable = "gml_GlobalScript_table_armor";

    public override void PatchMod()
    {
        // Build the tier resolver from the actual tables in the currently loaded
        // vanilla.win. Tier is column 2 and item id is column 3 in both tables.
        Msl.AddFunction(BuildTierColorFunction(), "scr_tic_tier_color");
        Msl.AddFunction(ModFiles.GetCode("scr_tic_enchantment_prefix.gml"), "scr_tic_enchantment_prefix");

        PatchLootColor();

        // Other_20 has already built enchantedAttributesArray at this point.
        // Prefix before vanilla wraps/measures title so the markers participate
        // in the normal tooltip layout.
        Msl.LoadGML(HoverWeaponRefresh)
            .MatchFrom("titleWidth = minWidth -")
            .InsertAbove("title = scr_tic_enchantment_prefix(owner, enchantedAttributesArray) + title")
            .Save();
    }

    private static string BuildTierColorFunction()
    {
        List<string>[] idsByTier = new List<string>[6];
        for (int tier = 1; tier <= 5; tier++)
            idsByTier[tier] = new List<string>();

        HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
        AddTableTiers(WeaponsTable, idsByTier, seen);
        AddTableTiers(ArmorTable, idsByTier, seen);

        if (seen.Count == 0)
        {
            throw new InvalidOperationException(
                "Tier Item Colors could not read any tiered equipment from table_weapons/table_armor.");
        }

        for (int tier = 1; tier <= 5; tier++)
            idsByTier[tier].Sort(StringComparer.Ordinal);

        string code =
            "function scr_tic_tier_color()\n" +
            "{\n" +
            "    var _quality = real(ds_map_find_value_ext(data, \"quality\", 0))\n" +
            "    if (_quality == (6 << 0))\n" +
            "        return make_colour_rgb(130, 72, 188)\n\n" +
            "    var _itemId = ds_map_find_value_ext(data, \"idName\", \"\")\n" +
            "    switch (_itemId)\n" +
            "    {\n";

        for (int tier = 1; tier <= 5; tier++)
        {
            if (idsByTier[tier].Count == 0)
                continue;

            foreach (string id in idsByTier[tier])
                code += "        case \"" + EscapeGmlString(id) + "\":\n";

            code += "            return " + TierColorExpression(tier) + "\n\n";
        }

        code +=
            "    }\n\n" +
            "    return noone\n" +
            "}\n";

        return code;
    }

    private static void AddTableTiers(
        string tableName,
        List<string>[] idsByTier,
        HashSet<string> seen)
    {
        List<string> table = ModLoader.GetTable(tableName);
        if (table == null)
            return;

        foreach (string row in table)
        {
            string[] fields = row.Split(';');
            if (fields.Length < 3)
                continue;

            int tier;
            if (!int.TryParse(fields[1].Trim(), out tier) || tier < 1 || tier > 5)
                continue;

            string id = fields[2].Trim();
            if (id.Length == 0 || !seen.Add(id))
                continue;

            idsByTier[tier].Add(id);
        }
    }

    private static string TierColorExpression(int tier)
    {
        switch (tier)
        {
            case 1: return "make_colour_rgb(200, 192, 175)"; // Bone #C8C0AF
            case 2: return "make_colour_rgb(127, 166, 106)"; // Moss #7FA66A
            case 3: return "make_colour_rgb(93, 142, 173)";  // Steel Blue #5D8EAD
            case 4: return "make_colour_rgb(181, 106, 90)";  // Burnished Red #B56A5A
            case 5: return "make_colour_rgb(214, 166, 74)";  // Antique Gold #D6A64A
            default: throw new ArgumentOutOfRangeException(nameof(tier));
        }
    }

    private static string EscapeGmlString(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static void PatchLootColor()
    {
        string code = Msl.GetStringGMLFromFile(LootColor);
        string[] lines = code.Replace("\r\n", "\n").Split('\n');

        int targetLine = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("undefined", StringComparison.OrdinalIgnoreCase)
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
