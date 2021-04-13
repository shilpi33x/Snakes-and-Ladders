using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dice : MonoBehaviour
{
    [SerializeField] int playerDiceRoll;
    [SerializeField] int enemyDiceRoll;

    [SerializeField] TextMeshProUGUI playerDiceText;
    [SerializeField] TextMeshProUGUI enemyDiceText;

    float waitTime;

    private void Start()
    {
        waitTime = FindObjectOfType<Token>().GetWaitTime();
    }

    public void CallRollDice()
    {
        StartCoroutine(RollDice());
    }

    private IEnumerator RollDice()
    {
        playerDiceRoll = Random.Range(1, 7);
        playerDiceText.text = playerDiceRoll.ToString();

        yield return new WaitForSeconds(waitTime);
        enemyDiceRoll = Random.Range(1, 7);
        enemyDiceText.text = enemyDiceRoll.ToString();
    }

    public int GetPlayerDiceRoll()
    {
        return playerDiceRoll;
    }

    public int GetEnemyDiceRoll()
    {
        return enemyDiceRoll;
    }

}
