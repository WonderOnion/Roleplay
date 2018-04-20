using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GenericTxtBtn : MonoBehaviour
{
    public bool IsButton;           //variabile di controllo per capire se si tratta di un bottone o di un testo
    public string BeforeText;       //testo da aggiungere prima del testo tradotto
    public string InEnglish;
    public string AfterText;        //testo da aggiungere dopo il testo tradotto
    private Settings settings;      //oggetto di riferimento per trovare la lingua
    public string Location;         //location all'interno del file xml
    private string Language = null; //variabile temporanea che controlla se è stato eseguito un cambio di lingua

    private void Start()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Language != settings.Language)
            {
                Language = settings.Language;
                if (settings.Language.Equals("en"))
                    if (IsButton)
                        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = BeforeText + InEnglish + AfterText;
                    else
                        gameObject.GetComponent<TextMeshProUGUI>().text = BeforeText + InEnglish + AfterText;
                else
                {
                    if (IsButton)
                        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = BeforeText + settings.Retrive_InnerText(0, "language/" + Location + gameObject.name) + AfterText;
                    else
                        gameObject.GetComponent<TextMeshProUGUI>().text = BeforeText + settings.Retrive_InnerText(0, "language/" + Location + gameObject.name) + AfterText;

                    if (settings.Retrive_InnerText(0, "language/" + Location + gameObject.name).Equals("NoText"))
                        settings.Error_Profiler("G004", 0, "Language: " + Language + "             path: " + "language/" + Location + gameObject.name,0);
                }
                
            }
        } catch (Exception e)
        {
            settings.Error_Profiler("M003", 0, gameObject.name + ": " + e.ToString(),2);
        }
    }
}
