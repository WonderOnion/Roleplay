using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuGUI : MonoBehaviour {

	/* LISTA MENU
	 * 0: null
	 * 1: menu principale
	 * 2: editor
	 */
	public int PosizioneNelMenu = 1;
	private int SubPosizione = 0;

	// Script esterni
	MenuManager MenuGui;

	void Start () {
		MenuGui = GetComponent<MenuManager> ();
	}

	void Update () {
		
	}

	void OnGUI() {
		if(MenuGui.UsaMenuDiDebug) { // ATTIVA MENU
			// Menu principale
			if(PosizioneNelMenu == 1) {
				if(SubPosizione == 0) {
					if(GUI.Button (new Rect (0,0,200,50),"EDITOR")) {
						PosizioneNelMenu = 2;
						SubPosizione = 0;
					}
				}
			} 
			// Menu editor
			else if(PosizioneNelMenu == 2) {
				if(SubPosizione == 0) {
					if(GUI.Button (new Rect (Screen.width-100,Screen.height-30,100,30),"> QUIT <")) { // QUIT RAPIDO SENZA SALVATAGGIO
						PosizioneNelMenu = 1;
						SubPosizione = 0;
					}
				}
			}
		}
	}
}
