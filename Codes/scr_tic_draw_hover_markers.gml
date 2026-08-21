function scr_tic_draw_hover_markers()
{
    var _owner = argument0
    var _enchantedAttributesArray = argument1
    var _contentX = argument2
    var _topY = argument3
    var _surfaceScale = argument4

    if (instance_exists(_owner))
    {
        var _data = _owner.data
        if (_data != noone && ds_exists(_data, ds_type_map))
        {
            // Hidden curse/enchantment state must stay hidden until identification.
            if (ds_map_find_value_ext(_data, "identified", true))
            {
                var _cursed = ds_map_find_value_ext(_data, "is_cursed", false)
                var _enchants = 0

                if (is_array(_enchantedAttributesArray))
                {
                    _enchants = floor(array_length(_enchantedAttributesArray) / 2)
                    if (_enchants > 2)
                        _enchants = 2
                }
                else
                {
                    if (ds_map_exists(_data, "Char0"))
                        _enchants = 1
                    if (ds_map_exists(_data, "Char1"))
                        _enchants = 2
                }

                var _iconCount = (_cursed ? 1 : 0) + (_enchants > 0 ? 1 : 0)
                if (_iconCount > 0)
                {
                    // Source sprites are 16x16. Draw them as 24x24 at the base UI
                    // scale so they are clearly visible while still fitting the title row.
                    var _iconScale = 1.5 * _surfaceScale
                    var _iconSize = 16 * _iconScale
                    var _iconGap = 2 * _surfaceScale
                    var _leftPadding = 6 * _surfaceScale

                    // Anchor markers to a guaranteed-visible position inside the
                    // tooltip header. This deliberately avoids measuring localized,
                    // wrapped title text because that can push icons outside the
                    // clipped hover surface.
                    var _drawX = _contentX + _leftPadding + (_iconSize * 0.5)
                    var _drawY = _topY + (_iconSize * 0.5)

                    // Cursed is always first. If an item is both cursed and enchanted,
                    // the enchantment marker is drawn immediately after it.
                    if (_cursed)
                    {
                        draw_sprite_ext(spr_tic_cursed, 0, _drawX, _drawY, _iconScale, _iconScale, 0, c_white, 1)
                        _drawX += _iconSize + _iconGap
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
