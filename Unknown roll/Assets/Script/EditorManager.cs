using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorManager : MonoBehaviour {
	/* POSIZIONE EDITOR
	 * 0: null
	 * 1: crea o carica progetto
	 * 100: in editor
	 */
	public int PosizioneEditor = 1;
	/* LISTA MID SCREEN
	 * 0: null
	 * 1: tabella per creare un blocco
	 */
	private int SubPosMidScreen = 0;

	public GameObject PrefabCella;

	public Camera camera;
	public bool RaycastMouse = false;
	public GameObject OggettoRaycast;

	// Variaibli GUI
	private Vector2 scrollPos = Vector2.zero;

	// Script esterni
	MenuGUI ManagerGui;


	public GameObject MainManager;

	void Start () {
		ManagerGui = MainManager.GetComponent<MenuGUI> ();
	}

	void Update () {

		// Gestione raycast
		raycast ();
	}

	void OnGUI() {
		if(ManagerGui.PosizioneNelMenu == 2) {
			if(PosizioneEditor == 1) {
				if(GUI.Button (new Rect (Screen.width/2-100,Screen.height/2-75,200,50),"NUOVA MAPPA")) {
					PosizioneEditor = 2;
				} if(GUI.Button (new Rect (Screen.width/2-100,Screen.height/2+25,200,50),"CARICA MAPPA")) {
					PosizioneEditor = 3;
				}
			}
			if(PosizioneEditor == 2) {

			}
			if(PosizioneEditor == 3) {
				scrollPos = GUI.BeginScrollView(new Rect(Screen.width/2-150, Screen.height/2+300, 300, 500), scrollPos, new Rect(0, 0, 10, 500));



				GUI.EndScrollView();
			}

			// UTILIZZO EDITOR
			if(PosizioneEditor == 100) {
				scrollPos = GUI.BeginScrollView(new Rect(0, 0, Screen.width, 50), scrollPos, new Rect(0, 0, Screen.width, 10));
				// INIZIO BARRA
				if(RaycastMouse) {
					if(GUI.Button (new Rect (0,0,150,50),"ACTIVE Mouse R.")) {
						RaycastMouse = true;
					}
				} else {
					if(GUI.Button (new Rect (0,0,150,50),"BLOCK Mouse R.")) {
						RaycastMouse = false;
					}
				}
				if(GUI.Button (new Rect (150,0,100,50),"New Cells")) {
					SubPosMidScreen = 1;
				}
				if(GUI.Button (new Rect (250 ,0,100,50),"Save Map")) {

				}
				// FINE BARRA
				GUI.EndScrollView();

				// GUI NEL MEZZO DELLO SCHERMO
				if(SubPosMidScreen == 1) {
					// Spawna pannello per generare un nuovo blocco
					string INIZIO_X = "" , INIZIO_Y = "", LUNGHEZZA_X = "", LUNGHEZZA_Y = "";

					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-200, 50, 15), "Start position X");
					INIZIO_X = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-175, 50, 20), INIZIO_X, 30);
					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-150, 50, 15), "Start position Y");
					INIZIO_Y = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-135, 50, 20), INIZIO_Y, 30);

					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-100, 50, 15), "Number of cells in X");
					LUNGHEZZA_X = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-85, 50, 20), LUNGHEZZA_X, 25);
					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-60, 50, 15), "Number of cells in Y");
					LUNGHEZZA_Y = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-45, 50, 20), LUNGHEZZA_Y, 25);

					if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2, 100, 20), "Generate")) {
						generaBlocco (int.Parse(INIZIO_X), int.Parse(INIZIO_Y), int.Parse(LUNGHEZZA_X), int.Parse(LUNGHEZZA_Y)); // genera blocco
						SubPosMidScreen = 0;
					}
					if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2+30, 100, 20),"Back")) {
						SubPosMidScreen = 0;
					}
				}
			}
		}
	}

	private void raycast() {
		if(RaycastMouse) {
			RaycastHit hit;
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit)) {
				OggettoRaycast = hit.transform.gameObject;
			}
		}
	}

	// funzione che genera un blocco
	public void generaBlocco(int InizioX, int InizioY, int LunghezzaX, int LunghezzaY) {
		GameObject Cella = null;

		for(int X=0; X < LunghezzaX; X++) {
			for(int Y=0; Y < LunghezzaY; Y++) {
				if(!cellaPresenteInPos(LunghezzaX, LunghezzaY)) {
					Cella = (GameObject)Instantiate (PrefabCella, new Vector3 (InizioX, InizioY, 0), Quaternion.identity) as GameObject;
					//Cella.name = 
				}
			}
		}
	}

	public bool cellaPresenteInPos(int PosizioneX, int PosizioneY) {
		return false;
	}
}
