using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Database;
using Firebase.Auth;
using System.Linq;
using System;

public class MainMenuButton : MonoBehaviour
{
    // Database references
    public DatabaseReference DBreference;
    public DatabaseReference database;
    public FirebaseUser user;
    string userId;

    // Player info screen
    public GameObject char1;
    public GameObject char2;
    public GameObject playerInfoScreen;
    public TMP_Text message;
    public TMP_Text usernameText;
    public TMP_Text levelText;
    public TMP_Text killsText;
    public TMP_Text deathsText;
    public TMP_Text highscoreText;
    public TMP_InputField newUsername;
    string level, kills, deaths, highscore, username;

    void Start()
    {
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(GetUserData());   
    }

    public void startButton()
    {
        GameManager.instance.ChangeScene(2);
    }

    public void leaderButton()
    {
        GameManager.instance.ChangeScene(4);
    }

    public void characterMenu()
    {
        GameManager.instance.ChangeScene(3);
    }

    public void signOut()
    {
        DBreference.Child("users").Child(userId).Child("active").SetValueAsync("false");
        DBreference.Child("users").Child(userId).Child("lastLogedIn").SetValueAsync(DateTime.Now.ToString());
        FirebaseManager.auth.SignOut();
        GameManager.instance.ChangeScene(0);
    }

    public void playerInfo()
    {
        StartCoroutine(GetUserData());              
        char1.SetActive(false);
        char2.SetActive(false);
        playerInfoScreen.SetActive(true);
        usernameText.text = username;
        levelText.text = "Level: " + level;
        killsText.text = "Kills: " + kills;
        deathsText.text = "Deaths: " + deaths;
        highscoreText.text = "Highscore: " + highscore;
    }

    public void exitButton()
    {
        char1.SetActive(true);
        char2.SetActive(true);
        playerInfoScreen.SetActive(false);
        newUsername.text = "";
        message.text = "";
    }

    public void changeUsername()
    {
        string tempName = newUsername.text;
        if (tempName.Length > 20)
        {
            message.text = "Username too long";
            newUsername.text = "";
        }
        else
        {
            StartCoroutine(ChangeUsername(tempName));
            StartCoroutine(UpdateUsernameAuth(tempName));
            ChangeLevelUsername(tempName);
        }
    }


    private IEnumerator ChangeUsername(string newName)
    {
        var DBTask = DBreference.Child("users").Child(userId).Child("username").SetValueAsync(newName);
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            message.text = "Username Changed!";
            username = newName;
            usernameText.text = username;
            newUsername.text = "";
        }
    }

    private void ChangeLevelUsername(string newName)
    {
        int currentLevel = int.Parse(level);
        if (currentLevel == 1)
        {
            DBreference.Child("levelone").Child(userId).Child("username").SetValueAsync(newName);
        }
        else if (currentLevel == 2)
        {
            DBreference.Child("leveltwo").Child(userId).Child("username").SetValueAsync(newName);
        }
        else if (currentLevel == 3)
        {
            DBreference.Child("levelthree").Child(userId).Child("username").SetValueAsync(newName);
        }
        else if (currentLevel == 4)
        {
            DBreference.Child("levelfour").Child(userId).Child("username").SetValueAsync(newName);
        }
    }

    private IEnumerator UpdateUsernameAuth(string name)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = name };

        //Call the Firebase auth update user profile function passing the profile with the username
        FirebaseManager.user.UpdateUserProfileAsync(profile);
        var ProfileTask = FirebaseManager.user.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }


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
            level = snapshot.Child("level").Value.ToString();
            kills = snapshot.Child("kills").Value.ToString();
            deaths = snapshot.Child("deaths").Value.ToString();
            highscore = snapshot.Child("highscore").Value.ToString();
        }
    }
}
