using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class IO : MonoBehaviour
{
    Settings settings;
    public bool D = true;

    private void Start()
    {
        settings = GameObject.Find("Main Camera").GetComponent<Settings>();
    }


    public int Check_Path_Exist (string path)
    {
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

    }                                     //data un percorso controlla se esiste (0), se è un file(2) o una directory(1)

    public List<string> List_of_Item_Inside_Directory (string path)
    {
        List<string> TempList = new List<string>();
        string[] Temp = Directory.GetDirectories(path);
        TempList.Add("");
        foreach (string Dir in Temp)
        {
            TempList.Add(Dir);
        }

        TempList[0] = TempList.Count.ToString();

        Temp = Directory.GetFiles(path);
        foreach(string Fil in Temp)
        {
            TempList.Add(Fil);
        }
        return TempList;
    }                   //data una path restituisce tutti gli elementi all'interno dove la prima riga contiene l'indice a cui terminano le directory ed iniziano i file ES 0:3 1:Dir 2:Dir 3:file 4:file etc

    public string[] Get_File_Information (string path)
    {
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
    }                               //restituisce tutte le informazioni inerenti a un determinato file

    public void Create_Directory(string path, string log,string err)
    {
        try
        {
            Directory.CreateDirectory(path);
            if (D) Debug.Log(log);
        }
        catch (Exception e)
        {
            Debug.LogError(err + e);
        }
    }

    public void Create_File(string path, List<string> Righe,bool Sovrascrivere)
    {

        if (Check_Path_Exist(path) == 2 && Sovrascrivere == false)  //controllo se il file esiste e se c'è l'ordine di non sovrascrivere
        {
            settings.Error_Profiler("I001", 0,"Create_File: ",2);
            return;
        }
        if (!File.Exists(path))
        {
            // Create a file to write to.
            using (StreamWriter sw = File.CreateText(path))
            {
                foreach (string Line in Righe)
                    sw.WriteLine(Line);
            }
        }
    }       //crea un file o se c'è già lo sovrascrive se la booleana è uguale a "true"

    public void Write_Into_File(string path, List<string> Righe, bool Sovrascrivere)    //
    {
        if (Check_Path_Exist(path) == 0 || Sovrascrivere == true)
        {
            Create_File(path, Righe, Sovrascrivere);
            return;
        }
        foreach (string Line in Righe)
            File.AppendAllText(path,Environment.NewLine + Line, Encoding.UTF8);
    }
}
