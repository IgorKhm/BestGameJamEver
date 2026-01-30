using TMPro;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public MaskData mask;
    public bool isAccepted = true;

    [SerializeField] private TMP_Text label; // optional, you can remove later
    [SerializeField] private NpcMaskRenderer maskRenderer;
    [SerializeField] private FaceMaskRenderer faceRenderer;
    private void Awake()
    {
        if (label == null) label = GetComponentInChildren<TMP_Text>();
        if (maskRenderer == null) maskRenderer = GetComponentInChildren<NpcMaskRenderer>();
        if (faceRenderer == null) faceRenderer = GetComponentInChildren<FaceMaskRenderer>();
        
    }

    public void Init(float speed, MaskData m, bool accepted, string debugText)
    {
        moveSpeed = speed;
        mask = m;
        isAccepted = accepted;

        // TEXT (optional) — you can disable this later
        if (label != null) label.text = debugText;

        // GRAPHICS
        // if (maskRenderer != null) maskRenderer.Apply(m);
        
        if (faceRenderer != null) faceRenderer.Apply(m);
        
    }

    private void Update()
    {
        transform.position += Vector3.right * (moveSpeed * Time.deltaTime);
    }
}