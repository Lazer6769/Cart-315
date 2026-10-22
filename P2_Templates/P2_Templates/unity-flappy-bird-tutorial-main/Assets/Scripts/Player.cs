using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static float SpeedMultiplier => speedBoostTimer > 0f ? activeBoostMultiplier : 1f;
    private static float speedBoostTimer;
    private static float activeBoostMultiplier = 1.25f;

    [SerializeField] private Text boostText;
    public Sprite[] sprites;
    public float strength = 5f;
    public float gravity = -9.81f;
    public float tilt = 5f;
    public float speedBoostDuration = 0.5f;
    public float spinDuration = 0.5f;

    private SpriteRenderer spriteRenderer;
    private Vector3 direction;
    private int spriteIndex;
    private float spinElapsed;
    private bool isSpinning;
    private int unbonusedPipePasses;
    private float boostCooldownTimer;
    private float spinCooldownTimer;
    private float slowChanceTimer;
    private float slowEffectTimer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    private void OnEnable()
    {
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
        direction = Vector3.zero;
        speedBoostTimer = 0f;
        activeBoostMultiplier = 1.25f;
        spinElapsed = 0f;
        isSpinning = false;
        unbonusedPipePasses = 0;
        boostCooldownTimer = 0f;
        spinCooldownTimer = 0f;
        slowChanceTimer = 5f;
        slowEffectTimer = 0f;
        UpdateBoostText();
    }

    private void Update()
    {
        slowChanceTimer -= Time.deltaTime;
        if (slowChanceTimer <= 0f) {
            slowChanceTimer = 5f;
            if (Random.value < 0.1f) {
                slowEffectTimer = 1f;
            }
        }

        float movementMultiplier = slowEffectTimer > 0f ? 0.5f : 1f;
        slowEffectTimer = Mathf.Max(0f, slowEffectTimer - Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.R) && spinDuration > 0f && spinCooldownTimer <= 0f) {
            spinElapsed = 0f;
            isSpinning = true;
            spinCooldownTimer = 1f;
        }

        if (isSpinning) {
            spinElapsed += Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Z)) {
            if (boostCooldownTimer <= 0f) {
                speedBoostTimer = speedBoostDuration;
                activeBoostMultiplier = Random.value < 0.01f ? 2.5f : 1.25f;
                boostCooldownTimer = 2.5f;
            }
        } else {
            speedBoostTimer = Mathf.Max(0f, speedBoostTimer - Time.deltaTime);
        }
        boostCooldownTimer = Mathf.Max(0f, boostCooldownTimer - Time.deltaTime);
        spinCooldownTimer = Mathf.Max(0f, spinCooldownTimer - Time.deltaTime);
        UpdateBoostText();

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
            direction = Vector3.up * strength;
        }

        // Apply gravity and update the position
        float movementDeltaTime = Time.deltaTime * movementMultiplier;
        direction.y += gravity * movementDeltaTime;
        transform.position += direction * movementDeltaTime;

        // Tilt the bird based on the direction
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;

        if (isSpinning) {
            float spinProgress = Mathf.Clamp01(spinElapsed / spinDuration);
            rotation.z += 360f * spinProgress;
            isSpinning = spinProgress < 1f;

            if (!isSpinning && unbonusedPipePasses > 0) {
                unbonusedPipePasses--;
                GameManager.Instance.IncreaseScore(1);
            }
        }

        transform.eulerAngles = rotation;
    }

    private void UpdateBoostText()
    {
        if (boostText == null) {
            return;
        }

        string boostStatus = boostCooldownTimer > 0f
            ? $"{boostCooldownTimer:0.0}s"
            : "READY";
        string spinStatus = spinCooldownTimer > 0f
            ? $"{spinCooldownTimer:0.0}s"
            : "READY";
        boostText.text = $"Z BOOST: {boostStatus}\nR SPIN: {spinStatus}";
    }

    private void AnimateSprite()
    {
        spriteIndex++;

        if (spriteIndex >= sprites.Length) {
            spriteIndex = 0;
        }

        if (spriteIndex < sprites.Length && spriteIndex >= 0) {
            spriteRenderer.sprite = sprites[spriteIndex];
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Obstacle")) {
            GameManager.Instance.GameOver();
        } else if (other.gameObject.CompareTag("Scoring")) {
            GameManager.Instance.IncreaseScore();
            unbonusedPipePasses++;
        }
    }

}
