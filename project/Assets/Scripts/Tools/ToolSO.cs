using UnityEngine;

[CreateAssetMenu(fileName = "ToolSO", menuName = "Scriptable Objects/ToolSO")]
public class ToolSO : ScriptableObject {

    [SerializeField] private string toolName;
    [SerializeField] private Sprite icon;
    [SerializeField] private Action action;

    public enum Action {
        Select,
        Place,
        Destroy
    }

    public string GetName() {
        return toolName;
    }

    public Sprite GetIcon() {
        return icon;
    }

    public bool IsAction(Action a) {
        return action == a;
    }
    
}
