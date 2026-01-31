using TMPro;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public MaskData mask;
    public bool isAccepted = true;

    private int _dir = 1; // +1 right, -1 left
    private bool _processedDoor = false;
    private float _despawnX;
    private float _rejectedSpeedMult;

    [SerializeField] private TMP_Text label; // optional, you can remove later
    [SerializeField] private NpcMaskRenderer maskRenderer;
    [SerializeField] private FaceMaskRenderer faceRenderer;

    private void Awake()
    {
        if (label == null) label = GetComponentInChildren<TMP_Text>();
        if (maskRenderer == null) maskRenderer = GetComponentInChildren<NpcMaskRenderer>();
        if (faceRenderer == null) faceRenderer = GetComponentInChildren<FaceMaskRenderer>();
    }

    public void Init(float speed, MaskData m, bool accepted,
        string debugText, float despawnX, float rejectedSpeedMult)
    {
        moveSpeed = speed;
        mask = m;
        isAccepted = accepted;

        _dir = 1;
        _processedDoor = false;
        _despawnX = despawnX;

        // if you want rejected to walk faster/slower after door:
        _rejectedSpeedMult = rejectedSpeedMult;

        // apply visuals here (you already do this)
        if (faceRenderer != null) faceRenderer.Apply(m);
        if (label != null) label.text = debugText;
    }

    public bool TryMarkDoorProcessed()
    {
        if (_processedDoor) return false;
        _processedDoor = true;
        return true;
    }

    public void RejectAndExitLeft()
    {
        _dir = -1;
        moveSpeed *= _rejectedSpeedMult;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    private void Update()
    {
        transform.position += Vector3.right * (_dir * moveSpeed * Time.deltaTime);

        // despawn when exiting left
        if (_dir < 0 && transform.position.x < _despawnX)
            Destroy(gameObject);
    }
}