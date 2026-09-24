using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector2 _direction;
    private bool _isRotating;

    public Paddle paddle;
    public float rotationSpeed = 180.0f;
    public float minimumRotationDelay = 3.0f;
    public float maximumRotationDelay = 7.0f;

    private void Start()
    {
        StartCoroutine(RandomlyRotatePaddle());
    }

    // Update is called once per frame
    private void Update()
    {
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
            yield return new WaitForSeconds(Random.Range(minimumRotationDelay, maximumRotationDelay));

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
            float frameRotation = Mathf.Min(degreesPerSecond * Time.deltaTime, 180.0f - rotated);
            paddle.transform.Rotate(0.0f, 0.0f, frameRotation * direction);
            rotated += frameRotation;
            yield return null;
        }

        _isRotating = false;
    }
}