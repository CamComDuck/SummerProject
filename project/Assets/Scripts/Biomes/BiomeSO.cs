using UnityEngine;

[CreateAssetMenu(fileName = "BiomeSO", menuName = "Scriptable Objects/BiomeSO")]
public class BiomeSO : ScriptableObject {
    
    [SerializeField] private string biomeName;
    [SerializeField] private Transform prefab;
    [SerializeField] private Sprite icon;

    public string GetName() {
        return biomeName;
    }

    public Transform GetPrefab() {
        return prefab;
    }

    public Sprite GetIcon() {
        return icon;
    }
}
