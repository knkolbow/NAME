using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MainMenuButton : MonoBehaviour
{
    
   
    // Start is called before the first frame update
    void Start()
    {
        string temp = gameObject.name;
        //add action listener and call on the buttons action
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => AddCallback(temp));
    }

    // Update is called once per frame
    private void AddCallback(string button)
    {

        if (button.CompareTo("Start Button") == 0)
        {
            SceneManager.LoadScene("TurnBasedCombat");
        }
        else if (button.CompareTo("Char Button") == 0)
        {
            SceneManager.LoadScene("CharMenu");
        }
        else if (button.CompareTo("Leader Button") == 0)
        {
            SceneManager.LoadScene("Leaderboard");
        }
    }
}
