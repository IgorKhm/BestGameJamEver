using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaskChoice
{
    public FeatureDefinition feature;
    public int variantIndex; // index into feature.variants
}

[Serializable]
public class MaskData
{
    Dictionary<string, int> variant_choice = new();
    public List<MaskChoice> choices = new();

    public void EnsureAllFeatures(List<FeatureDefinition> allFeatures)
    {
        foreach (var f in allFeatures)
        {
            if (choices.Exists(c => c.feature == f)) continue;
            choices.Add(new MaskChoice { feature = f, variantIndex = 0 });
        }
    }

    public int GetVariantIndex(FeatureDefinition f)
    {
        var c = choices.Find(x => x.feature == f);
        return c != null ? c.variantIndex : 0;
    }

    public void SetVariantIndex(FeatureDefinition f, int idx)
    {
        var c = choices.Find(x => x.feature == f);
        if (c == null)
        {
            choices.Add(new MaskChoice { feature = f, variantIndex = idx });
            return;
        }

        c.variantIndex = idx;
    }
}