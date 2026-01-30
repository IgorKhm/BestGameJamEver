using System;
using System.Collections.Generic;
using UnityEngine;

public class NpcFaceMaskRenderer : MonoBehaviour
{
    [Serializable]
    public class FeatureSlot
    {
        public string featureId;           // must match FeatureDefinition.featureId (e.g. "eyes")
        public SpriteRenderer renderer;    // the SpriteRenderer placed on the face
        public int orderInLayer = 0;       // optional override
    }

    [Header("Slots (one per feature)")]
    public List<FeatureSlot> slots = new();

    [Header("Fallback")]
    public Sprite fallbackSprite;          // tinted square if no sprite assigned in variant

    private Dictionary<string, FeatureSlot> _map;

    private void Awake()
    {
        BuildMap();
    }

    private void BuildMap()
    {
        _map = new Dictionary<string, FeatureSlot>(StringComparer.OrdinalIgnoreCase);

        foreach (var s in slots)
        {
            if (string.IsNullOrWhiteSpace(s.featureId) || s.renderer == null) continue;
            _map[s.featureId.Trim()] = s;
            s.renderer.sortingOrder = s.orderInLayer;
        }
    }

    public void Apply(MaskData mask)
    {
        if (mask == null || mask.choices == null) return;
        if (_map == null) BuildMap();

        foreach (var choice in mask.choices)
        {
            if (choice.feature == null) continue;

            var fid = choice.feature.featureId;
            if (string.IsNullOrEmpty(fid)) continue;

            if (!_map.TryGetValue(fid, out var slot)) continue;

            var variants = choice.feature.variants;
            if (variants == null || variants.Count == 0) continue;

            int idx = Mathf.Clamp(choice.variantIndex, 0, variants.Count - 1);
            var v = variants[idx];

            if (v.sprite != null)
            {
                slot.renderer.sprite = v.sprite;
                slot.renderer.color = Color.white;
            }
            else
            {
                slot.renderer.sprite = fallbackSprite;
                slot.renderer.color = v.placeholderColor;
            }

            slot.renderer.enabled = true;
        }
    }
}
