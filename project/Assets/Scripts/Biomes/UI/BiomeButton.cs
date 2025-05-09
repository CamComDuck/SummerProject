using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomeButton : MonoBehaviour {

    public event EventHandler OnBiomePanelClicked;
    
    [SerializeField] private TMP_Text biomeName;
    [SerializeField] private Image biomeIcon;

    private BiomeSO biomeSO;

    private void Start() {
        GetComponent<Button>().onClick.AddListener(OnClicked);
    }

    public void SetupBiomePanel(BiomeSO newBiomeSO) {
        biomeSO = newBiomeSO;

        biomeName.text = biomeSO.GetName();
        biomeIcon.sprite = biomeSO.GetIcon();
    }

    public BiomeSO GetBiomeSO() {
        return biomeSO;
    }

    private void OnClicked() {
        OnBiomePanelClicked?.Invoke(this, EventArgs.Empty);
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
}
