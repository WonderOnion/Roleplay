using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GenericSettingsRetriver : MonoBehaviour {

    private Settings settings;
    private int LanguageNumber = 0;                               //Variabile per identificar se vi sono stati dei cambi delle lingue

    public bool SelectedLanguageElement = false;            //se true andrà a caricare in questo game object tutte le lingua considerandolo come un drop down

	void Start ()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
        LanguageNumber = settings.LanguageList.Count;
        if (SelectedLanguageElement)
            gameObject.GetComponent<TMP_Dropdown>().ClearOptions();
        //IpRetriver to do (Play => Direct => IP input)
    }
	
	
	void Update ()
    {
        //Elemento delle impostazioni che seleziona la lingua preferita dal giocatore tra quelle disponibili

        if (SelectedLanguageElement)
        {
            if (LanguageNumber != settings.LanguageList.Count)
            {
                List<string>TempLang = new List<string>(settings.LanguageList);
                LanguageNumber = TempLang.Count;
                int actualValue = 0;
                for (int I = 0; I < LanguageNumber; I++)
                {
                    try
                    {
                        TempLang[I] = settings.Retrive_InnerText(-1, TempLang[I] + ",language/General/Name");
                        if (TempLang[I].Equals(settings.Retrive_InnerText(0, "language/General/Name")))
                            actualValue = I;

                    }
                    catch (Exception e)
                    {
                        settings.Error_Profiler("D001", 0, "SettingsElement => Language not found (Refresh): " + e,1);
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
        for (int I = 0; I < LanguageNumber; I++)
        {
            try
            {
                string TempNameLang = settings.Retrive_InnerText(-1, TempLang[I] + ",language/General/Name");
                if (TempNameLang.Equals(gameObject.transform.Find("Label").GetComponent<TextMeshProUGUI>().text))
                {
                    NextLang = TempLang[I].Split('\\')[1].Split('.')[0];
                }
            }
            catch (Exception e)
            {
                settings.Error_Profiler("D001", 0, "SettingsElement => Language not found (Selecting)\n"+e, 1);
            }
        }
        if (NextLang != null)
            settings.ChangeLanguage(NextLang);
        else
            settings.Error_Profiler("S001", 0, "Language requested not found", 5);

    }
}
