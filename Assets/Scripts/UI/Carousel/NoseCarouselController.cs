using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple nose carousel for testing.
/// Cycles through nose variants and updates Nose1/Nose2 display images.
/// </summary>
public class NoseCarouselController : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public FeatureDefinition noseFeature;
    
    [Header("Display Images")]
    [Tooltip("Image showing the current nose variant")]
    public Image nose1Image;
    [Tooltip("Image showing the next nose variant (optional preview)")]
    public Image nose2Image;
    
    [Header("Navigation")]
    public Button prevButton;
    public Button nextButton;

    private int _currentIndex = 0;

    private void Start()
    {
        if (prevButton != null)
            prevButton.onClick.AddListener(() => ChangeVariant(-1));
        
        if (nextButton != null)
            nextButton.onClick.AddListener(() => ChangeVariant(+1));
        
        RefreshDisplay();
    }

    private void ChangeVariant(int delta)
    {
        if (noseFeature == null || noseFeature.variants.Count == 0) return;
        
        int count = noseFeature.variants.Count;
        _currentIndex = (_currentIndex + delta) % count;
        if (_currentIndex < 0) _currentIndex += count;
        
        // Update the player mask if GameManager exists
        if (gameManager != null)
        {
            var mask = gameManager.GetPlayerMask();
            mask.SetVariantIndex(noseFeature, _currentIndex);
        }
        
        RefreshDisplay();
    }

    private void RefreshDisplay()
    {
        if (noseFeature == null || noseFeature.variants.Count == 0) return;
        
        int count = noseFeature.variants.Count;
        
        // Show current variant on Nose1
        var currentVariant = noseFeature.variants[_currentIndex];
        UpdateImage(nose1Image, currentVariant);
        
        // Show next variant on Nose2 (preview)
        if (nose2Image != null)
        {
            int nextIndex = (_currentIndex + 1) % count;
            var nextVariant = noseFeature.variants[nextIndex];
            UpdateImage(nose2Image, nextVariant);
        }
    }

    private void UpdateImage(Image img, VariantDefinition variant)
    {
        if (img == null || variant == null) return;
        
        if (variant.sprite != null)
        {
            img.sprite = variant.sprite;
            img.color = Color.white;
        }
        else
        {
            img.sprite = null;
            img.color = variant.placeholderColor;
        }
    }

    /// <summary>
    /// Select a specific variant by clicking on it.
    /// Call this from button OnClick events.
    /// </summary>
    public void SelectVariant(int index)
    {
        if (noseFeature == null) return;
        
        int count = noseFeature.variants.Count;
        _currentIndex = Mathf.Clamp(index, 0, count - 1);
        
        if (gameManager != null)
        {
            var mask = gameManager.GetPlayerMask();
            mask.SetVariantIndex(noseFeature, _currentIndex);
        }
        
        RefreshDisplay();
    }
}
