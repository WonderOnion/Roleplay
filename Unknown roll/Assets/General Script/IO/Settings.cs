using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

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
        ConsoleText = GameObject.FindWithTag("ConsoleText").GetComponent<TextMeshProUGUI>();       //Assegno la textArea della console alla variabile
        GameObject.Find("Canvas").GetComponent<MenuHandler>().AddMenuItem(GameObject.Find("Canvas/Console"));
        GameObject.Find("Canvas/Console").SetActive(false);
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
        Il File di lingua tuttavia gestisce anche una descrizione dei codici degli errori (quella visibile nel prompt dei comandi in gioco)
        
        
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



        //ora che ho la lista di tutte le lingue disponibili controllo che esista la lingua presente all'interno del file di impostazioni
        switch (InOut.Check_Path_Exist("Language/" + Language + ".xml"))
        {
            case 0:
                Console_Write("G001 From: RefreshLanguage, old Language: " + Language);
                if (!Language.Equals("en"))
                {
                    //salvo inglese come nuova lingua all'interno delle impostazioni
                    XmlNode title = SettingsFile.SelectSingleNode("Settings/Base/Language");
                    if (title != null)
                    {
                        title.InnerText = "en";
                        SettingsFile.Save(@"Settings.xml");
                    }

                    Language = "en";
                }
                return;
            case 2:
                //file della lingua trovato
                if (D) Debug.Log("Language file found");
                //Aggiornamento file di impostazioni:
                SettingsFile.GetElementsByTagName("Language")[0].InnerText = Language;
                SettingsFile.Save(@"Settings.xml");
                LanguageFile.Load("Language/" + Language + ".xml");
                break;
            default:
                //errore causato dallla modifica della funzione di Check_Path_Exist
                Error_Profiler("G001", 0, "Error when retriving language file information: Switch exception", 4);
                if (!Language.Equals("en"))
                {
                    //salvo inglese come nuova lingua all'interno delle impostazioni
                    XmlNode title = SettingsFile.SelectSingleNode("Settings/Base/Language");
                    if (title != null)
                    {
                        title.InnerText = "en";
                        SettingsFile.Save(@"Settings.xml");
                    }

                    Language = "en";
                }
                return;
        }
    }

    public void ChangeLanguage(string NextLanguage)
    {
        if (InOut.Check_Path_Exist("Language/" + NextLanguage + ".xml") != 2)
        {
            Error_Profiler("G001", 0, "ChangeLanguage => " + NextLanguage, 4);
            return;
        }
        XmlNode title = SettingsFile.SelectSingleNode("Settings/Base/Language");
        if (title != null)
        {
            title.InnerText = NextLanguage;
            SettingsFile.Save(@"Settings.xml");
        }
        lock(LanguageFile)
        {
            LanguageFile.Load("Language/" + NextLanguage + ".xml");
        }
        Language = NextLanguage;
    }
    public string Retrive_InnerText(int file,string path) //file: 0 = language 1= settings Utilizzato per trovare l'innerText di un tag XML <tag>InnerText</tag>
	{
        //eseguo un controllo su quale file voglia far riferimento (presenta la struttura con lo switch e un int per futuri upgrade)
		XmlNode child;
		switch (file)
		{

            case -1:
                //Richiesta la lettura di un XML di path data in input + location divise da una virgola (Languages/en.xml,langauge/General/Name)
                string[] TempPaths = path.Split(',');
                if (InOut.Check_Path_Exist(TempPaths[0]) == 2)
                {
                    XmlDocument TempFile = new XmlDocument();
                    TempFile.Load(TempPaths[0]);
                    child = TempFile.SelectSingleNode(TempPaths[1]);
                    break;
                }
                 Error_Profiler("D001", 0, "RetriveInnerText => wrong path: " + path,2);
                return "NoFile";
			case 0:
                //risultato essere richiesto il file di lingua attualemnte in uso
				child = LanguageFile.SelectSingleNode(path);
				break;
			case 1:
                //risultato essere richiesto il file di impostazioni
				child = SettingsFile.SelectSingleNode(path);
				break;
			default:
                //nel caso venga passato un numero non consono perciò fa riferimento a un file non esistente, verrà notificato l'errore solo nella console (giallo)
                //Error_Profiler("I002", 0, "From: Retrive_Innertext", 1); Se non esiste il file di testo generano un ciclo infinito, ignoro il problema altrimenti ci sarebbe troppo codice per un problema che si può presentare solo una votla a esecuzione
                return "NoDir";
		}
        if (child != null)
        return child.InnerText;
        // nel caso il codice arrivi qui sinifica che non è stato trovato alcun testo ma il file è esistente, errore molto comune durante il developing, errore bianco
        //Error_Profiler("M003", 0, "The path: " + path + " didn't exist",0); Se non esiste il file di testo generano un ciclo infinito, ignoro il problema altrimenti ci sarebbe troppo codice per un problema che si può presentare solo una votla a esecuzione
        return "NoText";
	}


    /*

                       ___                      _      
                      / __\___  _ __  ___  ___ | | ___ 
                     / /  / _ \| '_ \/ __|/ _ \| |/ _ \
                    / /__| (_) | | | \__ \ (_) | |  __/
                    \____/\___/|_| |_|___/\___/|_|\___|



            Da qui in poi viene gestita tutta la console e gli errori

        */
    public TextMeshProUGUI ConsoleText;                          //Variabile per riferirsi alla TextArea presente nella console

    public void Error_Profiler(string ErrorCode,float ErrorFileVersion, string MoreDeatils,int Level)  //Errorcode contiene il codice di errore, errorfileversion indica la versione del file di errore a cui fa riferimento il codice quando è stato scritto (prevenzione futuri errori), Level indica il livello di errore, 0 = ignorabile
    {
        //controllo se la versione presente nel file XML sia un float
        float SettingsVersion;
        if (float.TryParse(Retrive_InnerText(0, "language/General/ErrorVersion"), out SettingsVersion))
        {
            //suddivido l'errore in base alla categoria
            switch (ErrorCode.Substring(0,1))
            {
                case "D":
                    ErrorCode = "Debug/" + ErrorCode;           //utilizzato nelle fasi di debug per errori generici su controlli che andranno eliminati in fasi stabili
                    break;
                case "G":
                    ErrorCode = "General/" + ErrorCode;
                    break;
                case "M":
                    ErrorCode = "Menu/" + ErrorCode;
                    break;
                case "S":
                    ErrorCode = "Settings/" + ErrorCode;
                    break;
                case "I":
                    ErrorCode = "IO/" + ErrorCode;
                    break;
            }
            //In base alla gravità colorerò la scrittoa
            string ErrorColored;
            switch (Level)
            {
                // 0 & 3 == white
                // 1 & 4 == yellow
                // 2 & 5 == red
                case 1:
                case 4:
                    ErrorColored = "<color=\"yellow\"> " + ErrorCode;
                    break;
                case 2:
                case 5:
                    ErrorColored = "<color=\"red\"> " + ErrorCode;
                    break;

                default:
                    //nel caso non venga trovato il colore si imposta bianco
                ErrorColored = "<color=\"white\"> " + ErrorCode;
                    break;

            }
            // controllo che le versioni dei file siano identiche altrimenti genero un errore
            if (ErrorFileVersion == SettingsVersion)
            {
                //Le versioni coincidono, scrivo l'errore della debug console e console in game
                Debug.LogError(ErrorCode + " >> " + ErrorFileVersion + " >> " + Retrive_InnerText(0,"language/Error/"+ErrorCode) + " Details: " + MoreDeatils);
                ConsoleText.text = ConsoleText.text + "\n " +  ErrorColored + " >> " + ErrorFileVersion + " >> " + Retrive_InnerText(0,"language / Error / "+ErrorCode) + " Details: " + MoreDeatils;
                //aggiungo l'errore a ErrorLog.txt
                List<string> TempError = new List<string>();
                TempError.Add(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + " >> " +ErrorColored + " >> " + ErrorFileVersion + " >> " + Retrive_InnerText(0, "language/Error/" + ErrorCode) + " Details: " + MoreDeatils);

                //nel caso l'errore sia di gravità superiore a 2 viene generato un PopUp che sarà visualizzato dall'utente obbligatoriamente.
                if (Level > 2)
                {
                    GameObject.Find("Canvas").GetComponent<MenuHandler>().MenuElements.Where(obj => obj.name.Equals("ErrorPopup")).SingleOrDefault().SetActive(true);
                    GameObject.FindWithTag("ErrorText").GetComponent<TextMeshProUGUI>().text += "\n" + string.Format("{0:HH:mm:ss tt}", DateTime.Now) + " >> " + ErrorColored + " >> " + ErrorFileVersion + " >> " + Retrive_InnerText(0, "language/Error/" + ErrorCode) + " Details: " + MoreDeatils;
                }
                InOut.Write_Into_File("ErrorLog.txt", TempError, false);
            }
            else
            {
                //I le versioni non coincidono, si invia l'errore senza la descrizione presente nel documento.
                Debug.LogError("Different ErrorVersion, Error called: " + ErrorCode + " Version: " + ErrorFileVersion + " Detail: " + MoreDeatils);
                ConsoleText.text = "Different ErrorVersion, Error called: " + ErrorColored + " Version: " + ErrorFileVersion + " Detail: " + MoreDeatils;
                // aggiunta a ErrorLog.txt
                List<string> TempError = new List<string>();
                TempError.Add(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + " >> " + "Different ErrorVersion, Error called: " + ErrorCode + " Version: " + ErrorFileVersion + " Detail: " + MoreDeatils);
                InOut.Write_Into_File("ErrorLog.txt", TempError, false);
            }
        }
        else
        {
            //NOn è stato possibile leggere o il valore non è possibile convertirlo in FLoat, scrivo le informazioni sull'errore senza aggiungere i dettagli presenti nel file di lingua
            Debug.LogError("ErrorVersion can not be loaded, Error called: " + ErrorCode + " Version: " + ErrorFileVersion + " Detail: "  + MoreDeatils);
            ConsoleText.text = "ErrorVersion can not be loaded, Error called: " + ErrorCode + " Version: " + ErrorFileVersion + " Detail: " + MoreDeatils;
            // aggiunta a ErrorLog.txt
            List<string> TempError = new List<string>();
            TempError.Add(string.Format("{0:HH:mm:ss tt}", DateTime.Now) + " >> " + "ErrorVersion can not be loaded, Error called: " + ErrorCode + " Version: " + ErrorFileVersion + " Detail: " + MoreDeatils);
            InOut.Write_Into_File("ErrorLog.txt", TempError, false);
        }
        
    }
    
    public void Console_Write(string Text)
    {
        ConsoleText.text = ConsoleText.text + "\n<color=\"white\">" + Text;
    }
}
