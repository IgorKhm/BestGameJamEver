using System;
using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public static InputSystem Instance { get; private set; }

    public KeyCode AttemptEntry = KeyCode.A;
    public KeyCode BuildMask = KeyCode.B;
    public KeyCode MainMenu = KeyCode.R;
    public KeyCode CloseBuildWindow = KeyCode.Escape;

    public Action OnAttemptButtonPressed;
    public Action OnBuildMaskPressed;
    public Action OnRestartPressed;
    public Action OnEscapePressed;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(AttemptEntry))
        {
            OnAttemptButtonPressed?.Invoke();
        }

        if (Input.GetKeyDown(BuildMask))
        {
            OnBuildMaskPressed?.Invoke();
        }

        if (Input.GetKeyDown(MainMenu))
        {
            OnRestartPressed?.Invoke();
        }

        if (Input.GetKeyDown(CloseBuildWindow))
        {
            OnEscapePressed?.Invoke();
        }
    }
}