using UnityEngine;

[CreateAssetMenu(menuName = "Masks/Variant Definition")]
public class VariantDefinition : ScriptableObject
{
    public string variantId;              // e.g. "eyes_01"
    public Sprite sprite;                 // optional for later
    public Color placeholderColor = Color.white;
}