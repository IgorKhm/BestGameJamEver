using UnityEngine;
using UnityEngine.UI;

public class AttemptEntry : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        InputSystem.Instance.OnAttemptButtonPressed += () => button.onClick.Invoke();   
    }
}
