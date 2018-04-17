using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class SettingsElement : MonoBehaviour {

    private Settings settings;
    private int Language = 0;                               //Variabile per identificar se vi sono stati dei cambi delle lingue

    public bool SelectedLanguageElement = false;            //se true andrà a caricare in questo game object tutte le lingua considerandolo come un drop down

	void Start ()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
        Language = settings.LanguageList.Count;
        gameObject.GetComponent<TMP_Dropdown>().ClearOptions();

    }
	
	
	void Update ()
    {
        //Elemento delle impostazioni che seleziona la lingua preferita dal giocatore tra quelle disponibili

        if (SelectedLanguageElement)
        {
            if (Language != settings.LanguageList.Count)
            {
                List<string>TempLang = new List<string>(settings.LanguageList);
                Language = TempLang.Count;
                int actualValue = 0;
                for (int I = 0; I < Language; I++)
                {
                    try
                    {
                        TempLang[I] = settings.Retrive_InnerText(-1, TempLang[I] + ",language/General/Name");
                        if (TempLang[I].Equals(settings.Retrive_InnerText(0, "language/General/Name")))
                            actualValue = I;

                    }
                    catch (Exception e)
                    {
                        settings.Error_Profiler("D001", 0, "SettingsElement => Language not found (Refresh)",1);
                    }
                }
                gameObject.GetComponent<TMP_Dropdown>().ClearOptions();
                gameObject.GetComponent<TMP_Dropdown>().AddOptions(TempLang);
                gameObject.GetComponent<TMP_Dropdown>().value = actualValue;
            }
        }
    }


    public void ChangeLanguage()
    {
        List<string> TempLang = settings.LanguageList;
        string NextLang = null;
        for (int I = 0; I < Language; I++)
        {
            try
            {
                string TempNameLang = settings.Retrive_InnerText(-1, TempLang[I] + ",language/General/Name");
                if (TempNameLang.Equals(gameObject.transform.FindChild("Label").GetComponent<TextMeshProUGUI>().text))
                {
                    NextLang = TempLang[I].Split('\\')[1].Split('.')[0];
                }
            }
            catch (Exception e)
            {
                settings.Error_Profiler("D001", 0, "SettingsElement => Language not found (Selecting)\n", 1);
            }
        }
        if (NextLang != null)
            settings.ChangeLanguage(NextLang);
        else
            settings.Error_Profiler("S001", 0, "Language requested not found", 5);

    }
}
