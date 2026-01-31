using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class HorizontalCarousel : MonoBehaviour
{
    [Header("Items")]
    public RectTransform[] items;
    
    [Header("Navigation")]
    public Button leftButton;
    public Button rightButton;
    
    [Header("Animation")]
    [Range(0.1f, 3f)]
    public float slideDuration = 0.6f;
    public float slideDistance = 200f;
    
    [Header("MaskView Sync")]
    [Tooltip("The Image in MaskView to update when carousel changes")]
    public Image maskViewImage;
    
    [Tooltip("Mirrored Image in MaskView (for eyes, ears, horns, etc.)")]
    public Image maskViewMirrorImage;
    
    [Tooltip("Sprites to show in MaskView (same order as items)")]
    public Sprite[] maskViewSprites;
    
    [Tooltip("Use SetNativeSize on MaskView image")]
    public bool useNativeSize = true;
    
    [Tooltip("Reset position to 0,0 (disable for cropped sprites that have their own position)")]
    public bool centerPosition = false;
    
    [Header("Events")]
    public UnityEvent<int> onItemChanged;
    
    private int currentIndex = 0;
    private bool isAnimating = false;

    public int CurrentIndex => currentIndex;

    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                items[i].gameObject.SetActive(i == 0);
        }
        
        if (leftButton != null)
            leftButton.onClick.AddListener(ShowPrevious);
        
        if (rightButton != null)
            rightButton.onClick.AddListener(ShowNext);
        
        // Update MaskView with initial sprite
        UpdateMaskView();
    }

    public void ShowNext()
    {
        if (isAnimating || items.Length == 0) return;
        int nextIndex = (currentIndex + 1) % items.Length;
        StartCoroutine(DoSlide(currentIndex, nextIndex, true));
    }

    public void ShowPrevious()
    {
        if (isAnimating || items.Length == 0) return;
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = items.Length - 1;
        StartCoroutine(DoSlide(currentIndex, prevIndex, false));
    }

    IEnumerator DoSlide(int fromIndex, int toIndex, bool slideLeft)
    {
        isAnimating = true;
        
        RectTransform outItem = items[fromIndex];
        RectTransform inItem = items[toIndex];
        
        float dir = slideLeft ? 1f : -1f;
        
        outItem.anchoredPosition = Vector2.zero;
        inItem.anchoredPosition = new Vector2(dir * slideDistance, 0);
        inItem.gameObject.SetActive(true);
        
        float time = 0f;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / slideDuration);
            float smooth = Mathf.SmoothStep(0, 1, t);
            
            outItem.anchoredPosition = new Vector2(-dir * slideDistance * smooth, 0);
            inItem.anchoredPosition = new Vector2(dir * slideDistance * (1 - smooth), 0);
            
            yield return null;
        }
        
        outItem.anchoredPosition = Vector2.zero;
        inItem.anchoredPosition = Vector2.zero;
        outItem.gameObject.SetActive(false);
        
        currentIndex = toIndex;
        isAnimating = false;
        
        // Update MaskView
        UpdateMaskView();
        
        onItemChanged?.Invoke(currentIndex);
    }

    void UpdateMaskView()
    {
        if (maskViewSprites == null || currentIndex >= maskViewSprites.Length || maskViewSprites[currentIndex] == null)
            return;
        
        Sprite sprite = maskViewSprites[currentIndex];
        
        // Update main image
        if (maskViewImage != null)
        {
            maskViewImage.sprite = sprite;
            
            if (useNativeSize)
                maskViewImage.SetNativeSize();
            
            if (centerPosition)
            {
                var rect = maskViewImage.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = Vector2.zero;
            }
        }
        
        // Update mirrored image (same sprite, but the Image object has Scale X = -1)
        if (maskViewMirrorImage != null)
        {
            maskViewMirrorImage.sprite = sprite;
            
            if (useNativeSize)
                maskViewMirrorImage.SetNativeSize();
            
            if (centerPosition)
            {
                var rect = maskViewMirrorImage.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void SetIndex(int index)
    {
        if (index < 0 || index >= items.Length) return;
        
        // Hide current
        if (items[currentIndex] != null)
            items[currentIndex].gameObject.SetActive(false);
        
        currentIndex = index;
        
        // Show new
        if (items[currentIndex] != null)
            items[currentIndex].gameObject.SetActive(true);
        
        UpdateMaskView();
    }
}
