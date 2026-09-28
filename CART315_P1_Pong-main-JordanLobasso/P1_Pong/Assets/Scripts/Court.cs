using UnityEngine;

public class Court : MonoBehaviour
{
    public GameManager gameManager;
    public int courtId = 0;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = FindFirstObjectByType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ScoreBall(collision.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ScoreBall(collision.gameObject);
    }

    private void ScoreBall(GameObject objectThatEntered)
    {
        Ball ball = objectThatEntered.GetComponent<Ball>();
        if (ball == null) return;

        if (gameManager != null)
            gameManager.CourtTriggered(courtId);
    }
}