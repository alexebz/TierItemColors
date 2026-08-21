function scr_tic_apply_style(_data)
{
    if (_data == noone || !ds_exists(_data, ds_type_map))
        return

    var _tier = real(ds_map_find_value_ext(_data, "LVL", 0))
    var _colour = noone

    switch (_tier)
    {
        // T1 — Bone #C8C0AF
        case 1:
            _colour = make_colour_rgb(200, 192, 175)
            break

        // T2 — Moss #7FA66A
        case 2:
            _colour = make_colour_rgb(127, 166, 106)
            break

        // T3 — Steel Blue #5D8EAD
        case 3:
            _colour = make_colour_rgb(93, 142, 173)
            break

        // T4 — Burnished Red #B56A5A
        case 4:
            _colour = make_colour_rgb(181, 106, 90)
            break

        // T5 — Antique Gold #D6A64A
        case 5:
            _colour = make_colour_rgb(214, 166, 74)
            break
    }

    // Items without a normal 1..5 tier are left untouched. This keeps the mod
    // compatible with special/non-tiered consumables and other modded objects.
    if (_colour != noone)
        ds_map_set(_data, "Colour", _colour)
}
