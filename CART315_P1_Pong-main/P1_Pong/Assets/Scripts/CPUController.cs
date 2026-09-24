using System.Collections;
using UnityEngine;

public class CPUController : MonoBehaviour
{
    public Ball ball;
    public Paddle paddle;
    public float minimumDecisionTime = 0.4f;
    public float maximumDecisionTime = 1.0f;
    public float trackingChance = 0.5f;
    public float minimumY = -3.5f;
    public float maximumY = 3.5f;
    public float minimumRotationDelay = 3.0f;
    public float maximumRotationDelay = 7.0f;
    public float rotationSpeed = 180.0f;

    private float _decisionTimer;
    private float _targetY;

    private void Start()
    {
        ChooseTarget();
        StartCoroutine(RandomlyRotatePaddle());
    }

    private void Update()
    {
        _decisionTimer -= Time.deltaTime;

        if (_decisionTimer <= 0.0f)
            ChooseTarget();

        float distanceToTarget = _targetY - paddle.transform.position.y;

        if (Mathf.Abs(distanceToTarget) < 0.15f)
            paddle.direction = Vector2.zero;
        else
            paddle.direction = distanceToTarget > 0.0f ? Vector2.up : Vector2.down;
    }

    private void ChooseTarget()
    {
        _decisionTimer = Random.Range(minimumDecisionTime, maximumDecisionTime);

        if (Random.value < trackingChance)
            _targetY = ball.transform.position.y + Random.Range(-1.5f, 1.5f);
        else
            _targetY = Random.Range(minimumY, maximumY);

        _targetY = Mathf.Clamp(_targetY, minimumY, maximumY);
    }

    private IEnumerator RandomlyRotatePaddle()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minimumRotationDelay, maximumRotationDelay));
            yield return RotatePaddle180();
        }
    }

    private IEnumerator RotatePaddle180()
    {
        float rotated = 0.0f;
        float degreesPerSecond = Mathf.Abs(rotationSpeed);
        float direction = Mathf.Sign(rotationSpeed);

        while (rotated < 180.0f)
        {
            float frameRotation = Mathf.Min(degreesPerSecond * Time.deltaTime, 180.0f - rotated);
            paddle.transform.Rotate(0.0f, 0.0f, frameRotation * direction);
            rotated += frameRotation;
            yield return null;
        }
    }
}