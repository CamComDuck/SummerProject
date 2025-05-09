using System.Collections.Generic;
using UnityEngine;

public class BiomeSelectionPanel : MonoBehaviour {

    [SerializeField] private BiomeButton biomePanelPrefab;
    [SerializeField] private GameObject biomePanelObject;
    [SerializeField] private BiomeSO testingSO;
    [SerializeField] private BiomeSO testingSO2;

    private List<BiomeButton> biomePanels = new List<BiomeButton>();
    private BiomeButton selectedPanel;
    private Color normalColor;
    private Color highlightedColor;

    void Start() {
        FindFirstObjectByType<ToolSelectionPanel>().OnNewToolSelected += ToolSelectionPanel_OnNewToolSelected;
        AddBiomePanel(testingSO);
        AddBiomePanel(testingSO2);
        normalColor = biomePanels[0].GetNormalColor();
        highlightedColor = biomePanels[0].GetHighlightedColor();
        SetSelectedBiome(biomePanels[0]);
        biomePanelObject.SetActive(false);
    }

    private void SetSelectedBiome(BiomeButton biomePanel) {
        if (selectedPanel != null) selectedPanel.SetColor(normalColor);
        selectedPanel = biomePanel;
        selectedPanel.SetColor(highlightedColor);
    }

    private void AddBiomePanel(BiomeSO biomeSO) {
        BiomeButton newBiomePanel = Instantiate(biomePanelPrefab, biomePanelObject.transform);
        newBiomePanel.SetupBiomePanel(biomeSO);
        newBiomePanel.OnBiomePanelClicked += BiomePanel_OnClicked;
        biomePanels.Add(newBiomePanel);
    }

    private void BiomePanel_OnClicked(object sender, System.EventArgs e) {
        SetSelectedBiome((BiomeButton)sender);
    }

    private void ToolSelectionPanel_OnNewToolSelected(object sender, System.EventArgs e) {
        ToolSelectionPanel toolSelectionPanel = (ToolSelectionPanel)sender;
        if (toolSelectionPanel.IsSelectedToolAction(ToolSO.Action.Place)) {
            biomePanelObject.SetActive(true);
        } else {
            biomePanelObject.SetActive(false);
        }

    }

    public BiomeSO GetSelectedBiome() {
        return selectedPanel.GetBiomeSO();
    }
}
