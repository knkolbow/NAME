using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Auth;

public class PlayerCharacter : MonoBehaviour
{
    //create player variables
    public int health;
    public int maxHealth;
    public int mana;
    int attack;
    public int speed;
    private TMP_Text healthText;
    public int exp = 0;
    //new random generator
    System.Random rnd = new System.Random();
    //create object and script variables
    GameObject enemyChar;
    GameObject playerChar;
    GameObject combatController;
    EnemyCharacter enemyscript;
    PlayerCharacter playerscript;
    CombatControlScript controlscript;
    //Database reference and variables
    DatabaseReference DBreference;
    string userId;
    static public string username;
    static public int level;
    static public int kills;
    static public int deaths;
    static public int highscore;
    
    // Start is called before the first frame update
    void Start()
    {
        // Get current players data
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(GetUserData());
        //Player max health
        maxHealth = 20;
        //amount of possible damage
        attack = 4;
        //starting mana
        mana = 10;
        //current health
        health = 20;
        enemyChar = GameObject.FindGameObjectWithTag("EnemyChar");
        playerChar = GameObject.FindGameObjectWithTag("PlayerChar");
        combatController = GameObject.FindGameObjectWithTag("Controller");
        playerscript = playerChar.GetComponent<PlayerCharacter>();
        enemyscript = enemyChar.GetComponent<EnemyCharacter>();
        controlscript = combatController.GetComponent<CombatControlScript>();
    }//end start function


    //player attack function
    public void Attack()
    {
        //reduce enemy health by attack value
        enemyscript.health -= (attack);
    }

    //fireball function
    public void Fire()
    {
        //check if current mana is 6 or more 
        if (mana >= 6)
        {
            //calculate fireball damage
            enemyscript.health -= (int)((attack * level) / 1.5);
            mana -= 6;
            // changes turn
            controlscript.playerturn = false;
        }
    }

    //player ult function
    public void Ult()
    {
        //check if current mana is 9 or more 
        if (mana >= 9)
        {
            
            enemyscript.health -= (int)((attack * (2.5 * level)));
            if (health <= (maxHealth - 5))
            {
                health += (5*level);
            }
            
            mana -= 9;
            // changes turn
            controlscript.playerturn = false;
        }
    }//end ult function
    
    
    //player AI function
    public void playerTakeTurn()
	{
		if (enemyscript.health > 0){
			if (playerscript.mana >= 9){
				playerscript.Ult();
			}
			else if (playerscript.mana >= 6){
				playerscript.Fire();
			}
			else {
				playerscript.Attack();
			}
		}
	}//end AI function
    
    


    void Update()
    {
        //set player status text
        healthText = GetComponentInChildren<TMP_Text>();
        healthText.SetText("HP: " + health + "\nMana:" + mana + "\nAttack:" + attack);
        //check if player has 100 experience
        if (exp >= 100)
        {
            //remove 100 experience from player for level up
            exp -= 100;
            levelUp();
            // changes turn
            enemyscript.enemylevelUp();
            

        }
    }//end update function

    //player level up sequence
   void levelUp()
    {
        //increase level by 1 up until level 4
        if (level < 4)
        {
            level += 1;
            StartCoroutine(UpdateLevel(level));
	    highscore += 10;
            StartCoroutine(UpdateHighscore(highscore));
            UpdateDatabase();
        }
        //increase level by 1
        level += 1;
        //check if level is below level cap for stat boost
        if (level <= 4)
        {
            //check if level is divisible by 2 for attack increase
            if (level % 2 == 0)
            {
                //increase posible damage by a range of 1-3
                attack = (attack + rnd.Next(1, 3));
            }
            //increase max health by a random range of 1-4
            maxHealth += rnd.Next(1, 4);
        }
        //if player is above max level for stat increase just improve health
        else
        {
            //increase max health by a random range of 1-4
            maxHealth += rnd.Next(1, 4);
        }
        //set health back to max upon level up
        health += maxHealth-health;
        //increase mana by 3 upon level up
        mana += 3;
    }//end levelup function
    
        private IEnumerator GetUserData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(userId).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
	    username = snapshot.Child("username").Value.ToString();
            highscore = int.Parse(snapshot.Child("highscore").Value.ToString());
            level = int.Parse(snapshot.Child("level").Value.ToString());
            kills = int.Parse(snapshot.Child("kills").Value.ToString());
            deaths = int.Parse(snapshot.Child("deaths").Value.ToString());
        }
    }
    
    
    private IEnumerator UpdateHighscore(int newScore)
    {
        //Set the currently logged in user level
        var DBTask = DBreference.Child("users").Child(userId).Child("highscore").SetValueAsync(newScore);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
    }

    private IEnumerator UpdateLevel(int newLevel)
    {
        //Set the currently logged in user level
        var DBTask = DBreference.Child("users").Child(userId).Child("level").SetValueAsync(newLevel);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //level is now updated
        }
    }
    
    
     private void UpdateDatabase()    // Update the players in each level
    {
        if (level == 2)
        {   // Remove player from level one table
            DBreference.Child("levelone").Child(userId).Child("username").RemoveValueAsync();
            DBreference.Child("levelone").Child(userId).RemoveValueAsync();

            // Add player to level two table
            DBreference.Child("leveltwo").Child(userId);
            DBreference.Child("leveltwo").Child(userId).Child("username").SetValueAsync(username);
        }
        if (level == 3)
        {   // Remove player from level two table
            DBreference.Child("leveltwo").Child(userId).Child("username").RemoveValueAsync();
            DBreference.Child("leveltwo").Child(userId).RemoveValueAsync();

            // Add player to level three table
            DBreference.Child("levelthree").Child(userId);
            DBreference.Child("levelthree").Child(userId).Child("username").SetValueAsync(username);
        }
        if (level == 4)
        {
            // Remove player from level three table
            DBreference.Child("levelthree").Child(userId).Child("username").RemoveValueAsync();
            DBreference.Child("levelthree").Child(userId).RemoveValueAsync();

            // Add player to level four table
            DBreference.Child("levelfour").Child(userId);
            DBreference.Child("levelfour").Child(userId).Child("username").SetValueAsync(username);
        }
    }
    

    
}
