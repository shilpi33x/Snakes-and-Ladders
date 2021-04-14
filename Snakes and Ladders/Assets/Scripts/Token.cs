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
    [SerializeField] bool isActive = false;

    [Header("Multimedia")]
    [SerializeField] TextMeshProUGUI tokenDiceText;

    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip loseSound;
    [SerializeField] AudioClip playerMoveSound;
    [SerializeField] AudioClip enemyMoveSound;
    [SerializeField] GameObject particleVFX;

    [Header("Game Objects")]
    [SerializeField] GameObject button;
    [SerializeField] GameObject canvas;

    [SerializeField] Token otherToken;

    //cached reference
    Dice dice;

    private void Start()
    {
        //Cache refence to Dice script
        dice = FindObjectOfType<Dice>();
    }

    //Check if script is attached to Player or Enemy token
    private bool CheckPlayerOrEnemy()
    {
        if (tag == "Player")
            return true;
        else
            return false;
    }

    //Begin Token movement
    public void OnDiceRoll()
    {
        if (CheckPlayerOrEnemy())
            CheckIfActive();
        else
            StartCoroutine(DelayEnemyMove());
    }

    //Begin Enemy token movement after delay of waitTime - ensures that Player and Enemy do not move at same time
    private IEnumerator DelayEnemyMove()
    {
        yield return new WaitForSeconds(waitTime);
        CheckIfActive();
    }

    //Funtion to implement Game Rule: Token(either Player or Enemy) will remain inactive until the first time a one is rolled
    private void CheckIfActive()
    {
        //Fetch and display dice roll from Dice script
        int diceRoll;
        diceRoll = dice.RollDice(CheckPlayerOrEnemy());
        tokenDiceText.text = diceRoll.ToString();

        //Check if game has started for Token i.e. if Token is active; move only if active
        if (isActive)
            CheckMovement(diceRoll);
        else if (diceRoll == 1)
        {
            StartToken(diceRoll);
        }
    }

    //Set Token as active
    private void StartToken(int diceRoll)
    {
        isActive = true;

        //Increase opacity of Token to 100% when active
        Color opacity = gameObject.GetComponent<SpriteRenderer>().color;
        opacity.a = 255f;
        gameObject.GetComponent<SpriteRenderer>().color = opacity;

        if (CheckPlayerOrEnemy())
            dice.ResetProbabilityOfOneForPlayer();
        else
            dice.ResetProbabilityOfOneForEnemy();

        CheckMovement(diceRoll);
    }

    //Game Rule: Movement possible only if Token doesn't exceed 100 - has to fall exactly on 100 to win
    //Check if Token can move, and to which position(destination square's no.); implement Game Rule
    private void CheckMovement(int diceRoll)
    {
        if ((pos + diceRoll) <= 100)
        {
            pos += diceRoll;

            if (pos < 100)
            {
                //Token has not reached 100 - normal movement
                pos = CheckSnakeOrLadder(pos);
                MoveToPos(pos);
            }
            else
            {
                //Token reaches position 100
                if (CheckPlayerOrEnemy())
                    OnWin();
                else
                    OnLose();
            }
        }
    }

    //Calculate and move Token to position of destination square w.r.t main camera
    //DISCLAIMER: This approach only works with square cameras. Board has to be same size as Main Camera; used second camera for UI
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

    //Check if Token will land on a Snake or a Ladder; update destination position to target position of Snake or Ladder
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

    //Play audio every time the Token moves
    private void PlayMoveSound()
    {
        if (CheckPlayerOrEnemy())
            AudioSource.PlayClipAtPoint(playerMoveSound, Camera.main.transform.position);
        else
            AudioSource.PlayClipAtPoint(enemyMoveSound, Camera.main.transform.position);
    }

    //Play FX when Player reaches 100 first
    private void OnWin()
    {
        TriggerParticlesVFX();
        TriggerWinSFX();
        if (CheckPlayerOrEnemy())
            InstantiatePlayAgainButton();
        OnGameOver();
    }

    //Play FX when Enemy reaches 100 first
    private void OnLose()
    {
        TriggerLoseSFX();
        if (!CheckPlayerOrEnemy())
            InstantiatePlayAgainButton();
        OnGameOver();
    }

    private void TriggerParticlesVFX()
    {
        float particleXPos = 2.5f;
        float particleYPos = 6f;
        GameObject particle = Instantiate(particleVFX, new Vector2(particleXPos, particleYPos), Quaternion.identity);
        Destroy(particle, 20f);
    }

    private void TriggerWinSFX()
    {
        AudioSource.PlayClipAtPoint(winSound, Camera.main.transform.position);
    }

    private void TriggerLoseSFX()
    {
        AudioSource.PlayClipAtPoint(loseSound, Camera.main.transform.position);
    }

    //Play irrespective of which Token wins
    private void OnGameOver()
    {
        Destroy(gameObject);
        Destroy(otherToken.gameObject);
    }

    //Instantiate button to reload scene - play again
    private void InstantiatePlayAgainButton()
    {
        GameObject playAgainCanvas = Instantiate(canvas);
        GameObject playAgainButton = Instantiate(button);
        playAgainButton.transform.SetParent(playAgainCanvas.transform, false);
    }
}