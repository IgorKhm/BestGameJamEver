using System.Collections.Generic;
using UnityEngine;

public class NpcMaskRenderer : MonoBehaviour
{
    [Header("Look")]
    public Sprite fallbackSprite;                 // assign Sprite_SquareFallback
    public Vector2 startLocalOffset = new(0f, 1.2f); // above NPC
    public float xSpacing = 0.45f;                // distance between icons
    public float iconScale = 0.6f;                // size of each icon

    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int baseOrderInLayer = 20;             // above NPC body

    private readonly List<SpriteRenderer> _icons = new();
    private MaskData _lastMask;

    // Call this once after you have a mask (it creates the icon renderers)
    public void EnsureIcons(MaskData mask)
    {
        if (mask == null || mask.choices == null) return;

        // Rebuild if count changed
        if (_icons.Count == mask.choices.Count) return;

        // Clear old
        for (int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);

        _icons.Clear();

        for (int i = 0; i < mask.choices.Count; i++)
        {
            var go = new GameObject($"Icon_{i}");
            go.transform.SetParent(transform, false);

            go.transform.localPosition = startLocalOffset + new Vector2(i * xSpacing, 0f);
            go.transform.localScale = Vector3.one * iconScale;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.transform.localPosition = new Vector3(sr.transform.localPosition.x, sr.transform.localPosition.y,
                baseOrderInLayer + i);
            // sr.sortingLayerName = sortingLayerName;
            // sr.sortingOrder = baseOrderInLayer + i;

            _icons.Add(sr);
        }
    }

    public void Apply(MaskData mask)
    {
        if (mask == null || mask.choices == null) return;

        EnsureIcons(mask);

        for (int i = 0; i < mask.choices.Count; i++)
        {
            var choice = mask.choices[i];
            if (choice.feature == null) continue;

            var variants = choice.feature.variants;
            if (variants == null || variants.Count == 0) continue;

            int idx = Mathf.Clamp(choice.variantIndex, 0, variants.Count - 1);
            var v = variants[idx];

            var sr = _icons[i];

            // Sprite if exists; otherwise fallback tinted square
            if (v.sprite != null)
            {
                sr.sprite = v.sprite;
                sr.color = Color.white;
            }
            else
            {
                sr.sprite = fallbackSprite;
                sr.color = v.placeholderColor;
            }
        }

        _lastMask = mask;
    }
}
