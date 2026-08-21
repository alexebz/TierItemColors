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
    public override string Description => "Colors equipment names and the native Stoneshard tooltip frame by tier.";
    public override string Version => "0.1.0";
    public override string TargetVersion => "0.9.4.25";

    private const string HoverRenderDraw = "gml_Object_o_hoverRender_Other_21";
    private const string HoverBoard = "gml_GlobalScript_scr_hoversDrawBoard";
    private const string LootColor = "gml_GlobalScript_scr_loot_color";
    private const string WeaponsTable = "gml_GlobalScript_table_weapons";
    private const string ArmorTable = "gml_GlobalScript_table_armor";

    public override void PatchMod()
    {
        // Build both helpers from the currently loaded vanilla.win. This keeps
        // the mod aligned with the exact equipment tables and native hover-frame
        // implementation used by Stoneshard 0.9.4.25.
        Msl.AddFunction(BuildTierColorFunction(), "scr_tic_tier_color");
        Msl.AddFunction(BuildTintedHoverBoardFunction(), "scr_tic_hoversDrawBoard");

        PatchLootColor();
        PatchEquipmentHoverFrame();
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

            AddTierAlias(fields[0].Trim(), tier, idsByTier, seen);
            AddTierAlias(fields[2].Trim(), tier, idsByTier, seen);
        }
    }

    private static void AddTierAlias(
        string value,
        int tier,
        List<string>[] idsByTier,
        HashSet<string> seen)
    {
        if (value.Length == 0 || !seen.Add(value))
            return;

        idsByTier[tier].Add(value);
    }

    private static string TierColorExpression(int tier)
    {
        switch (tier)
        {
            case 1: return "make_colour_rgb(200, 192, 175)"; // Bone #C8C0AF, baseline
            case 2: return "make_colour_rgb(126, 169, 103)"; // Moss #7EA967, +10% saturation
            case 3: return "make_colour_rgb(85, 144, 181)";  // Steel Blue #5590B5, +20% saturation
            case 4: return "make_colour_rgb(195, 97, 76)";   // Burnished Red #C3614C, +30% saturation
            case 5: return "make_colour_rgb(242, 175, 46)"; // Antique Gold #F2AF2E, +40% saturation
            default: throw new ArgumentOutOfRangeException("tier");
        }
    }

    private static string EscapeGmlString(string value)
    {
        return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    private static string BuildTintedHoverBoardFunction()
    {
        string code = Msl.GetStringGMLFromFile(HoverBoard);
        string[] lines = code.Replace("\r\n", "\n").Split('\n');

        int signatureLine = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("function scr_hoversDrawBoard(", StringComparison.Ordinal))
            {
                signatureLine = i;
                break;
            }
        }

        if (signatureLine < 0)
        {
            throw new InvalidOperationException(
                "Tier Item Colors could not locate scr_hoversDrawBoard's function declaration.");
        }

        // The private clone uses an explicit eight-argument signature. arg6 is
        // the native corner sprite and arg7 is our requested frame tint. This
        // avoids relying on how UMT/MSL serializes default function arguments.
        string signature = lines[signatureLine];
        string signatureIndent = signature[..(signature.Length - signature.TrimStart().Length)];
        lines[signatureLine] = signatureIndent
            + "function scr_tic_hoversDrawBoard(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7)";

        int tintedDrawCalls = 0;
        int spriteDrawCalls = 0;

        for (int i = signatureLine + 1; i < lines.Length; i++)
        {
            if (!lines[i].Contains("draw_sprite_ext(", StringComparison.Ordinal))
                continue;

            spriteDrawCalls++;

            // Vanilla scr_hoversDrawBoard has exactly eight white frame draws:
            // top, bottom, left, right and four corners. Its two background
            // draws use make_color_rgb(...) and intentionally stay unchanged.
            if (lines[i].Contains("c_white", StringComparison.Ordinal))
            {
                lines[i] = lines[i].Replace("c_white", "arg7", StringComparison.Ordinal);
                tintedDrawCalls++;
            }
            else if (lines[i].Contains("16777215", StringComparison.Ordinal))
            {
                lines[i] = lines[i].Replace("16777215", "arg7", StringComparison.Ordinal);
                tintedDrawCalls++;
            }
        }

        if (tintedDrawCalls != 8)
        {
            throw new InvalidOperationException(
                "Tier Item Colors expected 8 white native frame draws while cloning " + HoverBoard +
                " but found " + tintedDrawCalls + " across " + spriteDrawCalls + " sprite draws.");
        }

        return string.Join("\n", lines);
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
                "Tier Item Colors could not locate the _color undefined check in " + LootColor + ". " +
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

    private static void PatchEquipmentHoverFrame()
    {
        string code = Msl.GetStringGMLFromFile(HoverRenderDraw);
        string[] lines = code.Replace("\r\n", "\n").Split('\n');

        int targetLine = -1;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("scr_hoversDrawBoard(", StringComparison.Ordinal))
            {
                targetLine = i;
                break;
            }
        }

        if (targetLine < 0)
        {
            throw new InvalidOperationException(
                "Tier Item Colors could not locate scr_hoversDrawBoard in " + HoverRenderDraw + ".");
        }

        string indent = lines[targetLine][..(lines[targetLine].Length - lines[targetLine].TrimStart().Length)];
        string originalCall = lines[targetLine].Trim();
        string tintedCall = originalCall.Replace(
            "scr_hoversDrawBoard(",
            "scr_tic_hoversDrawBoard(",
            StringComparison.Ordinal);

        int callCloseParen = tintedCall.LastIndexOf(')');
        if (callCloseParen < 0)
        {
            throw new InvalidOperationException(
                "Tier Item Colors found the hover-board call but could not parse it.");
        }

        // o_hoverRender's vanilla call supplies six arguments and relies on the
        // standard corner-sprite default. Pass that sprite explicitly, then our
        // tint, so the private clone always receives all eight arguments.
        tintedCall = tintedCall.Insert(callCloseParen, ", s_hcorner, _ticFrameColor");

        string injected =
            indent + "var _ticFrameColor = 16777215\n" +
            indent + "if (instance_exists(contentRender))\n" +
            indent + "{\n" +
            indent + "    var _ticContentObject = contentRender.object_index\n" +
            indent + "    if ((_ticContentObject == o_hoverWeapon || object_is_ancestor(_ticContentObject, o_hoverWeapon)) && variable_instance_exists(contentRender, \"titleColor\"))\n" +
            indent + "        _ticFrameColor = variable_instance_get(contentRender, \"titleColor\")\n" +
            indent + "}\n" +
            indent + tintedCall;

        string patched = string.Join("\n", lines, 0, targetLine)
            + (targetLine > 0 ? "\n" : "")
            + injected
            + "\n"
            + string.Join("\n", lines, targetLine + 1, lines.Length - targetLine - 1);

        Msl.SetStringGMLInFile(patched, HoverRenderDraw);
    }
}
