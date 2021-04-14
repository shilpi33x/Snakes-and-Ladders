using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    //config parameters
    [SerializeField] int diceRoll;
    [SerializeField] int enemyDiceRoll;

    float probabilityOfOne;
    float probabilityOfOneForPlayer = 30;
    float probabilityOfOneForEnemy = 30;

    //Function to assign value of dice roll
    public int RollDice(bool playerOrEnemy)
    {
        if (playerOrEnemy)
            probabilityOfOne = probabilityOfOneForPlayer;
        else
            probabilityOfOne = probabilityOfOneForEnemy;

        //Probability of rolling a one is greater until Token gets activated
        int probability = Random.Range(1, 101);
        if (probability < probabilityOfOne)
            diceRoll = 1;
        else
            diceRoll = Random.Range(2, 7);
        return diceRoll;
    }

    //Functions to reset probability of rolling one - set as equally likely as other values of dice
    public void ResetProbabilityOfOneForPlayer()
    {
        probabilityOfOneForPlayer = 100 / 6;
    }
    public void ResetProbabilityOfOneForEnemy()
    {
        probabilityOfOneForEnemy = 100 / 6;
    }
}
