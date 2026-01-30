using UnityEngine;

public class SquashStretchBob : MonoBehaviour
{
    public float frequency = 2.5f;
    public float squashAmount = 0.12f; // 0.05–0.2
    public bool randomizePhase = true;

    private Vector3 _baseScale;
    private float _phase;

    private void Awake()
    {
        _baseScale = transform.localScale;
        _phase = randomizePhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    private void Update()
    {
        float t = Time.time * frequency * (Mathf.PI * 2f) + _phase;

        // 0..1 “step” curve (more like feet bounce)
        float step = Mathf.Abs(Mathf.Sin(t));

        // When step is high: squash (wider, shorter). When low: stretch (taller, thinner)
        float squash = 1f + step * squashAmount;
        float stretch = 1f - step * squashAmount;

        transform.localScale = new Vector3(_baseScale.x * squash, _baseScale.y * stretch, _baseScale.z);
    }
}