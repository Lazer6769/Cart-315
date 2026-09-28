using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Score score;
    public Ball ball;
    public GameStates gameState;

    private void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        ball.ResetBall();
        ball.AddStartingForce();
    }

    public void CourtTriggered(int courtId)
    {
        if (courtId != 0 && courtId != 1) return;

        int scoringPlayerId = courtId == 0 ? 1 : 0;
        score.IncreaseScore(scoringPlayerId);
        StartRound();
    }
}