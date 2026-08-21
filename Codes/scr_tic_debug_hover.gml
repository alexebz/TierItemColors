function scr_tic_debug_hover()
{
    var _owner = argument0
    var _enchanted = argument1
    var _title = string(argument2)

    if (instance_exists(_owner))
    {
        if (!variable_global_exists("_tic_debug_last_owner"))
            global._tic_debug_last_owner = noone

        if (global._tic_debug_last_owner != _owner)
        {
            global._tic_debug_last_owner = _owner

            var _data = _owner.data
            if (_data != noone && ds_exists(_data, ds_type_map))
            {
                var _idName = string(ds_map_find_value_ext(_data, "idName", "<missing>"))
                var _arrayPosition = string(ds_map_find_value_ext(_data, "arrayPosition", "<missing>"))
                var _num = string(ds_map_find_value_ext(_data, "Num", "<missing>"))
                var _lvlData = string(ds_map_find_value_ext(_data, "LVL", "<missing>"))
                var _quality = string(ds_map_find_value_ext(_data, "quality", "<missing>"))
                var _colour = string(ds_map_find_value_ext(_data, "Colour", "<missing>"))
                var _identified = string(ds_map_find_value_ext(_data, "identified", "<missing>"))
                var _isCursed = string(ds_map_find_value_ext(_data, "is_cursed", "<missing>"))

                var _lvlInstance = "<missing>"
                if (variable_instance_exists(_owner, "LVL"))
                    _lvlInstance = string(variable_instance_get(_owner, "LVL"))

                var _char0 = "<missing>"
                if (ds_map_exists(_data, "Char0"))
                    _char0 = string(ds_map_find_value(_data, "Char0"))

                var _char1 = "<missing>"
                if (ds_map_exists(_data, "Char1"))
                    _char1 = string(ds_map_find_value(_data, "Char1"))

                var _enchantedLength = array_length(_enchanted)
                var _enchantedText = ""
                for (var _i = 0; _i < _enchantedLength; _i++)
                {
                    if (_i > 0)
                        _enchantedText += " | "
                    _enchantedText += string(_enchanted[_i])
                }

                var _prefix = scr_tic_enchantment_prefix(_owner, _enchanted)

                scr_msl_log(
                    "[TIC] title=" + _title
                    + " owner=" + string(_owner)
                    + " idName=" + _idName
                    + " arrayPosition=" + _arrayPosition
                    + " Num=" + _num
                    + " LVL(data)=" + _lvlData
                    + " LVL(instance)=" + _lvlInstance
                    + " quality=" + _quality
                    + " Colour=" + _colour
                    + " identified=" + _identified
                    + " cursed=" + _isCursed
                    + " Char0=" + _char0
                    + " Char1=" + _char1
                    + " enchantedLen=" + string(_enchantedLength)
                    + " enchanted=[" + _enchantedText + "]"
                    + " prefix='" + _prefix + "'"
                    + " prefixLen=" + string(string_length(_prefix)))
            }
            else
            {
                scr_msl_log("[TIC] title=" + _title + " owner=" + string(_owner) + " data=<invalid>")
            }
        }
    }
}
