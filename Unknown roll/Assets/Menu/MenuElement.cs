using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuElement : MonoBehaviour {

    public bool activeOnStart = false;
	// Aggiunge questo elemento alla lista degli elementi del menu
	void Start ()
    {
        GameObject.Find("Canvas").GetComponent<MenuHandler>().MenuElements.Add(gameObject);
        gameObject.SetActive(activeOnStart);
	}
	
}
