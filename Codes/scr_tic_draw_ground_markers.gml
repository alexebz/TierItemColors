function scr_tic_draw_ground_markers()
{
    var _owner = argument0

    if (instance_exists(_owner))
    {
        with (_owner)
        {
            if (data != noone && ds_exists(data, ds_type_map))
            {
                // Do not reveal curse/enchantment state before identification.
                if (ds_map_find_value_ext(data, "identified", true))
                {
                    var _cursed = ds_map_find_value_ext(data, "is_cursed", false)
                    var _enchants = 0

                    // Ground loot does not own the tooltip's enchantedAttributesArray,
                    // but vanilla persists the generated enchantments as Char0/Char1.
                    if (ds_map_exists(data, "Char0"))
                        _enchants = 1
                    if (ds_map_exists(data, "Char1"))
                        _enchants = 2

                    var _iconCount = (_cursed ? 1 : 0) + (_enchants > 0 ? 1 : 0)
                    if (_iconCount > 0)
                    {
                        // World-space markers are intentionally slightly smaller than
                        // the hover markers so they do not obscure the loot sprite.
                        var _iconScale = 0.75
                        var _iconSize = 16 * _iconScale
                        var _gap = 2
                        var _rowWidth = (_iconCount * _iconSize) + ((_iconCount - 1) * _gap)
                        var _drawX = x - (_rowWidth * 0.5) + (_iconSize * 0.5)
                        var _drawY = y - max(10, sprite_height * 0.5) - 8

                        if (_cursed)
                        {
                            draw_sprite_ext(spr_tic_cursed, 0, _drawX, _drawY, _iconScale, _iconScale, 0, c_white, 1)
                            _drawX += _iconSize + _gap
                        }

                        if (_enchants == 1)
                            draw_sprite_ext(spr_tic_enchant_minor, 0, _drawX, _drawY, _iconScale, _iconScale, 0, c_white, 1)
                        else if (_enchants >= 2)
                            draw_sprite_ext(spr_tic_enchant_major, 0, _drawX, _drawY, _iconScale, _iconScale, 0, c_white, 1)
                    }
                }
            }
        }
    }
}
