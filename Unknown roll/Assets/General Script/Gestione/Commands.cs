
using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class Commands : MonoBehaviour
{
    public Settings settings;


    public void Esegui_Comando (string TMPComando)
    {
        try
        {
            TMPComando.Replace(" ", string.Empty);     //rimuovo tutti gli spazi
            string[] Comando = TMPComando.Split(':');
            switch (Comando[0])
            {
                case "ActualDebug":     //Funzione che varia in base alle esigenze di debug
                    ActualDebug();
                    break;
                case "NewServer":       //Crea un nuovo server da comando
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().CreateServer(Comando[1]);
                    break;
                case "KillThreads":
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().KillThreads();
                    break;
                case "PopUp":

                    GameObject.Find("Canvas").GetComponent<MenuHandler>().MenuElements.Where(obj => obj.name.Equals("ErrorPopup")).SingleOrDefault().SetActive(true);
                    GameObject.FindWithTag("ErrorText").GetComponent<TextMeshProUGUI>().text += '\n' + Comando[1];
                    break;
                default:
                    settings.Console_Write("<color=\"yellow\">Comando non trovato: <color=\"red\">" +TMPComando);
                    break;
            }
        }
        catch (Exception e)
        {
            settings.Console_Write("<color=\"red\">Errore nell'esecuzione del comando: <color=\"yellow\">" + TMPComando + " <color=\"red\"> Codice errore: " + e);
        }
    }

    public void ActualDebug ()
    {

    }
}
