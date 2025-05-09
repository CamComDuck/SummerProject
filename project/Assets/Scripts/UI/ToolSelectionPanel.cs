using UnityEngine;
using UnityEngine.UI;

public class ToolSelectionPanel : MonoBehaviour {

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
        SelectToolClicked();
    }

    private void Start() {
        selectButton.GetComponent<Button>().onClick.AddListener(SelectToolClicked);
        placeButton.GetComponent<Button>().onClick.AddListener(PlaceToolClicked);
        destroyButton.GetComponent<Button>().onClick.AddListener(DestroyToolClicked);
    }

    private void SelectToolClicked() {
        SetButtonColor(normalColor);
        selectedTool = Tools.Select;
        SetButtonColor(selectedColor);
    }

    private void PlaceToolClicked() {
        SetButtonColor(normalColor);
        selectedTool = Tools.Place;
        SetButtonColor(selectedColor);
    }

    private void DestroyToolClicked() {
        SetButtonColor(normalColor);
        selectedTool = Tools.Destroy;
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
