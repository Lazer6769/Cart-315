using UnityEngine;

public class Pipes : MonoBehaviour
{
    private static float verticalMovementPauseUntil;

    private static bool VerticalMovementPaused => Time.time < verticalMovementPauseUntil;

    public Transform top;
    public Transform bottom;
    public float speed = 5f;
    public float gap = 3f;
    public float verticalMovementAmplitude = 0.5f;
    public float verticalMovementPeriod = 3f;
    public float verticalMovementPeriodVariation = 0.5f;
    public float minimumGap = 1.5f;
    public float gapMovementPeriod = 4f;
    public float gapMovementPeriodVariation = 1f;
    public float pipeStopChance = 0.1f;
    public float pipeStopDuration = 2f;

    private float leftEdge;
    private float initialY;
    private float movementPhase;
    private float movementPeriod;
    private float verticalMovementElapsed;
    private float gapMovementPhase;
    private float gapMovementPeriodWithVariation;
    private float gapMovementElapsed;
    private Vector3 topBaseLocalPosition;
    private Vector3 bottomBaseLocalPosition;

    private void Start()
    {
        leftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
        initialY = transform.position.y;
        movementPhase = Random.Range(0f, Mathf.PI * 2f);
        gapMovementPhase = Random.Range(0f, Mathf.PI * 2f);
        movementPeriod = verticalMovementPeriod > 0f
            ? Mathf.Max(0.01f, verticalMovementPeriod + Random.Range(-verticalMovementPeriodVariation, verticalMovementPeriodVariation))
            : 0f;
        gapMovementPeriodWithVariation = gapMovementPeriod > 0f
            ? Mathf.Max(0.01f, gapMovementPeriod + Random.Range(-gapMovementPeriodVariation, gapMovementPeriodVariation))
            : 0f;
        if (!VerticalMovementPaused && Random.value < pipeStopChance) {
            verticalMovementPauseUntil = Time.time + pipeStopDuration;
        }
        top.localPosition += Vector3.up * gap / 2;
        bottom.localPosition += Vector3.down * gap / 2;
        topBaseLocalPosition = top.localPosition;
        bottomBaseLocalPosition = bottom.localPosition;
    }

    private void Update()
    {
        Vector3 position = transform.position;
        position.x -= speed * 1.15f * Player.SpeedMultiplier * Time.deltaTime;

        if (movementPeriod > 0f) {
            if (!VerticalMovementPaused) {
                verticalMovementElapsed += Time.deltaTime;
            }

            float cycle = verticalMovementElapsed * (Mathf.PI * 2f / movementPeriod) + movementPhase;
            position.y = initialY + Mathf.Sin(cycle) * verticalMovementAmplitude;
        }

        float minimumGapSize = Mathf.Clamp(minimumGap, 0f, gap);
        float currentGap = gap;
        if (gapMovementPeriodWithVariation > 0f) {
            if (!VerticalMovementPaused) {
                gapMovementElapsed += Time.deltaTime;
            }

            float gapCycle = gapMovementElapsed * (Mathf.PI * 2f / gapMovementPeriodWithVariation) + gapMovementPhase;
            float gapProgress = (Mathf.Sin(gapCycle) + 1f) / 2f;
            currentGap = Mathf.Lerp(minimumGapSize, gap, gapProgress);
        }

        float gapOffset = (currentGap - gap) / 2f;
        top.localPosition = topBaseLocalPosition + Vector3.up * gapOffset;
        bottom.localPosition = bottomBaseLocalPosition + Vector3.down * gapOffset;

        transform.position = position;

        if (transform.position.x < leftEdge) {
            Destroy(gameObject);
        }
    }

}
