﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    private Settings settings;              
    public List<GameObject> MenuElements;          //lista che comprende tutte le schede del menu
    private Commands DoCommand = new Commands();
    public GameObject ConsoleOBJ;



    private void Start()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
        DoCommand.settings = settings;

        if (settings == null)
        {
            settings.Error_Profiler("D003", 0, "MenuHandler => Start => Settings", 2, true);
            return;
        }
        else
            if (ConsoleOBJ == null)
            {
                settings.Error_Profiler("D003", 0, "MenuHandler => Start => Console", 2, true);
                return;
            }
    }


    private void Update()
    {
        //Controllo per aprire o meno la console

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            if (ConsoleOBJ == null)
            {
                settings.Error_Profiler("D001", 0, "Cannot find console", 4, true);
                return;
            }
            if (!ConsoleOBJ.activeSelf)
            {
                ConsoleOBJ.SetActive(true);
                ConsoleOBJ.transform.Find("ConsoleInput").GetComponent<InputField>().Select();
            }
            else
                ConsoleOBJ.SetActive(false);
        }

        if (ConsoleOBJ.transform.Find("ConsoleInput").GetComponent<InputField>().isFocused && ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text != "" && Input.GetKeyDown(KeyCode.Tab))
        {
            settings.Console_Write(ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text, false);

            DoCommand.Esegui_Comando(ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text);

            ConsoleOBJ.transform.Find("ConsoleInput/Text").GetComponent<Text>().text = "";
        }
    }

    public void AddMenuItem (GameObject Item)
    {
        lock (MenuElements)
            if (MenuElements.Find(X => X == Item) == null)
                MenuElements.Add(Item);
    }

    public void SwitchMenu (string Temp)
    {
        try
        {
            //Controllo che venga passato il giusto numero di argomenti
            if (Temp.Split(',').Length != 2)
            {
                settings.Error_Profiler("D002", 0, "SwitchMenu: " + Temp,3, true);
                return;
            }
            //ricerco e controllo esistano quei due menu
            GameObject Actual = MenuElements.Where(obj => obj.name.Equals(Temp.Split(',')[0])).SingleOrDefault();
            GameObject NewOne = MenuElements.Where(obj => obj.name.Equals(Temp.Split(',')[1])).SingleOrDefault();
            if (NewOne == null || Actual == null && !Temp.Split(',')[0].ToLower().Equals("all"))
            {
                settings.Error_Profiler("M001", 0, "(MenuHandler => Switch_menu) not found: " + Temp,3, true);
                return;
            }

            if (Temp.Split(',')[0].ToLower().Equals("all"))
            {
                foreach (GameObject T in MenuElements)
                {
                    if (!T.name.Equals("MainMenu"))
                        T.SetActive(false);
                    else
                        Actual = T;
                }
            } 

            //Se il menu attuale è il MainMenu allora va a cambiare il valore di OnScreen Nell'animator, altrimenti disattiva la scheda corrente
            if (Actual.name.Equals("MainMenu"))
            gameObject.GetComponent<Animator>().SetBool("OnScreen", false);
            Actual.SetActive(false);
            //Se il menu in cui si vuole andare è il MainMenu allora va a cambiare il valore di Onscreen nell'animator a true, per permmetterne la visione, altrimenti imposta attivo il menu richiesto
            if (NewOne.name.Equals("MainMenu"))
                gameObject.GetComponent<Animator>().SetBool("OnScreen", true);
            NewOne.SetActive(true);

        }
        catch (Exception e)
        {
            settings.Error_Profiler("D001", 0, Temp + e,2, true);
        }
    }               //Gestisce i movimenti all'interno di un menu annidiato
    

    /// <summary>
    /// Serve per attivare un pop up dato il nome e nel caso delle identifidicazioni specifiche
    /// </summary>
    /// <param name="Name">nome del popup e relative informazioni divise da una virgola</param>
    public void CallPopUPByName(string Name)
    {
        //Instazio un vettore di 5 elementi che potrei utilizzare in seguito (è necessario inizializzarla al fine i integrità del codice)
        string[] SubStrings = new string[5];

        //Controllo se all'interno della stringa sono presenti delle virgole, se si li splitto e li assegno al vettore, mettendo il primo in Name, essendo il nome del popUP
        if (Name.IndexOf(',') >= 0)
        {
            SubStrings = Name.Split(',');
            Name = SubStrings[0];
        }

        // Guardo se esiste il popUP richiesto, se esiste lo attivo

        GameObject PopUP = MenuElements.Find(T => T.name.Equals(Name));
        if (PopUP == null && Name.Equals("All"))
            lock (MenuElements)
            {
                foreach (GameObject T in MenuElements)
                    if (!T.name.Equals("Sfondo"))
                        T.SetActive(false);
            }
        else
        {
            settings.Error_Profiler("G003", 0, "CallPopUpByName => Name: " + Name, 4, true);
            return;
        }
        PopUP.SetActive(true);

        //Eseguo uno switch sul PopUp appena avviato per capire se ha delle funzioni da eseguire durante l'avvio
        switch (PopUP.GetComponent<GenericPopUp>().PopUpID)     //see also in GenericPopUP
        {
            case "PlayDirectHost":
                PopUP.GetComponent<GenericPopUp>().DirectConnectOrHostGame = Int32.Parse(SubStrings[1]);
                break;
        }
    }

    public void KillPopUp (string Name)
    {
        try
        {
            GameObject.FindWithTag("ErrorText").GetComponent<TextMeshProUGUI>().text = "";
            MenuElements.Where(obj => obj.name.Equals("ErrorPopup")).SingleOrDefault().SetActive(false);
        }
        catch(Exception e)
        {
            settings.Error_Profiler("M004", 0, "Pop up " + Name +" non found: " + e, 2, true);
        }

    }

    public void KillApllication()
    {
        GameObject.FindGameObjectWithTag("NewtorkHandler").GetComponent<NetworkHandler>().KillThreads();
        Application.Quit();
    }
}