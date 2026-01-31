using UnityEngine;

public class BuilderPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject builderPanel;

    [SerializeField] private GameManager gameManager;
    
    [Header("UI")]
    public MaskBuilderToMaskDataBridge syncedDataBridge;
    
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        if (builderPanel != null)
        {
            _canvasGroup = builderPanel.GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = builderPanel.AddComponent<CanvasGroup>();
            }
        }
    }

    /// <summary>
    /// Shows the builder panel and enables interaction.
    /// </summary>
    public void EnableBuilderPanel()
    {
        if (builderPanel != null)
        {
            builderPanel.SetActive(true);
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.alpha = 1f;
        }
    }

    /// <summary>
    /// Hides the builder panel and disables interaction.
    /// </summary>
    public void DisableBuilderPanel()
    {
        syncedDataBridge.SyncMaskFromUI();
        if (builderPanel != null)
        {
            builderPanel.SetActive(false);
        }

        if (_canvasGroup != null)
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }
    }

}
