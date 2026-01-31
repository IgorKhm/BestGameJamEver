using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Horizontal carousel that loads all sprites from a Resources folder.
/// Can also update a linked MaskView image.
/// </summary>
public class FolderBasedCarousel : MonoBehaviour
{
    [Header("Folder Settings")]
    [Tooltip("Folder name inside Resources (e.g., 'Noses' loads from Assets/Resources/Noses/)")]
    public string resourceFolderName = "Noses";
    
    [Header("Picker Display")]
    [Tooltip("The Image in the picker that shows current sprite")]
    public Image displayImage;
    
    [Tooltip("Mirrored Image in the picker (leave empty if not needed)")]
    public Image displayMirrorImage;
    
    [Header("MaskView Display")]
    [Tooltip("The Image in MaskView that should also update (e.g., Nose_Feature's image)")]
    public Image maskViewImage;
    
    [Tooltip("Mirrored Image in MaskView (for eyes, ears, etc.) - leave empty if not needed")]
    public Image maskViewMirrorImage;
    
    [Tooltip("If true, calls SetNativeSize on MaskView image. If false, keeps current size.")]
    public bool maskViewUseNativeSize = true;
    
    [Header("Animation")]
    [Range(0.1f, 3f)]
    public float slideDuration = 0.5f;
    public float slideDistance = 200f;
    
    [Header("Events")]
    public UnityEvent<int> onSpriteChanged;
    
    private Sprite[] sprites;
    private int currentIndex = 0;
    private bool isAnimating = false;
    private RectTransform displayRect;
    private RectTransform displayMirrorRect;

    public int CurrentIndex => currentIndex;
    public int Count => sprites != null ? sprites.Length : 0;
    public Sprite CurrentSprite => sprites != null && currentIndex < sprites.Length ? sprites[currentIndex] : null;

    void Start()
    {
        LoadSpritesFromFolder();
        
        if (displayImage != null)
            displayRect = displayImage.GetComponent<RectTransform>();
        
        if (displayMirrorImage != null)
            displayMirrorRect = displayMirrorImage.GetComponent<RectTransform>();
        
        ShowCurrentSprite();
    }

    void LoadSpritesFromFolder()
    {
        sprites = Resources.LoadAll<Sprite>(resourceFolderName);
        
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning($"[FolderBasedCarousel] No sprites found in Resources/{resourceFolderName}/");
        }
        else
        {
            Debug.Log($"[FolderBasedCarousel] Loaded {sprites.Length} sprites from Resources/{resourceFolderName}/");
        }
    }

    void ShowCurrentSprite()
    {
        if (sprites == null || sprites.Length == 0) return;
        
        Sprite current = sprites[currentIndex];
        
        // Update picker display
        if (displayImage != null)
        {
            displayImage.sprite = current;
            displayImage.SetNativeSize();
        }
        
        // Update picker mirror display
        if (displayMirrorImage != null)
        {
            displayMirrorImage.sprite = current;
            displayMirrorImage.SetNativeSize();
        }
        
        // Update MaskView display
        if (maskViewImage != null)
        {
            maskViewImage.sprite = current;
            if (maskViewUseNativeSize)
                maskViewImage.SetNativeSize();
        }
        
        // Update MaskView mirror image (if assigned)
        if (maskViewMirrorImage != null)
        {
            maskViewMirrorImage.sprite = current;
            if (maskViewUseNativeSize)
                maskViewMirrorImage.SetNativeSize();
        }
        
        onSpriteChanged?.Invoke(currentIndex);
    }

    public void ShowNext()
    {
        if (isAnimating || sprites == null || sprites.Length == 0) return;
        int nextIndex = (currentIndex + 1) % sprites.Length;
        
        if (displayImage != null && displayRect != null)
        {
            StartCoroutine(AnimateToIndex(nextIndex, true));
        }
        else
        {
            // No animation, just switch
            currentIndex = nextIndex;
            ShowCurrentSprite();
        }
    }

    public void ShowPrevious()
    {
        if (isAnimating || sprites == null || sprites.Length == 0) return;
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = sprites.Length - 1;
        
        if (displayImage != null && displayRect != null)
        {
            StartCoroutine(AnimateToIndex(prevIndex, false));
        }
        else
        {
            // No animation, just switch
            currentIndex = prevIndex;
            ShowCurrentSprite();
        }
    }

    IEnumerator AnimateToIndex(int newIndex, bool slideLeft)
    {
        isAnimating = true;
        
        float dir = slideLeft ? 1f : -1f;
        Vector2 startPos = Vector2.zero;
        Vector2 exitPos = new Vector2(-dir * slideDistance, 0);
        Vector2 enterPos = new Vector2(dir * slideDistance, 0);
        
        // Mirror moves same direction as main
        Vector2 mirrorExitPos = exitPos;
        Vector2 mirrorEnterPos = enterPos;
        
        // Slide out
        float time = 0f;
        while (time < slideDuration / 2f)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / (slideDuration / 2f));
            float smooth = Mathf.SmoothStep(0, 1, t);
            
            displayRect.anchoredPosition = Vector2.Lerp(startPos, exitPos, smooth);
            if (displayMirrorRect != null)
                displayMirrorRect.anchoredPosition = Vector2.Lerp(startPos, mirrorExitPos, smooth);
            
            yield return null;
        }
        
        // Swap sprite
        currentIndex = newIndex;
        ShowCurrentSprite();
        displayRect.anchoredPosition = enterPos;
        if (displayMirrorRect != null)
            displayMirrorRect.anchoredPosition = mirrorEnterPos;
        
        // Slide in
        time = 0f;
        while (time < slideDuration / 2f)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / (slideDuration / 2f));
            float smooth = Mathf.SmoothStep(0, 1, t);
            
            displayRect.anchoredPosition = Vector2.Lerp(enterPos, startPos, smooth);
            if (displayMirrorRect != null)
                displayMirrorRect.anchoredPosition = Vector2.Lerp(mirrorEnterPos, startPos, smooth);
            
            yield return null;
        }
        
        displayRect.anchoredPosition = Vector2.zero;
        if (displayMirrorRect != null)
            displayMirrorRect.anchoredPosition = Vector2.zero;
        
        isAnimating = false;
    }

    public void SetIndex(int index)
    {
        if (sprites == null || index < 0 || index >= sprites.Length) return;
        currentIndex = index;
        ShowCurrentSprite();
    }
}
