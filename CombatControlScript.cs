using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatControlScript : MonoBehaviour
{
    //create object and script variables
    GameObject enemyChar;
    GameObject playerChar;
    EnemyCharacter enemyscript;
    PlayerCharacter playerscript;
    public bool playerturn;
    
    // Start is called before the first frame update
    void Start()
    {
        //set object and script variables
        enemyChar = GameObject.FindGameObjectWithTag("EnemyChar");
        playerChar = GameObject.FindGameObjectWithTag("PlayerChar");
        playerscript = playerChar.GetComponent<PlayerCharacter>();
        enemyscript = enemyChar.GetComponent<EnemyCharacter>();
        //start with player turn
        playerturn = true;  
    }

    // Update is called once per frame
    void Update()
    {
        //check if it is player or enemy turn
        //if enemy turn
        if (!playerturn)
        {
            //give player and enemy 1 mana
            playerscript.mana += 1;
            enemyscript.mana += 1;
            //need some way to pause actions here
            //currently enemy moves at a frame after the player, need a clear break, will look for fixes
            //checks if enemy has 10 or more mana
            if (enemyscript.mana >= 10)
            {
                //removes 10 mana and uses enemy ult
                enemyscript.mana -= 10;
                enemyscript.Ult();
            }
            //if less than 10 mana enemy does simple attack
            else
            {
                enemyscript.Attack();
            }
            //need some way to pause actions here
            //currently enemy moves at a frame after the player, need a clear break, will look for fixes
            playerturn = true;
        }
        //when player runs out of health goes back to main menu
        //plan to use a game over/save screen instead.
        if(playerscript.health <= 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
