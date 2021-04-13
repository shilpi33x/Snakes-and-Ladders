using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Token : MonoBehaviour
{
    //configuration parameters
    [Header("Values")]
    [SerializeField] float unit = 0.05f;
    [SerializeField] float waitTime = 0.5f;

    [SerializeField] int pos = 0;
    [SerializeField] bool hasStarted = false;

    [Header("Multimedia")]
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;
    [SerializeField] AudioClip playerMoveSound;
    [SerializeField] AudioClip enemyMoveSound;
    [SerializeField] GameObject particleVFX;

    [SerializeField] GameObject button;
    [SerializeField] GameObject canvas;

    //cached values
    Dice dice;

    private void Start()
    {
        dice = FindObjectOfType<Dice>();
    }

    private bool CheckPlayerOrEnemy()
    {
        if (tag == "Player")
            return true;
        else
            return false;
    }

    public void OnClickingButton()
    {
        if (CheckPlayerOrEnemy())
            CheckHasStarted();
        else
            StartCoroutine(DelayEnemyMove());
    }

    private IEnumerator DelayEnemyMove()
    {
        yield return new WaitForSeconds(waitTime);
        CheckHasStarted();
    }

    private void CheckHasStarted()
    {
        int diceRoll;
        if(CheckPlayerOrEnemy())
        {
            diceRoll = dice.GetPlayerDiceRoll();
        }
        else
        {
            diceRoll = dice.GetEnemyDiceRoll();
        }

        if (hasStarted)
            Move(diceRoll);
        else if (diceRoll == 1)
        {
            hasStarted = true;
            Move(diceRoll);
        }
//        else
//            IncreaseStartProbabilty();
    }

    private void Move(int diceRoll)
    {
        if ((pos + diceRoll) <= 100)
        {
            pos += diceRoll;
            if (pos < 100)
            {
                pos = CheckSnakeOrLadder(pos);
                MoveToPos(pos);
            }
            else
            {
                if (CheckPlayerOrEnemy())
                    OnWin();
                else
                    OnLose();
            }
        }
    }

    private void MoveToPos(int pos)
    {
        Camera gameCamera = Camera.main;
        var newXPos = CalculateXCoefficient(pos);
        var newYPos = ((pos - 1) / 10 * 2 * unit) + unit;
        var newZPos = 5;

        var newPos = new Vector3(newXPos, newYPos, newZPos);
        transform.position = gameCamera.ViewportToWorldPoint(newPos);

        PlayMoveSound();
    }

    private float CalculateXCoefficient(int pos)
    {
        if (((pos - 1) / 10) % 2 == 0)
            return (((pos - 1) % 10 * 2 * unit) + unit);            //left to right
        else
            return (((10 - (pos - 1) % 10) * 2 * unit) - unit);     //right to left
    }

    private int CheckSnakeOrLadder(int pos)
    {
        if (pos == 2)               //for ladders
            pos = 37;
        else if (pos == 5)
            pos = 14;
        else if (pos == 9)
            pos = 31;
        else if (pos == 28)
            pos = 84;
        else if (pos == 40)
            pos = 59;
        else if (pos == 51)
            pos = 67;
        else if (pos == 71)
            pos = 92;
        else if (pos == 78)
            pos = 97;
        else if (pos == 16)         //for snakes
            pos = 8;
        else if (pos == 53)
            pos = 29;
        else if (pos == 62)
            pos = 22;
        else if (pos == 64)
            pos = 60;
        else if (pos == 87)
            pos = 25;
        else if (pos == 93)
            pos = 89;
        else if (pos == 95)
            pos = 75;
        else if (pos == 99)
            pos = 80;
        return pos;
    }

    private void PlayMoveSound()
    {
        if (CheckPlayerOrEnemy())
            AudioSource.PlayClipAtPoint(playerMoveSound, Camera.main.transform.position);
        else
            AudioSource.PlayClipAtPoint(enemyMoveSound, Camera.main.transform.position);
    }

    private void OnWin()
    {
        TriggerParticlesVFX();
        if (CheckPlayerOrEnemy())
            InstantiatePlayAgainButton();
        OnGameOver();
    }

    private void OnLose()
    {
        if (!CheckPlayerOrEnemy())
            InstantiatePlayAgainButton();
        OnGameOver();
    }

    private void OnGameOver()
    {
        TriggerGameOverSFX();
        Destroy(gameObject);
    }

    private void TriggerParticlesVFX()
    {
        float particleXPos = 2.5f;
        float particleYPos = 6f;
        GameObject particle = Instantiate(particleVFX, new Vector2(particleXPos, particleYPos), Quaternion.identity);
        Destroy(particle, 20f);
    }

    private void TriggerGameOverSFX()
    {
        if(CheckPlayerOrEnemy())
            AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
        else
            AudioSource.PlayClipAtPoint(loseSound, Camera.main.transform.position);
    }

    private void InstantiatePlayAgainButton()
    {
        GameObject playAgainCanvas = Instantiate(canvas);
        GameObject playAgainButton = Instantiate(button);
        playAgainButton.transform.SetParent(playAgainCanvas.transform, false);
    }

    public float GetWaitTime()
    {
        return waitTime;
    }
}