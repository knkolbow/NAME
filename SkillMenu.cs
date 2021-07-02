using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMenu : MonoBehaviour
{
    public GameObject skillMenu;
    public GameObject anObject;
    // Start is called before the first frame update
    void Start()
    {
        skillMenu = GameObject.FindGameObjectWithTag("SkillMenu");
        skillMenu.SetActive(false);
        anObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
