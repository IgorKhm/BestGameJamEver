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
    
    [Header("Events")]
    public UnityEvent<int> onItemChanged;
    
    private int currentIndex = 0;
    private bool isAnimating = false;

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
        
        onItemChanged?.Invoke(currentIndex);
    }
}
