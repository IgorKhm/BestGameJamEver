using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Carousel for MaskView with delayed display change.
/// </summary>
public class MaskViewerCarousel : MonoBehaviour
{
    [Header("Items to cycle through")]
    public GameObject[] items;
    
    [Header("Navigation Buttons")]
    public Button leftButton;
    public Button rightButton;
    
    [Header("Timing")]
    [Tooltip("Delay before showing next item (match with animation duration)")]
    [Range(0f, 3f)]
    public float displayDelay = 0.6f;
    
    private int currentIndex = 0;
    private bool isChanging = false;

    public int CurrentIndex => currentIndex;

    void Start()
    {
        if (leftButton != null)
            leftButton.onClick.AddListener(ShowNext);
        
        if (rightButton != null)
            rightButton.onClick.AddListener(ShowPrevious);
        
        ShowItemInstant(0);
    }

    public void ShowNext()
    {
        if (isChanging || items.Length == 0) return;
        int nextIndex = (currentIndex + 1) % items.Length;
        StartCoroutine(DelayedShow(nextIndex));
    }

    public void ShowPrevious()
    {
        if (isChanging || items.Length == 0) return;
        int prevIndex = currentIndex - 1;
        if (prevIndex < 0) prevIndex = items.Length - 1;
        StartCoroutine(DelayedShow(prevIndex));
    }

    IEnumerator DelayedShow(int index)
    {
        isChanging = true;
        
        // Wait for the delay (matches animation time)
        yield return new WaitForSeconds(displayDelay);
        
        // Now show the new item
        ShowItemInstant(index);
        
        isChanging = false;
    }

    void ShowItemInstant(int index)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null)
                items[i].SetActive(i == index);
        }
        currentIndex = index;
    }
}
