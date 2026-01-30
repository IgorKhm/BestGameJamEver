using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaskBuilderUI : MonoBehaviour
{
    public GameManager gameManager;

    [Header("UI")]
    public Transform rowsParent;
    public GameObject featureRowPrefab;

    private readonly List<Row> _rows = new();

    private class Row
    {
        public FeatureDefinition feature;
        public TMP_Text nameText;
        public Button prevButton;
        public Button nextButton;
        public Image preview;
    }

    private void Start()
    {
        BuildRows();
        RefreshAll();
    }

    private void BuildRows()
    {
        foreach (Transform child in rowsParent) Destroy(child.gameObject);
        _rows.Clear();

        var features = gameManager.allFeatures;
        var mask = gameManager.GetPlayerMask();
        mask.EnsureAllFeatures(features);

        foreach (var f in features)
        {
            var go = Instantiate(featureRowPrefab, rowsParent);
            var texts = go.GetComponentsInChildren<TMP_Text>();
            var buttons = go.GetComponentsInChildren<Button>();
            var images = go.GetComponentsInChildren<Image>();

            // Assumes: first TMP_Text is name, first Image is preview, two Buttons are prev/next.
            var row = new Row
            {
                feature = f,
                nameText = texts[0],
                preview = images[0],
                prevButton = buttons[0],
                nextButton = buttons[1]
            };

            row.nameText.text = f.displayName;

            row.prevButton.onClick.AddListener(() => ChangeVariant(f, -1));
            row.nextButton.onClick.AddListener(() => ChangeVariant(f, +1));

            _rows.Add(row);
        }
    }

    private void ChangeVariant(FeatureDefinition f, int delta)
    {
        var mask = gameManager.GetPlayerMask();
        int idx = mask.GetVariantIndex(f);
        int count = f.variants.Count;
        idx = (idx + delta) % count;
        if (idx < 0) idx += count;

        mask.SetVariantIndex(f, idx);
        RefreshAll();
    }

    private void RefreshAll()
    {
        var mask = gameManager.GetPlayerMask();

        foreach (var r in _rows)
        {
            int idx = mask.GetVariantIndex(r.feature);
            var v = r.feature.variants[idx];
            r.preview.color = v.placeholderColor;
        }
    }
}
