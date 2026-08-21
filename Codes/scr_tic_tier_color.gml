function scr_tic_tier_color()
{
    var _quality = real(ds_map_find_value_ext(data, "quality", 0))

    // Unique items always keep Stoneshard's vanilla purple.
    if (_quality == (6 << 0))
        return make_colour_rgb(130, 72, 188)

    var _tier = 0

    // Enchantment mods may persist LVL in the data map, while vanilla equipment
    // exposes LVL as an instance variable. Support both representations.
    if (ds_map_exists(data, "LVL"))
        _tier = real(ds_map_find_value(data, "LVL"))
    else if (variable_instance_exists(id, "LVL"))
        _tier = real(LVL)

    switch (_tier)
    {
        case 1:
            return make_colour_rgb(200, 192, 175) // Bone #C8C0AF

        case 2:
            return make_colour_rgb(127, 166, 106) // Moss #7FA66A

        case 3:
            return make_colour_rgb(93, 142, 173) // Steel Blue #5D8EAD

        case 4:
            return make_colour_rgb(181, 106, 90) // Burnished Red #B56A5A

        case 5:
            return make_colour_rgb(214, 166, 74) // Antique Gold #D6A64A
    }

    // Non-equipment and anything without a recognized tier keep vanilla color.
    return noone
}
