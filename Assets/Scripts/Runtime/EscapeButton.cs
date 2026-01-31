using UnityEngine;
using UnityEngine.UI;

public class EscapeButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        InputSystem.Instance.OnEscapePressed+= () => button.onClick.Invoke();
    }
}
