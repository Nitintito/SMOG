using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Range(0, 100)]
    [SerializeField] private int playerHealth;
    [SerializeField] private int maxLife;
    private bool coroutineRunning;
    private int maxLifeStartvalue = 100;
    private int outOfTimeInt = 0;
    private int outOfLifeInt = 0;
    private int negativeCheck = 0;
    private int optionalValue = 0;

    /*
     Class for managing life, damage health and healing.

     A set of methods for managing this, lots of room for exploration and adding methods that fit the game.
     GetHealth is just a way to return health from other classes in the game.
    */
    public int GetHealth
    {
        get
        {
            return playerHealth;
        }
    }

    private void Start()
    {
        playerHealth = maxLife;
        maxLife = maxLifeStartvalue;
    }

    /*
     A method for changing the maxlife of the player, can be used for powerups or something similar.
     int LifeToAddToMaxLife is Added to the maximum.
    */
    public void SetMaxLife(int LifeToAddToMaxLife)
    {
        maxLife += LifeToAddToMaxLife;
    }


    /*
     Method for changing the maxlife of the player a overload version of the one 
     above where you can set duration in seconds with int DurationInSeconds so that it's not a permanent buff.
     Starts a Coroutine that waits for that amount of seconds before it goes back to original maxhealth.
     */
    public void SetMaxLife(int LifeToAddToMaxLife, int DurationInSeconds)
    {
        if (!coroutineRunning)
        {
            coroutineRunning = true;
            StartCoroutine(SetMaxLifeCoroutine(LifeToAddToMaxLife, DurationInSeconds));
        }
    }

   

    /*
     Coroutine belonging to the method above, takes in same variables as they are passed to this coroutine instead. 
         */

    private IEnumerator SetMaxLifeCoroutine(int LifeToAddToMaxLife, int DurationInSeconds)
    {
        int maxLifeTemp = maxLife;
        maxLife += LifeToAddToMaxLife;
        HealHealth(LifeToAddToMaxLife);
        yield return new WaitForSeconds(DurationInSeconds);
        TakeDamages(LifeToAddToMaxLife);
        maxLife = maxLifeTemp;
        coroutineRunning = false;
    }

    /*
     Method for healing the player, cannot heal negative health, cannot heal above maxhealth, int lifeToHeal is added to the players health. 
     Checks if LifeToHeal is a negative number, if not it is added to the players 

         */

    public void HealHealth(int lifeToHeal)
    {

        int PositiveValue = lifeToHeal > negativeCheck ? lifeToHeal : negativeCheck;

        if (PositiveValue + playerHealth < maxLife)
        {

            playerHealth += lifeToHeal;
        }
        else
        {
            playerHealth = maxLife;
        }
    }

    /*
     two methods for taking damage, checks for negative inputs then subtracts the AmountOfDamagefrom the playerHealth,
     an overloaded stagger version for microslowing the player, nothing done with that yet.
         */

    public void TakeDamages(int AmountOfDamage)
    {
        int PositiveValue = AmountOfDamage > negativeCheck ? AmountOfDamage : negativeCheck;

        playerHealth -= PositiveValue;
        CheckIfDied();

    }



    public void TakeDamages(int AmountOfDamage, float stagger)
    {
        int PositiveValue = AmountOfDamage > negativeCheck ? AmountOfDamage : negativeCheck;

        playerHealth -= PositiveValue;
        CheckIfDied();
        //use stagger to stagger player just a suggestion, messes with gameflow maybe.
    }

    /*
     Deathcheck check if playerHealth is below 0 or at 0. Then will start the deathsequence
     */


    private void CheckIfDied()
    {
        if (playerHealth <= outOfLifeInt)
        {
            //What should happen when player dies//
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetMaxLife(30, 3);
        }
    }


}
