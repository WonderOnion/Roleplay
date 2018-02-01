using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager {

	// Variabili globali per la gestine dei file
	public string MainDirectory = "Game";

	public void generaDirectoryBase() {
		System.IO.Directory.CreateDirectory (MainDirectory);
		System.IO.Directory.CreateDirectory (MainDirectory+"/Maps");
	}

	public bool esisteDirectory(string Directory) {
		if (System.IO.Directory.Exists (Directory))
			return true;
		else
			return false;
	}

	public bool esisteFile(string NomeFile) {
		if (System.IO.File.Exists (NomeFile))
			return true;
		else
			return false;
	}

	public void cancellaDirectory(string Directory) {
		if (esisteDirectory (Directory))
			System.IO.Directory.Delete (Directory);
	}

	public void creaDirectory(string Directory) {
		if (System.IO.Directory.Exists (Directory)) {
			cancellaDirectory (Directory);
		} 

		System.IO.Directory.CreateDirectory (Directory);
	}

	public void creaFile(string NomeFile) {
		System.IO.File.Create (NomeFile);
	}

	public bool creaMappa(string NomeMappa) {
		// Genera le directory della mappa
		creaDirectory (MainDirectory+"/Maps/"+NomeMappa);
		creaDirectory (MainDirectory+"/Maps/"+NomeMappa+"/Textures");
		creaDirectory (MainDirectory+"/Maps/"+NomeMappa+"/Sounds");
		creaDirectory (MainDirectory+"/Maps/"+NomeMappa+"/Scenes");

		creaFile (MainDirectory+"/Maps/"+NomeMappa+"/config_map.txt");

		// MAPPA CREATA
		return true;
	}

	public List<string> List_of_Item_Inside_Directory (string path) {
		List<string> TempList = new List<string>();
		string[] Temp = Directory.GetDirectories(path);
		TempList.Add("");
		foreach (string Dir in Temp)
		{
			//Debug.Log (">>"+Dir);
			string[] Explode = Dir.Split('\\');
			TempList.Add("/"+Explode[1]);
		}

		//TempList[0] = TempList.Count.ToString();

		Temp = Directory.GetFiles(path);
		foreach(string Fil in Temp)
		{
			TempList.Add(Fil);
		}
		return TempList;
	}               //data una path restituisce tutti gli elementi all'interno dove la prima riga contiene l'indice a cui terminano le directory ed iniziano i file ES 0:3 1:Dir 2:Dir 3:file 4:file etc

	public string[] Get_File_Information (string path) {
		if (Check_Path_Exist(path) != 2)
		{
			Debug.LogError("La directory selezionata non è un file o non esiste.");
			return null;
		}

		string[] Temp = new string[6];
		FileInfo file = new FileInfo(path);


		Temp[0] = file.Name;                            //prendo il nome del file con estensione
		Temp[1] = file.Extension;                       //prendo l'estensione del file
		Temp[2] = file.CreationTimeUtc.ToString();      //prendo la data di creazione
		Temp[3] = file.LastWriteTimeUtc.ToString();      //prendo la data dell'ultima modifica
		Temp[4] = file.Length.ToString();               //prendo la dimensione totale
		Temp[5] = file.FullName;                        //prendo la directory totale partendo da C://

		return Temp;
	}

	public int Check_Path_Exist (string path){
		if (File.Exists(path))
		{
			return 2;
		}
		else if (Directory.Exists(path))
		{
			return 1;
		}
		else
		{
			return 0;
		}

	}    
}
