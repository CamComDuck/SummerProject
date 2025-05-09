using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolButton : MonoBehaviour {
    
    public event EventHandler OnToolButtonClicked;
    
    [SerializeField] private Image image;

    private ToolSO tool;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    private void OnClicked() {
        OnToolButtonClicked?.Invoke(this, EventArgs.Empty);
    }

    public void SetupToolButton(ToolSO toolSO) {
        image.sprite = toolSO.GetIcon();
        tool = toolSO;
    }

    public bool IsToolSO(ToolSO t) {
        return t == tool;
    }

    public ToolSO GetToolSO() {
        return tool;
    }

    public void SetColor(Color newColor) {
        GetComponent<Image>().color = newColor;
    }

    public Color GetNormalColor() {
        return GetComponent<Button>().colors.normalColor;
    }

    public Color GetHighlightedColor() {
        return GetComponent<Button>().colors.highlightedColor;
    }

    public void SetButtonEnabled(bool isEnabled) {
        GetComponent<Button>().interactable = isEnabled;
    }
}
