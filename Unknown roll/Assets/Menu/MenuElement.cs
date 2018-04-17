using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MenuElement : MonoBehaviour {

    public bool activeOnStart = false;
    private bool Doit = false;
	// Aggiunge questo elemento alla lista degli elementi del menu
	void Update ()
    {
        if (!Doit)
        {
            GameObject.Find("Canvas").GetComponent<MenuHandler>().AddMenuItem(gameObject);
            gameObject.SetActive(activeOnStart);
            Doit = true;
        }
	}
	
}
