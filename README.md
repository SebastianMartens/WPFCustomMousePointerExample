WPFCustomMousePointerExample
============================

Example of how to create custom mouse pointers for dragdrop operations.

Motivation was the fact that a DragDrop operation should be performed between multiple instances of the same application. During this dragdrop the mouse pointer should change to something that indicates the dragged item etc.

Unfortunately the default way (using a WPF adorner that is shown beneath the mouse pointer) doesn't work outside of the application window.

As POC for a possible soultion this example creates a custom mouse pointer which can be a text or a visual representation of the dragged control. As the mouse pointer itself is changed, it is also possible to show the text/element outside of the bounds of the application.
