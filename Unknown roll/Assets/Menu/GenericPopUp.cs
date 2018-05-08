using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPopUp : MonoBehaviour {

    private Settings settings;


    public string PopUpID = "";                         //Utilizzata per capire la posizione e lo script che si vuole eseguire
    public int DirectConnectOrHostGame = 2;        //utilizzata in Play per capire se il PopUP deve mostrare la connessione diretta (0) oppure l'host (1) o nessuno (2)


    public List<GameObject> Childrens;                  //Utilizzato dalle funzioni che richiedono più children, la divisone dello scopo avviene tramite le variabili booleane



    void Start ()
    {
        settings = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Settings>();
	}
	
	
	void Update ()
    {
		switch (PopUpID)
        {
            case "Error":
                break;
            case "PlayDirectHost":      //see also in MenuHandler.cs
                if (DirectConnectOrHostGame == 0)
                {
                    Childrens[0].SetActive(true);
                    Childrens[1].SetActive(false);
                }
                else if (DirectConnectOrHostGame == 1)
                {
                    Childrens[0].SetActive(false);
                    Childrens[1].SetActive(true);
                }
                else
                {
                    Childrens[0].SetActive(false);
                    Childrens[1].SetActive(false);

                }
                break;
            default:
                settings.Error_Profiler("G003", 0, "GenericPopUP => " + PopUpID + "         Called By: " + gameObject.name, 4, true);
                break;
        }
	}

    public void ClosePopUP ()
    {
        gameObject.SetActive(false);
    }

}
