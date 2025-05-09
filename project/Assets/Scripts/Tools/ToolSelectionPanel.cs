using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolSelectionPanel : MonoBehaviour {
    
    public event EventHandler OnNewToolSelected;

    [SerializeField] private List<ToolSO> tools = new List<ToolSO>();
    [SerializeField] private ToolButton toolButtonPrefab;

    private ToolButton selectedTool;
    private Color normalColor;
    private Color highlightedColor;
    private List<ToolButton> toolButtons = new List<ToolButton>();

    private void Start() {
        foreach (ToolSO toolSO in tools) {
            ToolButton newToolButton = Instantiate(toolButtonPrefab, this.transform);
            newToolButton.SetupToolButton(toolSO);
            newToolButton.OnToolButtonClicked += ToolButton_OnToolButtonClicked;
            toolButtons.Add(newToolButton);
        }
        
        normalColor = toolButtons[0].GetNormalColor();
        highlightedColor = toolButtons[0].GetHighlightedColor();
        SetDestroyEnabled(false);
    }

    private void ToolButton_OnToolButtonClicked(object sender, System.EventArgs e) {
        SetSelectedTool((ToolButton)sender);
    }

    private void SetSelectedTool(ToolButton newTool) {
        if (selectedTool != null) selectedTool.SetColor(normalColor);
        selectedTool = newTool;
        OnNewToolSelected?.Invoke(this, EventArgs.Empty);
        selectedTool.SetColor(highlightedColor);
    }

    public ToolSO GetSelectedToolSO() {
        return selectedTool.GetToolSO();
    }

    public bool IsSelectedToolAction(ToolSO.Action action) {
        return selectedTool.GetToolSO().IsAction(action);
    }

    public void SetDestroyEnabled(bool isEnabled) {
        ToolButton destroyButton = GetToolButton(ToolSO.Action.Destroy);
        destroyButton.SetButtonEnabled(isEnabled);
        if (!isEnabled) {
            SetSelectedTool(GetToolButton(ToolSO.Action.Select));
        }
    }

    private ToolButton GetToolButton(ToolSO.Action action) {
        foreach (ToolButton button in toolButtons) {
            if (button.GetToolSO().IsAction(action)) {
                return button;
            }
        }
        return null;
    }
}
