using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiomePanel : MonoBehaviour {

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
}
