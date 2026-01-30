using System.Collections.Generic;
using UnityEngine;

public static class PartyRuleGenerator
{
    public static PartyRule Generate(List<FeatureDefinition> allFeatures, int relevantCount)
    {
        var rule = new PartyRule();

        var pool = new List<FeatureDefinition>(allFeatures);
        Shuffle(pool);

        relevantCount = Mathf.Clamp(relevantCount, 1, pool.Count);

        for (int i = 0; i < relevantCount; i++)
        {
            var f = pool[i];
            rule.relevantFeatures.Add(f);
            int idx = Random.Range(0, f.variants.Count);
            rule.requiredVariantIndex[f] = idx;
        }

        return rule;
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}