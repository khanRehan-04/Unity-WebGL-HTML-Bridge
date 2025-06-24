mergeInto(LibraryManager.library, {
  ShowHtmlInputWithPlaceholder: function (placeholderPtr, idPtr, valuePtr) {
    const placeholder = UTF8ToString(placeholderPtr);
    const inputId = UTF8ToString(idPtr);
    const defaultValue = UTF8ToString(valuePtr);
    window.ShowHtmlInput(placeholder, inputId, defaultValue);
  }
});
