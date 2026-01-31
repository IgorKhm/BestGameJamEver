using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Loads a sprite from a Resources folder and displays it.
/// Use this for MaskView features that need to show folder-based images.
/// </summary>
public class FolderImageLoader : MonoBehaviour
{
    [Header("Folder Settings")]
    [Tooltip("Folder name inside Resources (e.g., 'Faces')")]
    public string resourceFolderName = "Faces";
    
    [Header("Display")]
    [Tooltip("The Image component to display the sprite")]
    public Image targetImage;
    
    [Header("Options")]
    public bool useNativeSize = true;
    public bool centerPosition = true;
    
    private Sprite[] sprites;
    private int currentIndex = 0;

    public int CurrentIndex => currentIndex;
    public int Count => sprites != null ? sprites.Length : 0;

    void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        
        LoadSprites();
        ShowSprite(0);
    }

    void LoadSprites()
    {
        sprites = Resources.LoadAll<Sprite>(resourceFolderName);
        
        if (sprites == null || sprites.Length == 0)
            Debug.LogWarning($"[FolderImageLoader] No sprites in Resources/FaceParts/{resourceFolderName}/");
        else
            Debug.Log($"[FolderImageLoader] Loaded {sprites.Length} sprites from {resourceFolderName}");
    }

    public void ShowSprite(int index)
    {
        if (sprites == null || sprites.Length == 0) return;
        
        currentIndex = Mathf.Clamp(index, 0, sprites.Length - 1);
        
        if (targetImage != null)
        {
            targetImage.sprite = sprites[currentIndex];
            
            if (useNativeSize)
                targetImage.SetNativeSize();
            
            if (centerPosition)
            {
                var rect = targetImage.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void ShowNext()
    {
        int next = (currentIndex + 1) % sprites.Length;
        ShowSprite(next);
    }

    public void ShowPrevious()
    {
        int prev = currentIndex - 1;
        if (prev < 0) prev = sprites.Length - 1;
        ShowSprite(prev);
    }

    // Called by external scripts to sync with picker
    public void SetIndex(int index)
    {
        ShowSprite(index);
    }
}
