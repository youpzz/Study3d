using UnityEngine;

public class HeadbobSystem : MonoBehaviour
{
    [Range(0.001f, 0.1f)]
    public float Amount = 0.002f;

    [Range(1f, 30f)]
    public float bobFrequency = 10f;

    [Range(10f, 100f)]
    public float Smooth = 10f;


    Vector3 Startpos;

    private PlayerMovement playerMovement;





    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        Startpos = transform.localPosition;
    }

    void Update()
    {
        CheckForHeadbobTrigger();
        StopHeadbob();

    }

    void CheckForHeadbobTrigger()
    {
        if (!playerMovement.CanMove) return;
        float inputMagnitude = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).magnitude;
        if (inputMagnitude > 0) StartHeadBob();

    }

    void StopHeadbob()
    {
        if (transform.localPosition == Startpos) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, Startpos, 1 * Time.deltaTime);

    }

    private Vector3 StartHeadBob()
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * bobFrequency) * Amount * 1.4f, Smooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * bobFrequency / 2f) * Amount * 1.6f, Smooth * Time.deltaTime);
        transform.localPosition += pos;

        return pos;
    }
}
