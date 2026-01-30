using UnityEngine;

public class PlayerMaskVisual : MonoBehaviour
{
    public GameManager gameManager;
    public FaceMaskRenderer rendererTarget;

    private string _lastSignature;

    private void Awake()
    {
        if (rendererTarget == null) rendererTarget = GetComponentInChildren<FaceMaskRenderer>();
    }

    private void Update()
    {
        if (gameManager == null || rendererTarget == null) return;

        var mask = gameManager.GetPlayerMask();
        if (mask == null || mask.choices == null) return;

        // Cheap “did it change?” signature (good enough for jam)
        var sig = "";
        foreach (var c in mask.choices)
            sig += $"{c.feature?.featureId}:{c.variantIndex}|";

        if (sig == _lastSignature) return;
        _lastSignature = sig;

        rendererTarget.Apply(mask);
    }
}