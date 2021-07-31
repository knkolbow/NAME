using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Linq;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    //database reference
    public static Leaderboard instance;
    public DatabaseReference DBreference;
    public TMP_Text UsernameText;
    public TMP_Text LevelText;
    public TMP_Text KillsText;
    public TMP_Text DeathsText;
    public TMP_Text HighscoreText;
    public GameObject MainMenuButton;
    public GameObject LeaderboardButton;
    public GameObject MostKillsButton;

    // Start is called before the first frame update
    void Start()
    {
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(LoadLeaderboardData());
    }

    public void GoToMainMenu()
    {
        GameManager.instance.ChangeScene(1);
    }

    public void KillsButton()
    {
        StartCoroutine(LoadMostKillsData());
    }

    public void LeaderButton()
    {
        StartCoroutine(LoadLeaderboardData());
    }


    private void ClearScreen()
    {
        UsernameText.text = "";
        LevelText.text = "";
        KillsText.text = "";
        DeathsText.text = "";
        HighscoreText.text = "";
    }

    private IEnumerator LoadLeaderboardData()
    {

        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild("highscore").LimitToLast(10).GetValueAsync();


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            //Clear existing data
            ClearScreen();
            string usernames = "";
            string levels = "";
            string kills = "";
            string deaths = "";
            string highscores = "";

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                string kill = childSnapshot.Child("kills").Value.ToString();
                string death = childSnapshot.Child("deaths").Value.ToString();
                string level = childSnapshot.Child("level").Value.ToString();
                string score = childSnapshot.Child("highscore").Value.ToString();

                usernames = usernames + username + System.Environment.NewLine;
                levels = levels + level + System.Environment.NewLine;
                kills = kills + kill + System.Environment.NewLine;
                deaths = deaths + death + System.Environment.NewLine;
                highscores = highscores + score + System.Environment.NewLine;
            }
            UsernameText.text = usernames;
            LevelText.text = levels;
            KillsText.text = kills;
            DeathsText.text = deaths;
            HighscoreText.text = highscores;
        }
    }


    private IEnumerator LoadMostKillsData()
    {

        //Get all the users data ordered by kills amount
        var DBTask = DBreference.Child("users").OrderByChild("kills").LimitToLast(10).GetValueAsync();


        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Data has been retrieved
            DataSnapshot snapshot = DBTask.Result;
            //Clear existing data
            ClearScreen();
            string usernames = "";
            string levels = "";
            string kills = "";
            string deaths = "";
            string highscores = "";

            //Loop through every users UID
            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse<DataSnapshot>())
            {
                string username = childSnapshot.Child("username").Value.ToString();
                string kill = childSnapshot.Child("kills").Value.ToString();
                string death = childSnapshot.Child("deaths").Value.ToString();
                string level = childSnapshot.Child("level").Value.ToString();
                string score = childSnapshot.Child("highscore").Value.ToString();

                usernames = usernames + username + System.Environment.NewLine;
                levels = levels + level + System.Environment.NewLine;
                kills = kills + kill + System.Environment.NewLine;
                deaths = deaths + death + System.Environment.NewLine;
                highscores = highscores + score + System.Environment.NewLine;
            }
            UsernameText.text = usernames;
            LevelText.text = levels;
            KillsText.text = kills;
            DeathsText.text = deaths;
            HighscoreText.text = highscores;
        }
    }


}
