using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class EditorManager : MonoBehaviour {
	
	public Settings settings;
	
	
	
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
	public GameObject ContainerCellaPrefab;
	private GameObject ContainerCella;

	public Camera cameraobj;
	public bool RaycastMouse = false;
	public GameObject OggettoRaycast;

	// Variaibli GUI
	private Vector2 scrollPos = Vector2.zero;
	// variabili per generare un nuovo blocco
	private string LUNGHEZZA_X = "", LUNGHEZZA_Y = "", NOME_NEW_SCENE = "", NOME_NEW_SCENE_TEMP = "";

	// Gestione mappa
	public bool SpawnaMappa = false;
	public bool GeneraMappa =  false;
	private string NomeMappa = "";
	public List<string> InformazioniCelle;
	private List<string> SceneDellaMappa;
	public string ScenaCorrente = "";

	// Caricare partita
	List<string> MappeDelGioco;
	private int PosizioneGuiListaCaricaPartita = 0;
	private string TestoRigaFile = " ";
	private int I_FOR_DEBUG_GUI_CaricamentoPartita = 0;
	// Caricamento Texture
	List<string> TexturePlayer;
	private int PosizioneGuiTextureCaricata = 0;
	private int I_FOR_DEBUG_GUI_CaricamentoTexture = 0;

	public Texture TextureSelezionata;
	public GameObject OggettoSelezionato;

	// Script esterni
	MenuGUI ManagerGui;

	// FILE MANAGER
	public FileManager File = new FileManager();
	private GameObject[] CelleDellaMappa;

	public GameObject MainManager;

	// GUI 
	private Rect window_ContScene = new Rect(Screen.width-160, 100, 200, 200), window_ContTexture = new Rect(10, 100, 200, 200);
	private Vector2 scrollTutteLeScene = Vector2.zero, scrollTutteLeTexture = Vector2.zero;

	void Start () {
		ManagerGui = MainManager.GetComponent<MenuGUI> ();
		settings = GameObject.Find("Settings").GetComponent<Settings>();
		
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
			generaBlocco (int.Parse(LUNGHEZZA_X), int.Parse(LUNGHEZZA_Y));
		}

		// Genera tutta la mappa attraverso il file della cartella
		if(GeneraMappa) {
			caricaBlocco ();
		}

		// debug grafica
		if(ManagerGui.PosizioneNelMenu != 2) {
			PosizioneEditor = 1;
		}

		// Usa il pennello
		if(TextureSelezionata != null) {
			if(Input.GetMouseButton(0)) {
				inserisciTextureObj (OggettoRaycast, TextureSelezionata);
			}
		}
	}

	void OnGUI() {
		if(ManagerGui.PosizioneNelMenu == 2) {
			if(PosizioneEditor == 1) {
				if(GUI.Button (new Rect (Screen.width/2-100,Screen.height/2-75,200,50),settings.Retrive_InnerText(0,"language/"+ settings.Language + "/Menu/Play/New_Map"))) {
					PosizioneEditor = 2;

					// AZZERA VARIBIALI
					NomeMappa = "";
					NOME_NEW_SCENE = "";
					NOME_NEW_SCENE_TEMP = "";
					SceneDellaMappa =  new List<string>();
					PosizioneGuiListaCaricaPartita = 0;
					ScenaCorrente = "";
				} if(GUI.Button (new Rect (Screen.width/2-100,Screen.height/2+25,200,50),settings.Retrive_InnerText(0,"language/"+ settings.Language + "/Menu/Play/Load_Map"))) {
					PosizioneEditor = 3;

					// AZZERA VARIABILI
					PosizioneGuiListaCaricaPartita = 0;
					TestoRigaFile = " ";
					I_FOR_DEBUG_GUI_CaricamentoPartita = 0;
					InformazioniCelle = new List<string>();
					NOME_NEW_SCENE = "";
					NOME_NEW_SCENE_TEMP = "";
					SceneDellaMappa =  new List<string>();
					ScenaCorrente = "";
				}
			}
			// Salva il nome della nuova mappa
			if(PosizioneEditor == 2) {
				GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-200, 50, 15), settings.Retrive_InnerText(0,"language/"+ settings.Language + "/Menu/Play/Map_Name"));
				NomeMappa = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-165, 200, 20), NomeMappa, 30);

				if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2, 100, 20), settings.Retrive_InnerText(0,"language/"+ settings.Language + "/Menu/Play/Create"))) {
					File.creaMappa (NomeMappa);

					PosizioneEditor = 100;
				}
				if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2+30, 100, 20),settings.Retrive_InnerText(0,"language/"+ settings.Language + "/Menu/Main/Back"))) {
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

							//caricaBlocco ();

							PosizioneEditor = 100;

							Debug.Log ("Stai caricando una mappa: " + MappeDelGioco [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ());

							PosizioneGuiListaCaricaPartita = 0;
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
				if(!RaycastMouse) {
					if(GUI.Button (new Rect (0,0,150,50),"ACTIVE Mouse R.")) {
						RaycastMouse = true;
					}
				} else {
					if(GUI.Button (new Rect (0,0,150,50),"BLOCK Mouse R.")) {
						RaycastMouse = false;
					}
				}
				if(GUI.Button (new Rect (150,0,100,50),"New Scene")) {
					SubPosMidScreen = 1;

					// Azzeramento delle variabili
					LUNGHEZZA_X = ""; LUNGHEZZA_Y = "";
				}
				if(GUI.Button (new Rect (250 ,0,100,50),"Save Map")) {
					salvaMappa (); // SALVA LA MAPPA
				}
				if(GUI.Button (new Rect (350 ,0,100,50),"Kill Texture")) {
					TextureSelezionata = null;
				}
				if(GUI.Button (new Rect (450 ,0,100,50),"Kill Obj")) {
					TextureSelezionata = null;
				}
				// FINE BARRA
				GUI.EndScrollView();

				// GUI NEL MEZZO DELLO SCHERMO
				if(SubPosMidScreen == 1) {
					// Spawna pannello per generare un nuovo blocco

					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-100, 50, 15), "Number of cells in X");
					LUNGHEZZA_X = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-85, 50, 20), LUNGHEZZA_X, 25);
					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-60, 50, 15), "Number of cells in Y");
					LUNGHEZZA_Y = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-45, 50, 20), LUNGHEZZA_Y, 25);

					GUI.Label (new Rect(Screen.width/2-50, Screen.height/2-150, 50, 20), "Nome della scena");
					NOME_NEW_SCENE_TEMP = GUI.TextField(new Rect(Screen.width/2-50, Screen.height/2-130, 200, 20), NOME_NEW_SCENE_TEMP, 80);

					if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2, 100, 20), settings.Retrive_InnerText(0,"language/"+ settings.Language + "/Menu/Play/Create"))) {
						generaBlocco (int.Parse(LUNGHEZZA_X), int.Parse(LUNGHEZZA_Y)); // genera blocco
						SpawnaMappa = true;
						SubPosMidScreen = 0;

						NOME_NEW_SCENE = NOME_NEW_SCENE_TEMP;
						NOME_NEW_SCENE_TEMP = "";
					}
					if(GUI.Button (new Rect (Screen.width/2-50, Screen.height/2+30, 100, 20),settings.Retrive_InnerText(0,"language/"+ settings.Language + "/Menu/Main/Back"))) {
						SubPosMidScreen = 0;
					}
				}
				// FINE GUI NEL MEZZO DELLO SCHERMO

				// FINESTRA CON TUTTE LE SCENA DELLA MAPPA
				window_ContScene = GUI.Window(0, window_ContScene, finestraConLeScene, "Game scenes");
			}
		}
			
	}

	/* FUNZIONI PER LE GUI */
	void finestraConLeScene(int windowsID) {
		// Elementi
		if(GUI.Button (new Rect(0, 20, 200, 20), "Refresh scenes")) {
			refreshScenes ();
			//Debug.Log ("Numero di scene: " + SceneDellaMappa.Count);
		}

		scrollTutteLeScene = GUI.BeginScrollView(new Rect(0, 40, 200, 200), scrollTutteLeScene, new Rect(0, 0, 200, ((SceneDellaMappa.Count-1) * 20)));

		// STAMPA TUTTE LE SCENE DELLA MAPPA
		for(int I_FOR_DEBUG_GUI_CaricamentoPartita=0; I_FOR_DEBUG_GUI_CaricamentoPartita < SceneDellaMappa.Count; I_FOR_DEBUG_GUI_CaricamentoPartita++) {
			// Controlla se la direcotry esiste, se la esiste non e' un file
			if (File.esisteFile (SceneDellaMappa [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ())) {
				// tasto da cliccare con il nome della mappa per caricare quest'ultima
				if(GUI.Button(new Rect(0, 0 + PosizioneGuiListaCaricaPartita, 190, 20), SceneDellaMappa [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ())) {
					ScenaCorrente = SceneDellaMappa [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ();

					//caricaBlocco ();

					Debug.Log ("Cliccato: " + SceneDellaMappa [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ());

					TestoRigaFile = " ";
					GeneraMappa = true;
				}

				// Sposta le GUI
				PosizioneGuiListaCaricaPartita += 20;
			} else {
				//Debug.Log ("SCENA: " + SceneDellaMappa [I_FOR_DEBUG_GUI_CaricamentoPartita].ToString ());
			}
		}

		I_FOR_DEBUG_GUI_CaricamentoPartita = 0;
		PosizioneGuiListaCaricaPartita = 0;

		GUI.EndScrollView();

		// Scorrevole
		DoMyWindow();
	}


	void DoMyWindow() {
		GUI.DragWindow (new Rect (0, 0, 10000, 20));
	}
	/* FINE FUNZIONI PER LE GUI */

	private void raycast() {
		if(RaycastMouse) {

			RaycastHit hit;
			Ray ray = cameraobj.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast (ray, out hit)) {
				OggettoRaycast = hit.transform.gameObject;
			} else {
				OggettoRaycast = null;
			}
		}
	}

	// funzione che genera un blocco
	public void generaBlocco(int LunghezzaX, int LunghezzaY) {
		GameObject Cella = null;

		int PosX = 0, PosY = 0;

		// controlla se esiste gia una mappa
		spawnaContainerMap ();

		for(int X=0; X < LunghezzaX; X++) {
			for(int Y=0; Y < LunghezzaY; Y++) {
				if(!cellaPresenteInPos(LunghezzaX, LunghezzaY)) {
					Cella = spawnaCella(PosX, PosY);
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

	// Funzione che carica un blocco attraverso la lista presa dal file di salvataggio del blocco
	public void caricaBlocco() {
		prendiPosCelleDaFile ();

		spawnaContainerMap ();

		for(int I=0; I < InformazioniCelle.Count; I++) {
			string[] Explode = InformazioniCelle [I].Split (',');

			GameObject Cella = spawnaCella (int.Parse(Explode[0]), int.Parse(Explode[1]));
		}

		GeneraMappa = false;
	}

	private void prendiPosCelleDaFile() {
		if(!File.esisteFile(ScenaCorrente)) {
			return;
		}

		StreamReader FileMaps = new StreamReader(ScenaCorrente);

		InformazioniCelle = new List<string> ();

		for(int I=0; TestoRigaFile != null; I++) {
			InformazioniCelle.Add(FileMaps.ReadLine ());
			TestoRigaFile = FileMaps.ReadLine ();
		}

		//Debug.Log ("Grandezza: "+InformazioniCelle.Count);

		FileMaps.Close();
	}

	public bool cellaPresenteInPos(int PosizioneX, int PosizioneY) {
		return false;
	}
		
	public void salvaMappa() {
		//System.IO.File.Create (File.MainDirectory+"/Maps/"+NomeMappa+"/Scenes/"+NOME_NEW_SCENE+".txt");

		StreamWriter FileMaps = new StreamWriter(File.MainDirectory+"/Maps/"+NomeMappa+"/Scenes/"+NOME_NEW_SCENE+".txt", true);
		//StreamWriter FileMaps = new StreamWriter(File.MainDirectory+"/Maps/"+NomeMappa+"/wer.txt", true);

		CelleDellaMappa = GameObject.FindGameObjectsWithTag ("Cella");

		// IMPOSTAZIONE DEL CONTENUTO DEL FILE:
		// POSIZIONE_X,POSIZIONE_Y
		foreach (GameObject Cella in CelleDellaMappa) {
			FileMaps.WriteLine(Cella.transform.position.x.ToString()+","+Cella.transform.position.z.ToString());
		}

		FileMaps.Close();
	}

	public GameObject spawnaCella(int PosizioneX, int PosizioneY) {
		GameObject Cella = (GameObject)Instantiate (PrefabCella, new Vector3 (PosizioneX, 0, PosizioneY), Quaternion.identity) as GameObject;

		// Cambia rotazione
		Cella.transform.Rotate (-90, 0, 0);

		// Cambia nome
		Cella.name = "cella_"+PosizioneX.ToString()+"_"+PosizioneY.ToString();

		// Imposta TAG
		Cella.tag = "Cella";

		// Impostazione parentela
		Cella.transform.parent = ContainerCella.transform;

		return Cella;
	}

	// Spawna l'oggetto che contiene la mappa (le varie cella)
	private void spawnaContainerMap() {
		// controlla se esiste gia un cantainer
		if(ContainerCella != null) {

			// Se esiste una mappa deve essere cancellata per far spawnare quella nuova
			DestroyObject (ContainerCella);

			ContainerCella = null;
		}

		ContainerCella = (GameObject)Instantiate (ContainerCellaPrefab, new Vector3 (0, 0,0), Quaternion.identity) as GameObject;
	}

	// Funzione che controlla tutte le scene del gioco
	private void refreshScenes() {
		SceneDellaMappa = new List<string> ();

		SceneDellaMappa = File.List_of_Item_Inside_Directory (File.MainDirectory+"/Maps/"+NomeMappa+"/Scenes/"); 
	}

	private void refreshTexture() {
		TexturePlayer = new List<string> ();

		TexturePlayer = File.List_of_Item_Inside_Directory (File.MainDirectory+"/Maps/"+NomeMappa+"/Textures/"); 
	}

	public void inserisciTextureObj(GameObject Oggetto, Texture Textur) {
		Oggetto.GetComponent<MeshRenderer> ().materials [0].SetTexture ("_MainTex", Textur);
	}

	public void posizionaOggettoSullaMappa(GameObject Oggetto, Vector3 VettoreConPosizione) {
		Oggetto.transform.position = VettoreConPosizione;
	}
}
