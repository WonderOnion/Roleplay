using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    private Settings settings;
    public List<GameObject> MenuElements = new List<GameObject>();
    private bool ConsoleDown = false;
    private bool MenuPosition = false;                      //se true lo sfondo viene spostato, altrimenti si trova nella posizione iniziale



    private void Start()
    {
        settings = GameObject.Find("Main Camera").GetComponent<Settings>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            GameObject temp = MenuElements.Where(obj => obj.name == "Console").SingleOrDefault();
            if (!temp.activeSelf)
                    temp.SetActive(true);
                else
                    temp.SetActive(false);
        }
    }

    public void SwitchMenu (string Temp)
    {
        try
        {
            if (Temp.Split(',').Length != 2)
            {
                settings.Error_Profiler("D002", 0, "SwitchMenu: " + Temp,3);
                return;
            }
            GameObject Actual = MenuElements.Where(obj => obj.name.Equals(Temp.Split(',')[0])).SingleOrDefault();
            GameObject NewOne = MenuElements.Where(obj => obj.name.Equals(Temp.Split(',')[1])).SingleOrDefault();
            if (NewOne == null || Actual == null)
            {
                settings.Error_Profiler("M001", 0, "Switch_menu not found: " + Temp,3);
                return;
            }

            if (Actual.name.Equals("MainMenu"))
                gameObject.GetComponent<Animator>().SetBool("OnScreen", false);
            else
                Actual.SetActive(false);
            //GameObject.Find("Canvas/" + Actual.name).GetComponent<Animator>().SetBool("OnScreen", false);

            if (NewOne.name.Equals("MainMenu"))
                gameObject.GetComponent<Animator>().SetBool("OnScreen", true);
            else
                NewOne.SetActive(true);

        }
        catch (Exception e)
        {
            settings.Error_Profiler("D001", 0, Temp + e,2);
        }
    }               //Gestisce i movimenti all'interno di un menu annidiato
    
    public void KillPopUp (string Name)
    {
        try
        {
            GameObject.FindWithTag("ErrorText").GetComponent<TextMeshProUGUI>().text = "";
            MenuElements.Where(obj => obj.name.Equals("ErrorPopup")).SingleOrDefault().SetActive(false);
        }
        catch(Exception e)
        {
            settings.Error_Profiler("M004", 0, "Pop up " + Name +" non found: " + e, 2);
        }

    }
}