
using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
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
            switch (Comando[0].ToLower())
            {
                case "actualdebug":     //Funzione che varia in base alle esigenze di debug
                    ActualDebug();
                    break;
                case "startserver":       //Crea un nuovo server da comando
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().CreateServer(Comando[1]);
                    break;
                case "killnetwork":
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().KillThreads();
                    break;
                case "popup":

                    GameObject.Find("Canvas").GetComponent<MenuHandler>().MenuElements.Where(obj => obj.name.Equals("ErrorPopup")).SingleOrDefault().SetActive(true);
                    GameObject.FindWithTag("ErrorText").GetComponent<TextMeshProUGUI>().text += '\n' + Comando[1];
                    break;
                case "networklist":
                    foreach (Thread T in GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().ThreadList)
                        settings.Console_Write(T.Name + ": nella lista");
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
