using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Masks/Feature Definition")]
public class FeatureDefinition : ScriptableObject
{
    public string featureId;              // e.g. "eyes"
    public string displayName = "Eyes";
    public List<VariantDefinition> variants = new();
}