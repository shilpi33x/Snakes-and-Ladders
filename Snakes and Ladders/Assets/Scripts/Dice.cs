using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dice : MonoBehaviour
{
    //config parameters
    [SerializeField] int playerDiceRoll;
    [SerializeField] TextMeshProUGUI playerDiceText;

    [SerializeField] int enemyDiceRoll;
    [SerializeField] TextMeshProUGUI enemyDiceText;

    //cached reference
    float waitTime;

    private void Start()
    {
        waitTime = FindObjectOfType<Token>().GetWaitTime();
    }

    //Function gets called on button press - start coroutine to roll dice
    public void CallRollDice()
    {
        StartCoroutine(RollDice());
    }

    //Coroutine to assign dice roll as random between 1-6 and display value of roll on screen
    private IEnumerator RollDice()
    {
        //Perform dice roll operation for Player token
        playerDiceRoll = Random.Range(1, 7);
        playerDiceText.text = playerDiceRoll.ToString();

        //Perform dice roll operation for Enemy token after delay of waitTime
        yield return new WaitForSeconds(waitTime);
        enemyDiceRoll = Random.Range(1, 7);
        enemyDiceText.text = enemyDiceRoll.ToString();
    }


    //Send Player dice roll to another script
    public int GetPlayerDiceRoll()
    {
        return playerDiceRoll;
    }

    //Send Enemy dice roll to another script
    public int GetEnemyDiceRoll()
    {
        return enemyDiceRoll;
    }

}
