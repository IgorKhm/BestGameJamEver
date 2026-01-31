using System;
using System.Collections.Generic;
using UnityEngine;

public class MaskBuilderToMaskDataBridge : MonoBehaviour
{
    [Serializable]
    public class Binding
    {
        public FeatureDefinition feature;

        // Assign ONE of these per feature (depending on which system that picker uses)
        public FolderBasedCarousel folderCarousel;
        public HorizontalCarousel horizontalCarousel;

        [Tooltip("If true, we first try to map by sprite reference/name; if not found, fallback to same index.")]
        public bool mapBySprite = true;

        [Tooltip("Optional: log mappings to console.")]
        public bool debug;
    }

    public GameManager gameManager;
    public List<Binding> bindings = new();

    [Header("Sync behavior")]
    public bool syncUIFromMaskOnEnable = true;   // when builder opens, set carousels to current MaskData
    public bool syncMaskFromUIOnStart = true;    // when scene starts, push current UI selection into MaskData

    private void Awake()
    {
        // Subscribe to change events
        foreach (var b in bindings)
        {
            if (b == null || b.feature == null) continue;

            if (b.folderCarousel != null)
                b.folderCarousel.onSpriteChanged.AddListener((idx) => ApplyFolderChoice(b, idx));

            if (b.horizontalCarousel != null)
                b.horizontalCarousel.onItemChanged.AddListener((idx) => ApplyHorizontalChoice(b, idx));
        }
    }

    private void Start()
    {
        if (syncMaskFromUIOnStart)
            SyncMaskFromUI();
    }

    private void OnEnable()
    {
        if (syncUIFromMaskOnEnable)
            SyncUIFromMask();
    }

    public void SyncUIFromMask()
    {
        if (gameManager == null) return;

        var mask = gameManager.GetPlayerMask();
        mask.EnsureAllFeatures(gameManager.allFeatures);

        foreach (var b in bindings)
        {
            if (b == null || b.feature == null) continue;

            int idx = mask.GetVariantIndex(b.feature);

            if (b.folderCarousel != null)
                b.folderCarousel.SetIndex(idx);

            if (b.horizontalCarousel != null)
                b.horizontalCarousel.SetIndex(idx);
        }
    }

    public void SyncMaskFromUI()
    {
        foreach (var b in bindings)
        {
            if (b == null || b.feature == null) continue;

            if (b.folderCarousel != null)
                ApplyFolderChoice(b, b.folderCarousel.CurrentIndex);

            if (b.horizontalCarousel != null)
                ApplyHorizontalChoice(b, b.horizontalCarousel.CurrentIndex);
        }
    }

    private void ApplyFolderChoice(Binding b, int uiIndex)
    {
        if (gameManager == null) return;

        var mask = gameManager.GetPlayerMask();
        var sprite = b.folderCarousel != null ? b.folderCarousel.CurrentSprite : null;

        int variantIndex = ResolveVariantIndex(b, uiIndex, sprite);

        mask.SetVariantIndex(b.feature, variantIndex);

        if (b.debug)
            Debug.Log($"[Bridge] {b.feature.featureId} <- folder idx {uiIndex} -> variant {variantIndex} (sprite={sprite?.name})");
    }

    private void ApplyHorizontalChoice(Binding b, int uiIndex)
    {
        if (gameManager == null) return;

        var mask = gameManager.GetPlayerMask();

        // HorizontalCarousel doesn’t expose “current sprite” directly, but it does have maskViewSprites[]
        Sprite sprite = null;
        if (b.horizontalCarousel != null && b.horizontalCarousel.maskViewSprites != null &&
            uiIndex >= 0 && uiIndex < b.horizontalCarousel.maskViewSprites.Length)
        {
            sprite = b.horizontalCarousel.maskViewSprites[uiIndex];
        }

        int variantIndex = ResolveVariantIndex(b, uiIndex, sprite);

        mask.SetVariantIndex(b.feature, variantIndex);

        if (b.debug)
            Debug.Log($"[Bridge] {b.feature.featureId} <- horizontal idx {uiIndex} -> variant {variantIndex} (sprite={sprite?.name})");
    }

    private int ResolveVariantIndex(Binding b, int fallbackIndex, Sprite chosenSprite)
    {
        var feature = b.feature;
        if (feature == null || feature.variants == null || feature.variants.Count == 0)
            return 0;

        // Best effort: map by sprite reference/name
        if (b.mapBySprite && chosenSprite != null)
        {
            for (int i = 0; i < feature.variants.Count; i++)
            {
                var v = feature.variants[i];
                if (v == null) continue;

                // exact reference match
                if (v.variantId == chosenSprite.name) return i;

                // name match fallback (useful if loaded via Resources or duplicated)
                if (v.sprite != null && v.sprite.name == chosenSprite.name) return i;
            }
        }

        // Fallback: assume ordering matches
        return Mathf.Clamp(fallbackIndex, 0, feature.variants.Count - 1);
    }
}
