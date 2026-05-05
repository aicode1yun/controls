window.shinyControls = window.shinyControls || {};

shinyControls.setCursorPosition = function (element, position) {
    if (element && element.setSelectionRange) {
        element.setSelectionRange(position, position);
    }
};
