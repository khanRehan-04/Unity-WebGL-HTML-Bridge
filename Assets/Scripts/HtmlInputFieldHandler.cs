using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_InputField))]
public class HtmlInputFieldHandler : MonoBehaviour
{
    public TMP_InputField tmpInputField { get; private set; }
    private string inputFieldId;

    private void Awake()
    {
        tmpInputField = GetComponent<TMP_InputField>();

        // Generate a unique ID based on hierarchy path
        inputFieldId = GetHierarchyPath(transform);

        InputBridge.RegisterInputField(inputFieldId, this);

#if UNITY_WEBGL && !UNITY_EDITOR
        tmpInputField.onSelect.AddListener(OnInputFieldFocus);
#endif
    }

    private string GetHierarchyPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }

    public void OnInputFieldFocus(string _)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string placeholder = tmpInputField.placeholder?.GetComponent<TextMeshProUGUI>()?.text ?? "";
        InputBridge.Instance.ShowInputForField(inputFieldId, placeholder);
        tmpInputField.DeactivateInputField();
#endif
    }

    public void ReceiveText(string value)
    {
        tmpInputField.text = value;
        tmpInputField.ActivateInputField();
    }
}
