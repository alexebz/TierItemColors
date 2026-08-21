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
    private const string LootColor = "gml_GlobalScript_scr_loot_color";
    private const string WeaponsTable = "gml_GlobalScript_table_weapons";
    private const string ArmorTable = "gml_GlobalScript_table_armor";

    public override void PatchMod()
    {
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
        // This is the 0.9.4.25 scr_hoversDrawBoard geometry from the user's
        // exported Code/, isolated into a private helper. Do not use named GML
        // parameters here: MSL/UndertaleModLib can compile higher named args as
        // instance-variable reads. argument[] is already used by vanilla scripts
        // and reliably preserves the actual call arguments.
        return
            "function scr_tic_hoversDrawBoard()\n" +
            "{\n" +
            "    var _ticX = argument[0]\n" +
            "    var _ticY = argument[1]\n" +
            "    var _ticWidth = argument[2]\n" +
            "    var _ticHeight = argument[3]\n" +
            "    var _ticScale = argument[4]\n" +
            "    var _ticDrawBackground = argument[5]\n" +
            "    var _ticCornerSprite = argument[6]\n" +
            "    var _ticFrameColor = argument[7]\n\n" +
            "    var _lineHorizontalHeight = sprite_get_height(s_wline) * _ticScale\n" +
            "    var _lineVerticalWidth = sprite_get_width(s_hline) * _ticScale\n" +
            "    var _cornerWidth = sprite_get_width(_ticCornerSprite) * _ticScale\n" +
            "    var _cornerHeight = sprite_get_height(_ticCornerSprite) * _ticScale\n\n" +
            "    if (_ticDrawBackground)\n" +
            "        draw_sprite_ext(s_point, 0, _ticX, _ticY, _ticWidth, _ticHeight, 0, make_color_rgb(27, 25, 38), 1)\n\n" +
            "    draw_sprite_ext(s_wline, 0, _ticX, _ticY, _ticWidth, _ticScale, 0, _ticFrameColor, 1)\n" +
            "    draw_sprite_ext(s_wline, 1, _ticX, (_ticY + _ticHeight) - _lineHorizontalHeight, _ticWidth, _ticScale, 0, _ticFrameColor, 1)\n" +
            "    draw_sprite_ext(s_point, 0, _ticX, _ticY + _lineHorizontalHeight, _ticWidth, _ticScale, 0, make_color_rgb(17, 16, 26), 1)\n" +
            "    draw_sprite_ext(s_hline, 0, _ticX, _ticY, _ticScale, _ticHeight, 0, _ticFrameColor, 1)\n" +
            "    draw_sprite_ext(s_hline, 1, (_ticX + _ticWidth) - _lineVerticalWidth, _ticY, _ticScale, _ticHeight, 0, _ticFrameColor, 1)\n" +
            "    draw_sprite_ext(_ticCornerSprite, 0, _ticX, _ticY, _ticScale, _ticScale, 0, _ticFrameColor, 1)\n" +
            "    draw_sprite_ext(_ticCornerSprite, 1, (_ticX + _ticWidth) - _cornerWidth, _ticY, _ticScale, _ticScale, 0, _ticFrameColor, 1)\n" +
            "    draw_sprite_ext(_ticCornerSprite, 2, _ticX, (_ticY + _ticHeight) - _cornerHeight, _ticScale, _ticScale, 0, _ticFrameColor, 1)\n" +
            "    draw_sprite_ext(_ticCornerSprite, 3, (_ticX + _ticWidth) - _cornerWidth, (_ticY + _ticHeight) - _cornerHeight, _ticScale, _ticScale, 0, _ticFrameColor, 1)\n" +
            "}\n";
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

        // o_hoverRender's vanilla call has six arguments. Supply the native
        // default corner sprite explicitly and then the tier tint as arg 8.
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
