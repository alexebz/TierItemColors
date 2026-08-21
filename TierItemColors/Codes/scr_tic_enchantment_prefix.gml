function scr_tic_enchantment_prefix(_owner)
{
    if (!instance_exists(_owner))
        return ""

    var _data = _owner.data
    if (_data == noone || !ds_exists(_data, ds_type_map))
        return ""

    // Do not reveal hidden enchantments before the item is identified.
    if (!ds_map_find_value_ext(_data, "identified", true))
        return ""

    var _count = 0
    while (_count < 2 && ds_map_exists(_data, "Char" + string(_count)))
        _count++

    switch (_count)
    {
        case 1:
            return "✦ "
        case 2:
            return "✦✦ "
    }

    return ""
}
