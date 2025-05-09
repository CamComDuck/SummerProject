using System;
using UnityEngine;
using UnityEngine.UI;

public class ToolSelectionPanel : MonoBehaviour {
    
    public event EventHandler OnNewToolSelected;

    [SerializeField] private GameObject selectButton;
    [SerializeField] private GameObject placeButton;
    [SerializeField] private GameObject destroyButton;

    private Tools selectedTool;
    private Color normalColor;
    private Color selectedColor;

    public enum Tools {
        Select,
        Place,
        Destroy
    }

    private void Awake() {
        normalColor = selectButton.GetComponent<Button>().colors.normalColor;
        selectedColor = selectButton.GetComponent<Button>().colors.selectedColor;
    }

    private void Start() {
        selectButton.GetComponent<Button>().onClick.AddListener(SelectToolClicked);
        placeButton.GetComponent<Button>().onClick.AddListener(PlaceToolClicked);
        destroyButton.GetComponent<Button>().onClick.AddListener(DestroyToolClicked);
        SelectToolClicked();
    }

    private void SelectToolClicked() {
        SetSelectedTool(Tools.Select);
    }

    private void PlaceToolClicked() {
        SetSelectedTool(Tools.Place);
    }

    private void DestroyToolClicked() {
        SetSelectedTool(Tools.Destroy);
    }

    private void SetSelectedTool(Tools newTool) {
        SetButtonColor(normalColor);
        selectedTool = newTool;
        OnNewToolSelected?.Invoke(this, EventArgs.Empty);
        SetButtonColor(selectedColor);
    }

    private void SetButtonColor(Color color) {
        switch (selectedTool) {
            case Tools.Select:
                selectButton.GetComponent<Image>().color = color;
                break;

            case Tools.Place:
                placeButton.GetComponent<Image>().color = color;
                break;

            case Tools.Destroy:
                destroyButton.GetComponent<Image>().color = color;
                break;
        }
    }

    public Tools GetSelectedTool() {
        return selectedTool;
    }
}
