function scr_tic_enchantment_prefix()
{
    var _owner = argument0
    var _enchantedAttributesArray = argument1

    if (!instance_exists(_owner))
        return ""

    var _data = _owner.data
    if (_data == noone || !ds_exists(_data, ds_type_map))
        return ""

    // Do not reveal hidden curse/enchantment information before identification.
    if (!ds_map_find_value_ext(_data, "identified", true))
        return ""

    var _prefix = ""

    if (ds_map_find_value_ext(_data, "is_cursed", false))
        _prefix += "☠"

    // Other_20 already asks vanilla for scr_hoversGetEnchantedAttributes().
    // That result is a flat [name, value, name, value, ...] array, so every
    // two entries represent one displayed enchantment attribute.
    var _count = floor(array_length(_enchantedAttributesArray) / 2)
    if (_count > 2)
        _count = 2

    switch (_count)
    {
        case 1:
            _prefix += "✦"
            break
        case 2:
            _prefix += "✦✦"
            break
    }

    if (_prefix != "")
        return _prefix + " "

    return ""
}
