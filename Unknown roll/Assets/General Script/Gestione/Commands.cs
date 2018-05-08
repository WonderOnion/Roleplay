
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
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().Create_Server(Comando[1]);
                    break;
                case "serverdebug":
                    if (GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Server>().D)
                        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Server>().D = false;
                    else
                        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Server>().D = true;
                    break;
                case "clientdebug":
                    if (GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Client>().D)
                        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Client>().D = false;
                    else
                        GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Client>().D = true;
                    break;
                case "connect":       //Si collega ad un server
                    GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<NetworkHandler>().Create_Client(Comando[1] + ":" + Comando[2]);
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
                        settings.Console_Write(T.Name + ": nella lista",true);
                    break;
                default:
                    settings.Console_Write("<color=\"yellow\">Comando non trovato: <color=\"red\">" +TMPComando, true);
                    break;
            }
        }
        catch (Exception e)
        {
            settings.Console_Write("<color=\"red\">Errore nell'esecuzione del comando: <color=\"yellow\">" + TMPComando + " <color=\"red\"> Codice errore: " + e, true);
        }
    }

    public void ActualDebug ()
    {

    }
}
