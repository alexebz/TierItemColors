function scr_tic_draw_hover_corners()
{
    var _contentX = argument0
    var _contentY = argument1
    var _contentWidth = argument2
    var _hover = argument3
    var _colour = argument4
    var _surfaceScale = argument5

    // contentX/contentY describe the inner content area. Do not guess a fixed
    // frame padding: use the hover instance's real x/y as the outer frame origin
    // when they are close enough to the content bounds to be plausible.
    var _thickness = max(1, round(2 * _surfaceScale))
    var _edgeInset = max(1, round(1 * _surfaceScale))
    var _maxPadding = max(8, round(24 * _surfaceScale))

    var _left = _contentX
    var _top = _contentY
    var _padX = 0
    var _padY = 0

    if (instance_exists(_hover))
    {
        if (variable_instance_exists(_hover, "x"))
        {
            var _hoverX = real(variable_instance_get(_hover, "x"))
            var _candidatePadX = _contentX - _hoverX

            if (_candidatePadX >= 0 && _candidatePadX <= _maxPadding)
            {
                _left = _hoverX
                _padX = _candidatePadX
            }
        }

        if (variable_instance_exists(_hover, "y"))
        {
            var _hoverY = real(variable_instance_get(_hover, "y"))
            var _candidatePadY = _contentY - _hoverY

            if (_candidatePadY >= 0 && _candidatePadY <= _maxPadding)
            {
                _top = _hoverY
                _padY = _candidatePadY
            }
        }
    }

    var _right = _contentX + _contentWidth + _padX

    var _contentHeight = 0
    if (instance_exists(_hover) && variable_instance_exists(_hover, "contentHeight"))
        _contentHeight = real(variable_instance_get(_hover, "contentHeight"))

    if (_contentHeight > 0)
    {
        var _bottom = _contentY + _contentHeight + _padY

        // Keep the overlay one pixel inside the calculated outer edge so it is
        // guaranteed to remain inside the tooltip surface and is not clipped.
        _left = round(_left + _edgeInset)
        _right = round(_right - _edgeInset)
        _top = round(_top + _edgeInset)
        _bottom = round(_bottom - _edgeInset)

        var _oldColour = draw_get_color()
        var _oldAlpha = draw_get_alpha()
        draw_set_color(_colour)
        draw_set_alpha(0.9)

        // Thin tier-colored overlay on all four outer frame edges.
        draw_rectangle(_left, _top, _right, _top + _thickness, false)
        draw_rectangle(_left, _bottom - _thickness, _right, _bottom, false)
        draw_rectangle(_left, _top, _left + _thickness, _bottom, false)
        draw_rectangle(_right - _thickness, _top, _right, _bottom, false)

        draw_set_color(_oldColour)
        draw_set_alpha(_oldAlpha)
    }
}
