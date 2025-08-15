// Auto-scroll function for LCD text
window.scrollToBottom = function (element) {
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Keyboard listener for phone keypad
window.addKeyboardListener = function () {
    document.addEventListener('keydown', function (event) {
        // Prevent default behavior for number keys and special keys
        if (event.key >= '0' && event.key <= '9' || event.key === '*' || event.key === '#') {
            event.preventDefault();
            
            // Call the Blazor method to process the key
            DotNet.invokeMethodAsync('OldPhone.UI.Shared', 'ProcessKeyboardKey', event.key);
        }
    });
}; 