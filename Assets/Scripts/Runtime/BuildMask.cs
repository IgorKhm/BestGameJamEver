using UnityEngine;
using UnityEngine.UI;

public class BuildMask : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        InputSystem.Instance.OnBuildMaskPressed += () => button.onClick.Invoke();   
    }
}
