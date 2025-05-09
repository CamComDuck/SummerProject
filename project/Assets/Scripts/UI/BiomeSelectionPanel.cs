using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class BiomeSelectionPanel : MonoBehaviour {

    [SerializeField] private BiomePanel biomePanelPrefab;
    [SerializeField] private GameObject biomePanelObject;
    [SerializeField] private BiomeSO testingSO;
    [SerializeField] private BiomeSO testingSO2;

    private List<BiomePanel> biomePanels = new List<BiomePanel>();
    private BiomePanel selectedPanel;
    private Color normalColor;
    private Color selectedColor;

    void Start() {
        FindFirstObjectByType<ToolSelectionPanel>().OnNewToolSelected += toolSelectionPanel_OnNewToolSelected;
        AddBiomePanel(testingSO);
        AddBiomePanel(testingSO2);
        normalColor = biomePanels[0].GetNormalColor();
        selectedColor = biomePanels[0].GetSelectedColor();
        SetSelectedBiome(biomePanels[0]);
    }

    private void SetSelectedBiome(BiomePanel biomePanel) {
        if (selectedPanel != null) selectedPanel.SetColor(normalColor);
        selectedPanel = biomePanel;
        selectedPanel.SetColor(selectedColor);
    }

    private void AddBiomePanel(BiomeSO biomeSO) {
        BiomePanel newBiomePanel = Instantiate(biomePanelPrefab, biomePanelObject.transform);
        newBiomePanel.SetupBiomePanel(biomeSO);
        newBiomePanel.OnBiomePanelClicked += BiomePanel_OnClicked;
        biomePanels.Add(newBiomePanel);
    }

    private void BiomePanel_OnClicked(object sender, System.EventArgs e) {
        SetSelectedBiome((BiomePanel)sender);
    }

    private void toolSelectionPanel_OnNewToolSelected(object sender, System.EventArgs e) {
        ToolSelectionPanel toolSelectionPanel = (ToolSelectionPanel)sender;
        if (toolSelectionPanel.GetSelectedTool() == ToolSelectionPanel.Tools.Place) {
            biomePanelObject.SetActive(true);
        } else {
            biomePanelObject.SetActive(false);
        }

    }

    public BiomeSO GetSelectedBiome() {
        return selectedPanel.GetBiomeSO();
    }
}
