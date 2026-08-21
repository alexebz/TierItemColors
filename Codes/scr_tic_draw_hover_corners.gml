function scr_tic_draw_hover_corners()
{
    var _contentX = argument0
    var _contentY = argument1
    var _contentWidth = argument2
    var _hover = argument3
    var _colour = argument4
    var _surfaceScale = argument5

    // contentX/contentY describe the inner tooltip content area, while the
    // decorative Stoneshard frame sits roughly 12 px outside it at base scale.
    // Expand the bounds so the tier line overlays the actual outer frame edge
    // instead of drawing a second rectangle inside the tooltip.
    var _thickness = max(1, round(2 * _surfaceScale))
    var _frameOutset = max(1, round(12 * _surfaceScale))

    var _left = round(_contentX - _frameOutset)
    var _right = round(_contentX + _contentWidth + _frameOutset)
    var _top = round(_contentY - _frameOutset)

    var _contentHeight = 0
    if (instance_exists(_hover) && variable_instance_exists(_hover, "contentHeight"))
        _contentHeight = real(variable_instance_get(_hover, "contentHeight"))

    if (_contentHeight > 0)
    {
        var _bottom = round(_contentY + _contentHeight + _frameOutset)

        var _oldColour = draw_get_color()
        var _oldAlpha = draw_get_alpha()
        draw_set_color(_colour)
        draw_set_alpha(0.9)

        // Overlay the four outer frame edges.
        draw_rectangle(_left, _top, _right, _top + _thickness, false)
        draw_rectangle(_left, _bottom - _thickness, _right, _bottom, false)
        draw_rectangle(_left, _top, _left + _thickness, _bottom, false)
        draw_rectangle(_right - _thickness, _top, _right, _bottom, false)

        draw_set_color(_oldColour)
        draw_set_alpha(_oldAlpha)
    }
}
