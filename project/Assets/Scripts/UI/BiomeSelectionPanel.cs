using System.Collections.Generic;
using UnityEngine;

public class BiomeSelectionPanel : MonoBehaviour {

    [SerializeField] private BiomePanel biomePanelPrefab;
    [SerializeField] private BiomeSO testingSO;
    [SerializeField] private BiomeSO testingSO2;

    private List<BiomePanel> biomePanels = new List<BiomePanel>();
    private BiomeSO selectedBiome;

    void Start() {
        AddBiomePanel(testingSO);
        AddBiomePanel(testingSO2);
        selectedBiome = biomePanels[0].GetBiomeSO();
    }

    private void AddBiomePanel(BiomeSO biomeSO) {
        BiomePanel newBiomePanel = Instantiate(biomePanelPrefab, this.transform);
        newBiomePanel.SetupBiomePanel(biomeSO);
        newBiomePanel.OnBiomePanelClicked += biomePanel_OnClicked;
        biomePanels.Add(newBiomePanel);
    }

    private void biomePanel_OnClicked(object sender, System.EventArgs e) {
        BiomePanel panelClicked = (BiomePanel)sender;
        selectedBiome = panelClicked.GetBiomeSO();
    }

    public BiomeSO GetSelectedBiome() {
        return selectedBiome;
    }
}
