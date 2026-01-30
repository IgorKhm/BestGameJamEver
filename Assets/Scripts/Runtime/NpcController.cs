using TMPro;
using UnityEngine;

public class NpcController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public MaskData mask;
    public bool isAccepted = true;

    [SerializeField] private TMP_Text label;

    public void Init(float speed, MaskData m, bool accepted, string debugText)
    {
        moveSpeed = speed;
        mask = m;
        isAccepted = accepted;

        if (label != null) label.text = debugText;
    }

    private void Update()
    {
        transform.position += Vector3.right * (moveSpeed * Time.deltaTime);
    }
}