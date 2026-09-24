using System.Collections;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ball : MonoBehaviour
{
    private Rigidbody2D _rigidBody;

    public float speed = 100.0f;
    public float minimumRotationDelay = 3.0f;
    public float maximumRotationDelay = 7.0f;
    public float rotationSpeed = 360.0f;
    public float minimumSize = 0.75f;
    public float maximumSize = 1.25f;
    public float minimumSizeChangeDelay = 2.0f;
    public float maximumSizeChangeDelay = 5.0f;
    public float sizeChangeDuration = 0.5f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        StartCoroutine(RandomlyRotateBall());
        StartCoroutine(RandomlyChangeSize());
    }

    public void ResetBall()
    {
        _rigidBody.linearVelocity = Vector2.zero;
        _rigidBody.angularVelocity = 0;
        transform.position = Vector3.zero;
    }

    public void AddStartingForce()
    {
        float x = Random.value < 0.5f ? -1.0f : 1.0f;
        float y = (Random.value < 0.5f ? -1.0f : 1.0f) * Random.Range(0.5f, 0.9f);

        Vector2 direction = new Vector2(x, y);

        _rigidBody.AddForce(direction * speed);
    }

    private IEnumerator RandomlyRotateBall()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minimumRotationDelay, maximumRotationDelay));
            yield return RotateBall360();
        }
    }

    private IEnumerator RotateBall360()
    {
        float rotated = 0.0f;
        float degreesPerSecond = Mathf.Abs(rotationSpeed);
        float direction = Mathf.Sign(rotationSpeed);

        while (rotated < 360.0f)
        {
            float frameRotation = Mathf.Min(degreesPerSecond * Time.deltaTime, 360.0f - rotated);
            transform.Rotate(0.0f, 0.0f, frameRotation * direction);
            rotated += frameRotation;
            yield return null;
        }
    }

    private IEnumerator RandomlyChangeSize()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minimumSizeChangeDelay, maximumSizeChangeDelay));

            float targetSize = Random.Range(minimumSize, maximumSize);
            Vector3 startingScale = transform.localScale;
            Vector3 targetScale = new Vector3(targetSize, targetSize, startingScale.z);
            float elapsed = 0.0f;

            while (elapsed < sizeChangeDuration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsed / sizeChangeDuration);
                transform.localScale = Vector3.Lerp(startingScale, targetScale, progress);
                yield return null;
            }

            transform.localScale = targetScale;
        }
    }
}