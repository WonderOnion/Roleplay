using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
	// variabili per generare un nuovo blocco
	private string INIZIO_X = "" , INIZIO_Y = "", LUNGHEZZA_X = "", LUNGHEZZA_Y = "";

	// Gestione mappa
	public bool SpawnaMappa = false;
	public bool GeneraMappa =  false;
	private string NomeMappa = "";
	private int[][] PosizioneCelle;

	// Caricare partita
	List<string> MappeDelGioco;
	private int PosizioneGuiListaCaricaPartita = 0;
	private string TestoRigaFile = " ";
	private int I_FOR_DEBUG_GUI_CaricamentoPartita = 0;

	// Script esterni
	MenuGUI ManagerGui;

	// FILE MANAGER
	public FileManager File = new FileManager();
	private GameObject[] CelleDellaMappa;

	public GameObject MainManager;

	void Start () {
		ManagerGui = MainManager.GetComponent<MenuGUI> ();

		// Genera file base
		if(!File.esisteDirectory(File.MainDirectory)) {
			File.generaDirectoryBase();
		}
	}

	void Update () {

		// Gestione raycast
		raycast ();

		// Spawna un blocco di cella
		if(SpawnaMappa) {
			generaBlocco (int.Parse(INIZIO_X), int.Parse(INIZIO_Y), int.Parse(LUNGHEZZA_X), int.Parse(LUNGHEZZA_Y));
		}

		// Genera tutta la mappa attraverso il file della cartella
		if(GeneraMappa) {
			generaMappa ();
		}

		// debug grafica
		if(ManagerGui.PosizioneNelMenu != 2) {
			PosizioneEditor = 1;
		}
	}

	void OnGUI() {
		if(ManagerGui.PosizioneNelMenu == 2) {
			if(PosizioneEditor == 1) {
				if(GUI.Button (new Rect (Screen.width/2-100,Screen.height/2-75,200,50),"NUOVA MAPPA")) {
					PosizioneEditor = 2;

					// AZZERA VARIBIALI
					NomeMappa = "";
				} if(GUI.Button (new Rect (Screen.width/2-100,Screen.height/2+25,200,50),"CARICA MAPPA")) {
					PosizioneEditor = 3;

					// AZZERA VARIABILI
					PosizioneGuiListaCaricaPartita = 0;
					TestoRigaFile = " ";
					I_FOR_DEBUG_GUI_CaricamentoPartita = 0;
				}
			}
			// Salva il nome della nuova mappa
			if(PosizioneEditor == 2) {
				GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-200, 50, 15), "NOME MAPPA");
				NomeMappa = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-165, 200, 20), NomeMappa, 30);

				if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2, 100, 20), "Create")) {
					File.creaMappa (NomeMappa);

					PosizioneEditor = 100;
				}
				if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2+30, 100, 20),"Back")) {
					SubPosMidScreen = 1;
				}
			}
			if(PosizioneEditor == 3) {
				scrollPos = GUI.BeginScrollView(new Rect(Screen.width/2-150, Screen.height/2-300, 300, 500), scrollPos, new Rect(0, 0, 10, 500));

				MappeDelGioco = File.List_of_Item_Inside_Directory (File.MainDirectory+"/Maps"); 

				for(int I_FOR_DEBUG_GUI_CaricamentoPartita=0; I_FOR_DEBUG_GUI_CaricamentoPartita < MappeDelGioco.Count; I_FOR_DEBUG_GUI_CaricamentoPartita++) {
					// Controlla se la direcotry esiste, se la esiste non e' un file
					if (File.esisteDirectory (File.MainDirectory + "/Maps/" + MappeDelGioco [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ())) {

						// tasto da cliccare con il nome della mappa per caricare quest'ultima
						if(GUI.Button(new Rect(0, PosizioneGuiListaCaricaPartita, 300, 20), MappeDelGioco [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ())) {

							NomeMappa = MappeDelGioco [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ();

							generaMappa ();

							Debug.Log ("Stai caricando una mappa: " + MappeDelGioco [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ());
						}

						// Sposta le GUI
						PosizioneGuiListaCaricaPartita += 20;
					} else {
						Debug.Log ("Un file non Directory prensente nella cartella delle Mappe: " + MappeDelGioco [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ());
					}
				}

				I_FOR_DEBUG_GUI_CaricamentoPartita = 0;
				PosizioneGuiListaCaricaPartita = 0;

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

					// Azzeramento delle variabili
					INIZIO_X = ""; INIZIO_Y = ""; LUNGHEZZA_X = ""; LUNGHEZZA_Y = "";
				}
				if(GUI.Button (new Rect (250 ,0,100,50),"Save Map")) {
					salvaMappa (); // SALVA LA MAPPA
				}
				// FINE BARRA
				GUI.EndScrollView();

				// GUI NEL MEZZO DELLO SCHERMO
				if(SubPosMidScreen == 1) {
					// Spawna pannello per generare un nuovo blocco

					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-200, 50, 15), "Start position X");
					INIZIO_X = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-175, 50, 20), INIZIO_X, 30);
					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-150, 50, 15), "Start position Y");
					INIZIO_Y = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-135, 50, 20), INIZIO_Y, 30);

					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-100, 50, 15), "Number of cells in X");
					LUNGHEZZA_X = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-85, 50, 20), LUNGHEZZA_X, 25);
					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-60, 50, 15), "Number of cells in Y");
					LUNGHEZZA_Y = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-45, 50, 20), LUNGHEZZA_Y, 25);

					if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2, 100, 20), "Generate")) {
						//generaBlocco (int.Parse(INIZIO_X), int.Parse(INIZIO_Y), int.Parse(LUNGHEZZA_X), int.Parse(LUNGHEZZA_Y)); // genera blocco
						SpawnaMappa = true;
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

		int PosX = 0, PosY = 0;

		for(int X=0; X < LunghezzaX; X++) {
			for(int Y=0; Y < LunghezzaY; Y++) {
				if(!cellaPresenteInPos(LunghezzaX, LunghezzaY)) {
					Cella = (GameObject)Instantiate (PrefabCella, new Vector3 (InizioX+PosX, 0, InizioY+PosY), Quaternion.identity) as GameObject;

					// Cambia rotazione
					Cella.transform.Rotate (-90, 0, 0);

					// Imposta TAG
					Cella.tag = "Cella";
				}

				PosY++;
			}

			if(PosY == LunghezzaY) {
				PosY = 0;
				PosX++;
			}
		}

		SpawnaMappa = false;
	}

	public void generaMappa() {
		prendiPosCelleDaFile ();

		GeneraMappa = false;
	}

	private void prendiPosCelleDaFile() {
		if(!File.esisteFile(File.MainDirectory+"/Maps/"+NomeMappa+"/"+File.NomeFileContenenteCelle)) {
			return;
		}

		StreamReader FileMaps = new StreamReader(File.MainDirectory+"/Maps/"+NomeMappa+"/"+File.NomeFileContenenteCelle);

		for(int I=0; TestoRigaFile != null; I++) {
			TestoRigaFile = FileMaps.ReadLine ();

			string[] Explode = TestoRigaFile.Split (',');

			PosizioneCelle [I] [0] = int.Parse (Explode[0]);
			PosizioneCelle [I] [1] = int.Parse (Explode[1]);
		}

		FileMaps.Close();
	}

	public bool cellaPresenteInPos(int PosizioneX, int PosizioneY) {
		return false;
	}

	// FUNZIONI PER LA GESTIONE DEI SALVATAGGI DELLA MAPPA
	public void salvaMappa() {
		StreamWriter FileMaps = new StreamWriter(File.MainDirectory+"/Maps/"+NomeMappa+"/"+File.NomeFileContenenteCelle, true);

		CelleDellaMappa = GameObject.FindGameObjectsWithTag ("Cella");

		// IMPOSTAZIONE DEL CONTENUTO DEL FILE:
		// POSIZIONE_X,POSIZIONE_Y
		foreach (GameObject Cella in CelleDellaMappa) {
			FileMaps.WriteLine(Cella.transform.position.x.ToString()+","+Cella.transform.position.z.ToString());
		}

		FileMaps.Close();
	}
}
