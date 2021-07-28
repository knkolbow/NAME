using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuButton : MonoBehaviour
{

public DatabaseReference DBreference;
    public FirebaseUser user;

    string userId;
    public TMP_Text usernameText;
    public TMP_Text levelText;
    public TMP_Text killsText;
    TMP_Text deathsText;

    void Start()
    {
        userId = FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
        usernameText = GameObject.Find("Main Menu/Menu Image/Username").GetComponent<TMP_Text>();
        levelText = GameObject.Find("Main Menu/Menu Image/Level").GetComponent<TMP_Text>();
        killsText = GameObject.Find("Main Menu/Menu Image/Kills").GetComponent<TMP_Text>();
        deathsText = GameObject.Find("Main Menu/Menu Image/Deaths").GetComponent<TMP_Text>();
        StartCoroutine(GetUserData());   
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
            usernameText.text = "Username: " + snapshot.Child("username").Value.ToString();
            levelText.text = "Level: " + snapshot.Child("level").Value.ToString();
            killsText.text = "Kills: " + snapshot.Child("kills").Value.ToString();
            deathsText.text = "Deaths: " + snapshot.Child("deaths").Value.ToString();
        }
    }

    public void startButton()
    {
        GameManager.instance.ChangeScene(2);
    }

    public void leaderButton()
    {
        GameManager.instance.ChangeScene(4);
    }

    public void settingsMenu()
    {
        GameManager.instance.ChangeScene(3);
    }

    public void signOut()
    {
        FirebaseManager.auth.SignOut();
        GameManager.instance.ChangeScene(0);
    }
}
