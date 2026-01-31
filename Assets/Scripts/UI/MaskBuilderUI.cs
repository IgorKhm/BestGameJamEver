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
    
    [Header("Panel Control")]
    [Tooltip("Assign the X button to close the builder panel")]
    public Button closeButton;
    public BuilderPanelManager builderPanelManager;

    private readonly List<Row> _rows = new();

    private class Row
    {
        public FeatureDefinition feature;
        public TMP_Text nameText;
        public Button prevButton;
        public Button nextButton;
        public Image preview;
        public TMP_Text variantLabel; // Shows "1/4" or variant name
    }

    private void Start()
    {
        BuildRows();
        RefreshAll();
        
        // Wire up close button
        if (closeButton != null && builderPanelManager != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }
    }

    private void OnCloseButtonClicked()
    {
        if (builderPanelManager != null)
        {
            builderPanelManager.DisableBuilderPanel();
        }
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

            // Find the preview image (the one named VariantPreview or first non-button image)
            Image previewImg = null;
            foreach (var img in images)
            {
                if (img.gameObject.name == "VariantPreview" || 
                    (img.GetComponent<Button>() == null && img.transform.parent.GetComponent<Button>() == null))
                {
                    previewImg = img;
                    break;
                }
            }

            var row = new Row
            {
                feature = f,
                nameText = texts.Length > 0 ? texts[0] : null,
                preview = previewImg,
                prevButton = buttons.Length > 0 ? buttons[0] : null,
                nextButton = buttons.Length > 1 ? buttons[1] : null,
                variantLabel = texts.Length > 1 ? texts[1] : null // Optional second text for variant label
            };

            if (row.nameText != null)
                row.nameText.text = f.displayName;

            if (row.prevButton != null)
                row.prevButton.onClick.AddListener(() => ChangeVariant(f, -1));
            
            if (row.nextButton != null)
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
            int count = r.feature.variants.Count;
            var variant = r.feature.variants[idx];

            // Update preview image
            if (r.preview != null)
            {
                if (variant.sprite != null)
                {
                    r.preview.sprite = variant.sprite;
                    r.preview.color = Color.white;
                }
                else
                {
                    r.preview.sprite = null;
                    r.preview.color = variant.placeholderColor;
                }
            }

            // Update variant label (shows "1/4" format)
            if (r.variantLabel != null)
            {
                r.variantLabel.text = $"{idx + 1}/{count}";
            }
            
            // Update feature name to include variant info if no separate label
            if (r.variantLabel == null && r.nameText != null)
            {
                r.nameText.text = $"{r.feature.displayName} ({idx + 1}/{count})";
            }
        }
    }
}
