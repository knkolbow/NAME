using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading;
using Firebase.Database;
using Firebase.Auth;

public class CombatControlScript : MonoBehaviour
{
    //create object and script variables
    GameObject enemyChar;
    GameObject playerChar;
    EnemyCharacter enemyscript;
    PlayerCharacter playerscript;
    public bool playerturn;
    
    // database reference
    string userId;
    DatabaseReference DBreference;
    
    // Start is called before the first frame update
    void Start()
    {
        // database reference
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        //set object and script variables
        enemyChar = GameObject.FindGameObjectWithTag("EnemyChar");
        playerChar = GameObject.FindGameObjectWithTag("PlayerChar");
        playerscript = playerChar.GetComponent<PlayerCharacter>();
        enemyscript = enemyChar.GetComponent<EnemyCharacter>();
        //start with player turn
        playerturn = true;
        combatAI = false;
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

			
            //the enemy takes his turn based on the function in EnemyCharacter
            //can put Thread.Pause(300) here if needed
			enemyscript.enemyTakeTurn();

            //after enemy turn is complete, set playerturn to true
            playerturn = true;
        }
        //if combatAI is true, run through the player's turn
        else if (combatAI == true)
		{
            //player takes turn based on class in PlayerCharacter
			playerscript.playerTakeTurn();
			Thread.Sleep(300); //after AI takes turn, pause for 3 seconds
		}
        
        //when player runs out of health goes back to main menu
        //plan to use a game over/save screen instead.
        if(playerscript.health <= 0)
        {
            // Update player deaths
            PlayerCharacter.deaths += 1;
            StartCoroutine(UpdateDeaths(PlayerCharacter.deaths));
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    
        IEnumerator UpdateDeaths(int deaths)
    {
        //Set the currently logged in user deaths
        var DBTask = DBreference.Child("users").Child(userId).Child("deaths").SetValueAsync(deaths);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Deaths are now updated
        }
    }
}
