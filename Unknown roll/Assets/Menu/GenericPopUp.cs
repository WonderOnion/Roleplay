using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPopUp : MonoBehaviour {

    private Settings settings;


    public string PopUpID = "";                         //Utilizzata per capire la posizione e lo script che si vuole eseguire
    public bool DirectConnectOrHostGame = false;        //utilizzata in Play per capire se il PopUP deve mostrare la connessione diretta (false) oppure l'host (true)


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
                if (!DirectConnectOrHostGame)
                {
                    Childrens[0].SetActive(true);
                    Childrens[1].SetActive(false);
                }
                else
                {
                    Childrens[0].SetActive(false);
                    Childrens[1].SetActive(true);
                }
                break;
            default:
                settings.Error_Profiler("G003", 0, "GenericPopUP => " + PopUpID, 4);
                break;
        }
	}

    public void ClosePopUP ()
    {
        gameObject.SetActive(false);
    }

}
