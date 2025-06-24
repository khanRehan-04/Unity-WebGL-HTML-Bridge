using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class InputBridge : MonoBehaviour
{
    public static InputBridge Instance;

    [DllImport("__Internal")]
    private static extern void ShowHtmlInputWithPlaceholder(string placeholder, string inputId, string defaultValue);

    private static Dictionary<string, HtmlInputFieldHandler> inputHandlers = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static void RegisterInputField(string id, HtmlInputFieldHandler handler)
    {
        if (!inputHandlers.ContainsKey(id))
        {
            inputHandlers.Add(id, handler);
        }
    }

    public void ShowInputForField(string inputId, string placeholder)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLInput.captureAllKeyboardInput = false;

        string currentValue = "";
        if (inputHandlers.TryGetValue(inputId, out var handler))
        {
            currentValue = handler.tmpInputField.text;
        }

        ShowHtmlInputWithPlaceholder(placeholder, inputId, currentValue);
#endif
    }

    public void ReturnFromHtmlInput(string raw)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string[] parts = raw.Split('|');
        if (parts.Length != 2)
        {
            Debug.LogWarning("Invalid return value format: " + raw);
            return;
        }

        string id = parts[0];
        string value = parts[1];

        if (inputHandlers.TryGetValue(id, out var handler))
        {
            handler.ReceiveText(value);
            WebGLInput.captureAllKeyboardInput = true;
        }
        else
        {
            Debug.LogWarning("No input handler found for ID: " + id);
        }
#endif
    }
}
