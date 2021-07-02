using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class CharMenuButtons : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {
        string temp = gameObject.name;
        //add action listener and call on the buttons action
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => AddCallback(temp));
    }

    
    private void AddCallback(string button)
    {

        if (button.CompareTo("Main Menu Button") == 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
        
    }
}
