using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// Vertical carousel to switch between feature pickers (nose, mouth, eyes, etc.)
/// Each "item" is a horizontal picker panel.
/// </summary>
public class VerticalCarousel : MonoBehaviour
{
    [Header("Feature Pickers (top to bottom)")]
    [Tooltip("Each picker panel (contains its own HorizontalCarousel)")]
    public RectTransform[] pickers;
    
    [Header("Navigation")]
    public Button upButton;
    public Button downButton;
    
    [Header("Animation")]
    [Range(0.1f, 3f)]
    public float slideDuration = 0.5f;
    public float slideDistance = 150f;
    
    [Header("Events")]
    public UnityEvent<int> onPickerChanged;
    
    private int currentIndex = 0;
    private bool isAnimating = false;

    public int CurrentIndex => currentIndex;

    void Start()
    {
        // Show only first picker
        for (int i = 0; i < pickers.Length; i++)
        {
            if (pickers[i] != null)
                pickers[i].gameObject.SetActive(i == 0);
        }
        
        if (upButton != null)
            upButton.onClick.AddListener(ShowPrevious);
        
        if (downButton != null)
            downButton.onClick.AddListener(ShowNext);
    }

    public void ShowNext()
    {
        Debug.Log($"[VerticalCarousel] ShowNext called. isAnimating={isAnimating}, count={pickers.Length}, current={currentIndex}");
        if (isAnimating || pickers.Length == 0) return;
        int nextIndex = (currentIndex + 1) % pickers.Length;
        StartCoroutine(DoSlide(currentIndex, nextIndex, true)); // Slide up
    }

    public void ShowPrevious()
    {
        Debug.Log($"[VerticalCarousel] ShowPrevious called. isAnimating={isAnimating}, count={pickers.Length}, current={currentIndex}");
        if (isAnimating || pickers.Length == 0) return;
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = pickers.Length - 1;
        StartCoroutine(DoSlide(currentIndex, prevIndex, false)); // Slide down
    }

    IEnumerator DoSlide(int fromIndex, int toIndex, bool slideUp)
    {
        isAnimating = true;
        
        RectTransform outPicker = pickers[fromIndex];
        RectTransform inPicker = pickers[toIndex];
        
        // Direction: slideUp means current goes up (positive Y), new comes from below (negative Y)
        float dir = slideUp ? 1f : -1f;
        
        // Starting positions
        outPicker.anchoredPosition = Vector2.zero;
        inPicker.anchoredPosition = new Vector2(0, -dir * slideDistance);
        inPicker.gameObject.SetActive(true);
        
        // Animate
        float time = 0f;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / slideDuration);
            float smooth = Mathf.SmoothStep(0, 1, t);
            
            // Outgoing moves away (up or down)
            outPicker.anchoredPosition = new Vector2(0, dir * slideDistance * smooth);
            // Incoming moves to center
            inPicker.anchoredPosition = new Vector2(0, -dir * slideDistance * (1 - smooth));
            
            yield return null;
        }
        
        // Finalize
        outPicker.anchoredPosition = Vector2.zero;
        inPicker.anchoredPosition = Vector2.zero;
        outPicker.gameObject.SetActive(false);
        
        currentIndex = toIndex;
        isAnimating = false;
        
        onPickerChanged?.Invoke(currentIndex);
    }
}
