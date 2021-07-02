using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSwitch : MonoBehaviour
{
    public GameObject actionMenu;
    public GameObject skillMenu;
    
    // Start is called before the first frame update
    void Start()
    {
        actionMenu = GameObject.FindGameObjectWithTag("ActMenu");
        skillMenu = GameObject.FindGameObjectWithTag("SkillMenu");
        skillMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //switch between players action and skill menus
        if (!(actionMenu.activeSelf))
        {
            skillMenu.SetActive(true);
        }
        if (!(skillMenu.activeSelf))
        {
            actionMenu.SetActive(true);
        }
    }
}
