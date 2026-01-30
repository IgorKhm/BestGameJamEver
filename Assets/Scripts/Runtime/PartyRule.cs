using System.Collections.Generic;

public class PartyRule
{
    public List<FeatureDefinition> relevantFeatures = new();
    public Dictionary<FeatureDefinition, int> requiredVariantIndex = new();
}