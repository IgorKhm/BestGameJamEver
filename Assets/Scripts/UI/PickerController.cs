using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls which picker responds to left/right buttons.
/// Supports both HorizontalCarousel and FolderBasedCarousel.
/// Only the currently visible picker responds.
/// </summary>
public class PickerController : MonoBehaviour
{
    [Header("Vertical Carousel")]
    public VerticalCarousel verticalCarousel;
    
    [Header("Pickers (same order as vertical carousel)")]
    [Tooltip("Drag the picker GameObjects here - they can have either HorizontalCarousel or FolderBasedCarousel")]
    public GameObject[] pickers;
    
    [Header("Shared Navigation Buttons")]
    public Button leftButton;
    public Button rightButton;
    
    [Header("Debug")]
    public bool debugMode = true;

    void Start()
    {
        // Remove button listeners from individual carousels so only this controller handles them
        foreach (var picker in pickers)
        {
            if (picker == null) continue;
            
            var hCarousel = picker.GetComponent<HorizontalCarousel>();
            if (hCarousel != null)
            {
                hCarousel.leftButton = null;
                hCarousel.rightButton = null;
            }
        }
        
        if (leftButton != null)
            leftButton.onClick.AddListener(OnLeftClicked);
        
        if (rightButton != null)
            rightButton.onClick.AddListener(OnRightClicked);
        
        if (debugMode)
            Debug.Log($"[PickerController] Ready with {pickers.Length} pickers");
    }

    void OnLeftClicked()
    {
        int index = verticalCarousel != null ? verticalCarousel.CurrentIndex : 0;
        
        if (debugMode) Debug.Log($"[PickerController] LEFT - vertical index: {index}");
        
        if (pickers == null || index < 0 || index >= pickers.Length || pickers[index] == null)
        {
            if (debugMode) Debug.LogError($"[PickerController] Invalid picker at index {index}");
            return;
        }
        
        var picker = pickers[index];
        
        // Try FolderBasedCarousel first
        var folderCarousel = picker.GetComponent<FolderBasedCarousel>();
        if (folderCarousel != null)
        {
            if (debugMode) Debug.Log($"[PickerController] Calling ShowPrevious on FolderBasedCarousel: {picker.name}");
            folderCarousel.ShowPrevious();
            return;
        }
        
        // Try HorizontalCarousel
        var hCarousel = picker.GetComponent<HorizontalCarousel>();
        if (hCarousel != null)
        {
            if (debugMode) Debug.Log($"[PickerController] Calling ShowPrevious on HorizontalCarousel: {picker.name}");
            hCarousel.ShowPrevious();
            return;
        }
        
        if (debugMode) Debug.LogWarning($"[PickerController] No carousel found on {picker.name}");
    }

    void OnRightClicked()
    {
        int index = verticalCarousel != null ? verticalCarousel.CurrentIndex : 0;
        
        if (debugMode) Debug.Log($"[PickerController] RIGHT - vertical index: {index}");
        
        if (pickers == null || index < 0 || index >= pickers.Length || pickers[index] == null)
        {
            if (debugMode) Debug.LogError($"[PickerController] Invalid picker at index {index}");
            return;
        }
        
        var picker = pickers[index];
        
        // Try FolderBasedCarousel first
        var folderCarousel = picker.GetComponent<FolderBasedCarousel>();
        if (folderCarousel != null)
        {
            if (debugMode) Debug.Log($"[PickerController] Calling ShowNext on FolderBasedCarousel: {picker.name}");
            folderCarousel.ShowNext();
            return;
        }
        
        // Try HorizontalCarousel
        var hCarousel = picker.GetComponent<HorizontalCarousel>();
        if (hCarousel != null)
        {
            if (debugMode) Debug.Log($"[PickerController] Calling ShowNext on HorizontalCarousel: {picker.name}");
            hCarousel.ShowNext();
            return;
        }
        
        if (debugMode) Debug.LogWarning($"[PickerController] No carousel found on {picker.name}");
    }
}
