function scr_tic_draw_hover_markers()
{
    var _owner = argument0
    var _enchantedAttributesArray = argument1
    var _centerX = argument2
    var _topY = argument3
    var _title = argument4
    var _textScale = argument5
    var _surfaceScale = argument6

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
                    // Marker PNGs are 16x16. Scale them with the UI so they keep the
                    // same apparent size as the title text at every resolution.
                    var _iconScale = 0.75 * _surfaceScale
                    var _iconSize = 16 * _iconScale
                    var _gap = 2 * _surfaceScale
                    var _rowWidth = (_iconCount * _iconSize) + ((_iconCount - 1) * _gap)

                    // Find the actual title width in the same font vanilla uses. The
                    // marker row is then placed immediately to the left of the title.
                    var _oldFont = draw_get_font()
                    draw_set_font(global.f_digits)
                    var _textWidth = string_width(_title) * _textScale
                    draw_set_font(_oldFont)

                    var _drawX = _centerX - (_textWidth * 0.5) - _gap - (_rowWidth * 0.5)
                    var _drawY = _topY + (8 * _surfaceScale)

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
