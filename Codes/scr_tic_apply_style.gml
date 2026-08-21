function scr_tic_apply_style()
{
    var _data = argument0

    if (_data != noone && ds_exists(_data, ds_type_map))
    {
        // Unique items keep Stoneshard's vanilla purple so they remain immediately
        // distinguishable from normal equipment regardless of tier.
        var _quality = real(ds_map_find_value_ext(_data, "quality", 0))
        var _unique = (6 << 0)

        if (_quality == _unique)
        {
            ds_map_set(_data, "Colour", make_colour_rgb(130, 72, 188)) // #8248BC
        }
        else
        {
            var _tier = real(ds_map_find_value_ext(_data, "LVL", 0))
            var _colour = noone

            switch (_tier)
            {
                // T1 - Bone #C8C0AF
                case 1:
                    _colour = make_colour_rgb(200, 192, 175)
                    break

                // T2 - Moss #7FA66A
                case 2:
                    _colour = make_colour_rgb(127, 166, 106)
                    break

                // T3 - Steel Blue #5D8EAD
                case 3:
                    _colour = make_colour_rgb(93, 142, 173)
                    break

                // T4 - Burnished Red #B56A5A
                case 4:
                    _colour = make_colour_rgb(181, 106, 90)
                    break

                // T5 - Antique Gold #D6A64A
                case 5:
                    _colour = make_colour_rgb(214, 166, 74)
                    break
            }

            // Cursed equipment intentionally uses its tier color; curse state is shown
            // by a skull name prefix instead of replacing the tier color.
            // Items without a normal 1..5 tier are left untouched.
            if (_colour != noone)
                ds_map_set(_data, "Colour", _colour)
        }
    }
}
