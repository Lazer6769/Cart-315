using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _direction;
    private bool _isRotating;
    private bool _rotationPaused;
    private bool _isReturningToNormal;
    private Quaternion _normalRotation;

    public Paddle paddle;
    public float rotationSpeed = 360.0f;
    public float minimumRotationDelay = 3.0f;
    public float maximumRotationDelay = 7.0f;
    public float winningRotationDelayMultiplier = 0.5f;

    private Score _score;

    private void Start()
    {
        _score = FindFirstObjectByType<Score>();
        _normalRotation = paddle.transform.localRotation;
        StartCoroutine(RandomlyRotatePaddle());
    }

    // Update is called once per frame
    private void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            _rotationPaused = !_rotationPaused;

            if (!_rotationPaused && !_isReturningToNormal)
                StartCoroutine(RotatePaddleToNormal());
        }

        _direction = Vector2.zero;

        if (Keyboard.current.sKey.isPressed)
            _direction = Vector2.up;
        else if (Keyboard.current.wKey.isPressed)
            _direction = Vector2.down;

        paddle.direction = _direction;
    }

    private IEnumerator RandomlyRotatePaddle()
    {
        while (true)
        {
            while (_rotationPaused || _isReturningToNormal)
                yield return null;

            float delay = Random.Range(minimumRotationDelay, maximumRotationDelay);
            if (_score != null && _score.scorePlayerOne > _score.scorePlayerTwo)
                delay *= winningRotationDelayMultiplier;

            yield return new WaitForSeconds(delay);

            if (_rotationPaused || _isReturningToNormal)
                continue;

            if (!_isRotating)
                yield return RotatePaddle180();
        }
    }

    private IEnumerator RotatePaddle180()
    {
        _isRotating = true;
        float rotated = 0.0f;
        float direction = Mathf.Sign(rotationSpeed);
        float degreesPerSecond = Mathf.Abs(rotationSpeed);

        while (rotated < 180.0f)
        {
            if (_rotationPaused)
                break;

            float frameRotation = Mathf.Min(degreesPerSecond * Time.deltaTime, 180.0f - rotated);
            paddle.transform.Rotate(0.0f, 0.0f, frameRotation * direction);
            rotated += frameRotation;
            yield return null;
        }

        _isRotating = false;
    }

    private IEnumerator RotatePaddleToNormal()
    {
        _isReturningToNormal = true;

        while (Quaternion.Angle(paddle.transform.localRotation, _normalRotation) > 0.01f)
        {
            while (_rotationPaused)
                yield return null;

            paddle.transform.localRotation = Quaternion.RotateTowards(
                paddle.transform.localRotation,
                _normalRotation,
                Mathf.Abs(rotationSpeed) * Time.deltaTime);
            yield return null;
        }

        paddle.transform.localRotation = _normalRotation;
        _isReturningToNormal = false;
    }
}