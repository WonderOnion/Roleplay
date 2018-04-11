using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class Settings : MonoBehaviour
{
    /*
     

                 __      _   _   _                           
                / _\ ___| |_| |_(_)_ __   __ _ ___   ___ ___ 
                \ \ / _ \ __| __| | '_ \ / _` / __| / __/ __|
                _\ \  __/ |_| |_| | | | | (_| \__ \| (__\__ \
                \__/\___|\__|\__|_|_| |_|\__, |___(_)___|___/
                                         |___/               
 
        Gestisce:
            -Impostazioni
            -Lingua
        Tali sezioni sono divise da un commento con relativa spiegazione

        Questo file Ha la funzione di Caricamento e di gestione di tutte le impostazioni e della lingua che l'utente
        utilizzerà all'interno del gioco.

        
    
     */
    private IO InOut = new IO();                            //essenziale al fine delle operazioni Input/Output che compongono gran parte del file
    public string ActualVersion = "0.02";                   //Per controllare e nel caso aggiornare il file di impostazioni
    public bool D = true;                                   //Variabile di Debug

    public XmlDocument SettingsFile = new XmlDocument();    //File XML di impostazioni, assegnato all'avvio del programma
    public XmlDocument LanguageFile = new XmlDocument();    //FIle XML della lingua, assegnato all'avvio del programma e ogni qualvolta si cambi legalmente o illegalmente(In maniere non consone quali modifica di Setting.xml o programmi esterni) la lingua

   
    public List<string> LanguageList = new List<string>();  //lista di tutte le lingue conosciute al momento
    public string Language = "";                            //Lingua attuale su cui si basano tutti i bottoni
    private string OldLanguage;                             //variabile per capire se vi sono stati cambiamente della lingua per poter accedervi dinamicamente

    void Start()
    {
        //controllo esistenza file di impostazioni
        switch (InOut.Check_Path_Exist("Settings.xml"))
        {
            case 0:
                //nel caso non esista il file Settings.xml lo si crea poichè salvato via codice
                Debug.LogError("File impostazioni non esistente, creazione in corso");
                Create_Standard_Option();
                SettingsFile.Load("Settings.xml");
                break;
            case 2:
                //Nel caso esista si controlla la versione e nel caso siano differenti si procede a eliminare il precedente e crearne uno aggiornato
                if (D) Debug.Log("File impostazioni trovato, verifica versione");
                SettingsFile.Load("Settings.xml");
                if (!Retrive_Settings_Version().Equals(ActualVersion))
                {
                    Debug.LogError("Versione impostazioni attuale (" + ActualVersion + ") differente da impostazioni: " + Retrive_Settings_Version());
                    Debug.LogError("Funzione aggiornamento impostazioni non ancora creato, creazione nuovo file eliminando il precedente");
                    Create_Standard_Option();

                }
                else if (D) Debug.Log("Versione Impostazioni genuina: V" + Retrive_Settings_Version());
                break;
            default:
                //errori irraggiungibili
                Debug.LogError("Errore nel controllo dell'esistenza delle impostazioni: Switch exception");
                return;

        }
        //Estrapolo la lingua desiderata dal file di impostazioni
        Language = Retrive_InnerText(1, "Settings/Base/Language");
        OldLanguage = Language;

        //controllo delle lingue disponibili
        if (InOut.Check_Path_Exist("Language") == 0)
            InOut.Create_Directory("Language", "Language Folder didn't exist, creating", "Error: Can not create Language Folder");

        RefreshLanguage();
        
    }                           //caricamento, nel caso creazione del file impostazioni e lingua


    private void Update()
    {
        //controllo in tempo reale se viene cambiata in modo dinamico la lingua
        if(!Language.Equals(OldLanguage))
        {
            lock(LanguageFile)
            {
                RefreshLanguage();
                OldLanguage = Language;
            }
        }
    }                   //controllo cambi di lingua


   
    string Retrive_Settings_Version()
    {
        XmlElement elt = SettingsFile.SelectSingleNode("Settings") as XmlElement;
        if (elt != null)
            return  elt.GetAttribute("Ver") as string;
        return null;
    }       //ritorna la versione attuale delle impostazioni

    void Create_Standard_Option()
    {
        XmlWriter TempFile = XmlWriter.Create("Settings.xml");

        TempFile.WriteStartDocument();
        TempFile.WriteStartElement("Settings");
        TempFile.WriteAttributeString("Ver", ActualVersion);

        TempFile.WriteStartElement("Base");
        TempFile.WriteStartElement("Language");
        TempFile.WriteString("en");
        TempFile.WriteEndElement();
        TempFile.WriteEndElement();


        TempFile.WriteStartElement("Network");
        TempFile.WriteStartElement("LastIP");
        TempFile.WriteString("127.0.0.1");
        TempFile.WriteEndElement();
        TempFile.WriteEndElement();


        TempFile.WriteStartElement("Graphic");
        TempFile.WriteStartElement("Resolution");
        TempFile.WriteString("1920x1080");
        TempFile.WriteEndElement();
        TempFile.WriteStartElement("windowed");
        TempFile.WriteString("true");
        TempFile.WriteEndElement();
        TempFile.WriteStartElement("Quality");
        TempFile.WriteString("Awesome");
        TempFile.WriteEndElement();
        TempFile.WriteEndElement();

        TempFile.WriteStartElement("Audio");
        TempFile.WriteStartElement("Master");
        TempFile.WriteString("100");
        TempFile.WriteEndElement();
        TempFile.WriteEndElement();

        TempFile.WriteEndDocument();
        TempFile.Close();

    }           //Genera un file Settings.xml con le impostazioni desiderate
    /*
                        __                                          
                          / /  __ _ _ __   __ _ _ _  __ _  __ _  ___ 
                         / /  / _` | '_ \ / _` | | | |/ _` |/ _` |/ _ \
                        / /__| (_| | | | | (_| | |_| | (_| | (_| |  __/
                        \____/\__,_|_| |_|\__, |\__,_|\__,_|\__, |\___|
                                          |___/             |___/      

        I nomi dei gameobject che fanno riferimento al gioco essenziale sono in Inglese
        Se richiesta la lingua inglese il gioco prende il nome dei bottoni e lo inserisce come testo
        Perciò tutti i comandi essenziali sono disponibili anche senza il file XML della lingua.
        
        
        RefreshLanguage():  Ha la funzione di controllare se la nuova lingua inserita sia esistente a livello fisico (xml) e nel caso aggiornare

        Retrive_InnerText():  Ha la funzione di prendere l'innerText dal file desiderato, 0 se si intente reperire un testo dal file di lingue e
                            1 se si intende prelevarlo dal file di Settings
        
        ERRORE 1:Manca XML lingua
            possibili cause:    Usa software esterni di cheating e si cambia la lingua
                                Classico smanettone che si cambia la lingua in modo errato sul file setting
                                non reperibilità del file
            Il gioco inserirà in automatico la lingua inglese, che, come anticipatamente spiegato ha i comandi essenziali.  (RefreshLanguage)
            Questo controllo viene eseguito ogni frame per evitare qualsivoglia problema 


        ERRORE 2:Manca Tag o Tag vuoto nell'xml
            possibili cause:    Un utente cerca di farsi la sua versione della lingua e lascia incompleti dei campi
                                Dimenticanze di sviluppo

        Il gioco inserirà NoText (RetriveInnerText)
*/


    private void RefreshLanguage()
    {

        //lista di tutte le lingue disponibili nella cartella e controllo genuinita file (scarta tutto ciò che non sia .XML)
        LanguageList = InOut.List_of_Item_Inside_Directory("Language");
        for (int I = 0; I < LanguageList.Count; I++)
        {
            if (InOut.Check_Path_Exist(LanguageList[I]) == 0 || InOut.Check_Path_Exist(LanguageList[I]) == 1)
            {
                LanguageList.RemoveAt(I);
                I--;
            }
            else
            {
                string[] FileInformation = InOut.Get_File_Information(LanguageList[I]);
                if (!FileInformation[1].Equals(".xml"))
                {
                    LanguageList.Remove(LanguageList[I]);
                    I--;
                }
            }
        }

        switch (InOut.Check_Path_Exist("Language/" + Language + ".xml"))
        {
            case 0:
                Debug.LogError("Language file doesn't exist. you must download it");
                if (!Language.Equals("en"))
                {
                    XmlNode title = SettingsFile.SelectSingleNode("Settings/Base/Language");
                    if (title != null)
                    {
                        title.InnerText = "en";
                        SettingsFile.Save(@"Settings.xml");
                    }
                    Debug.LogWarning("Language: " + Language + " didn't exist anymore, return to english");
                    Language = "en";
                }
                return;
            case 2:
                if (D) Debug.Log("Language file found");
                //Aggiornamento file di impostazioni:
                SettingsFile.GetElementsByTagName("Language")[0].InnerText = Language;
                SettingsFile.Save(@"Settings.xml");
                LanguageFile.Load("Language/" + Language + ".xml");
                break;
            default:
                Debug.LogError("Error when retriving language file information: Switch exception");
                return;
        }
    }

    public string Retrive_InnerText(int file,string path) //file: 0 = language 1= setting
	{
		XmlNode child;
		switch (file)
		{	
			case 0:
				child = LanguageFile.SelectSingleNode(path);
				break;
			case 1:
				child = SettingsFile.SelectSingleNode(path);
				break;
			default:
				Debug.LogError("Error loading path or file: " + path);
				return "NoDir";
		}
        if (child != null)
            return child.InnerText;
        return "NoText";
	}

}
