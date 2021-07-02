using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Button : MonoBehaviour
{
    public GameObject actionMenu;
    public GameObject skillMenu;
    [SerializeField]
    private bool physical;
    GameObject enemyChar;
    GameObject playerChar;
    GameObject combatController;
    EnemyCharacter enemyscript;
    PlayerCharacter playerscript;
    CombatControlScript controlscript;
    // Start is called before the first frame update
    void Start()
    {
        

        //sets gameObject name to the name of the button pressed
        string temp = gameObject.name;
        //add action listener and call on the buttons action
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => AddCallback(temp));
        
        //get active game objects and their needed scripts
        actionMenu = GameObject.FindGameObjectWithTag("ActMenu");
        skillMenu = GameObject.FindGameObjectWithTag("SkillMenu");
        enemyChar = GameObject.FindGameObjectWithTag("EnemyChar");
        playerChar = GameObject.FindGameObjectWithTag("PlayerChar");
        combatController = GameObject.FindGameObjectWithTag("Controller");
        playerscript = playerChar.GetComponent<PlayerCharacter>();
        enemyscript = enemyChar.GetComponent<EnemyCharacter>();
        controlscript = combatController.GetComponent<CombatControlScript>();

    }
    //ethod to call and perform action based on button pressed
    private void AddCallback(string button)
    {
        //player attack action via button
        if (button.CompareTo("Melee Button") == 0)
        {
            //calls attack from player script
            playerscript.Attack();
            // changes turn
            controlscript.playerturn = false;
            
        }
        else if (button.CompareTo("Skill Button") == 0)
        {
            
            actionMenu.SetActive(false);
            skillMenu.SetActive(true);
            
        }
        else if (button.CompareTo("Back Button") == 0)
        {
            //return To previous menu
            skillMenu.SetActive(false);
            actionMenu.SetActive(true);
            
        }
        else if (button.CompareTo("Ult Button") == 0)
        {
            
            playerscript.Ult();
            skillMenu.SetActive(false);
            actionMenu.SetActive(true);
            

        }
        else if (button.CompareTo("Fire Button") == 0)
        {
            
            playerscript.Fire();
            skillMenu.SetActive(false);
            actionMenu.SetActive(true);
            
        }
        else if (button.CompareTo("Run Button") == 0)
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
    
}
