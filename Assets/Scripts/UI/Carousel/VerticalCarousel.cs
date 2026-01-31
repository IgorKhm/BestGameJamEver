using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// Vertical carousel using Update-based animation (more reliable than coroutines).
/// </summary>
public class VerticalCarousel : MonoBehaviour
{
    [Header("Feature Pickers (top to bottom)")]
    public RectTransform[] pickers;
    
    [Header("Navigation")]
    public Button upButton;
    public Button downButton;
    
    [Header("Animation")]
    [Range(0.1f, 3f)]
    public float slideDuration = 0.5f;
    public float slideDistance = 150f;
    
    [Header("Title Display")]
    public Image facePartTitleBox;
    public Sprite[] titleSprites;
    
    [Header("Events")]
    public UnityEvent<int> onPickerChanged;
    
    private int currentIndex = 0;
    
    // Animation state
    private bool isAnimating = false;
    private float animTime = 0f;
    private RectTransform outPicker;
    private RectTransform inPicker;
    private float animDir = 1f;
    private int targetIndex = 0;

    public int CurrentIndex => currentIndex;

    void Start()
    {
        for (int i = 0; i < pickers.Length; i++)
        {
            if (pickers[i] != null)
                pickers[i].gameObject.SetActive(i == 0);
        }
        
        if (upButton != null)
            upButton.onClick.AddListener(ShowPrevious);
        
        if (downButton != null)
            downButton.onClick.AddListener(ShowNext);
        
        UpdateTitle(currentIndex);
    }
    
    void Update()
    {
        if (!isAnimating) return;
        
        // Safety: if pickers are null, force finish
        if (outPicker == null || inPicker == null)
        {
            Debug.LogWarning("[VerticalCarousel] Picker became null during animation - forcing finish");
            ForceFinish();
            return;
        }
        
        animTime += Time.deltaTime;
        float t = Mathf.Clamp01(animTime / slideDuration);
        float smooth = Mathf.SmoothStep(0, 1, t);
        
        // Safety: try-catch for position updates
        try
        {
            outPicker.anchoredPosition = new Vector2(0, animDir * slideDistance * smooth);
            inPicker.anchoredPosition = new Vector2(0, -animDir * slideDistance * (1 - smooth));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[VerticalCarousel] Animation error: {e.Message}");
            ForceFinish();
            return;
        }
        
        if (t >= 1f)
        {
            FinishAnimation();
        }
    }
    
    void ForceFinish()
    {
        isAnimating = false;
        currentIndex = targetIndex;
        UpdateTitle(currentIndex);
        
        // Hide all except current
        for (int i = 0; i < pickers.Length; i++)
        {
            if (pickers[i] != null)
                pickers[i].gameObject.SetActive(i == currentIndex);
        }
        
        outPicker = null;
        inPicker = null;
    }
    
    void UpdateTitle(int index)
    {
        if (facePartTitleBox == null) return;
        if (titleSprites != null && index >= 0 && index < titleSprites.Length && titleSprites[index] != null)
        {
            facePartTitleBox.sprite = titleSprites[index];
            facePartTitleBox.SetNativeSize();
        }
    }

    public void ShowNext()
    {
        if (isAnimating || pickers.Length == 0) return;
        int nextIndex = (currentIndex + 1) % pickers.Length;
        StartSlide(currentIndex, nextIndex, true);
    }

    public void ShowPrevious()
    {
        if (isAnimating || pickers.Length == 0) return;
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = pickers.Length - 1;
        StartSlide(currentIndex, prevIndex, false);
    }
    
    void StartSlide(int fromIndex, int toIndex, bool slideUp)
    {
        if (fromIndex < 0 || fromIndex >= pickers.Length) return;
        if (toIndex < 0 || toIndex >= pickers.Length) return;
        if (pickers[fromIndex] == null || pickers[toIndex] == null) return;
        
        outPicker = pickers[fromIndex];
        inPicker = pickers[toIndex];
        targetIndex = toIndex;
        animDir = slideUp ? 1f : -1f;
        animTime = 0f;
        
        // Reset positions
        outPicker.anchoredPosition = Vector2.zero;
        inPicker.anchoredPosition = new Vector2(0, -animDir * slideDistance);
        inPicker.gameObject.SetActive(true);
        
        Debug.Log($"[VerticalCarousel] Starting slide from {outPicker.name} to {inPicker.name}");
        isAnimating = true;
    }
    
    void FinishAnimation()
    {
        Debug.Log($"[VerticalCarousel] FinishAnimation - now at index {targetIndex}");
        isAnimating = false;
        
        if (outPicker != null)
        {
            outPicker.anchoredPosition = Vector2.zero;
            outPicker.gameObject.SetActive(false);
        }
        
        if (inPicker != null)
            inPicker.anchoredPosition = Vector2.zero;
        
        currentIndex = targetIndex;
        UpdateTitle(currentIndex);
        onPickerChanged?.Invoke(currentIndex);
        
        outPicker = null;
        inPicker = null;
    }
}
