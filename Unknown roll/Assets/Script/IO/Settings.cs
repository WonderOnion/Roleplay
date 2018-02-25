using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

public class Settings : MonoBehaviour
{

    public IO InOut;
	public string ActualVersion = "0.02";
	
    public XmlDocument SettingsFile;
	public XmlDocument LanguageFile;
	
    public bool D = true;
	public string Language = "";

    void Start()
    {
        InOut = new IO();
        switch (InOut.Check_Path_Exist("Settings.xml"))
        {
            case 0:
                Debug.LogError("File impostazioni non esistente, creazione in corso");
                Create_Standard_Option();
                break;
            case 2:
                if (D) Debug.Log("File impostazioni trovato, verifica versione");
                SettingsFile = new XmlDocument();
                SettingsFile.Load("Settings.xml");
                if (!Retrive_Settings_Version().Equals(ActualVersion))
                {
                    Debug.LogError("Versione impostazioni attuale ("+ ActualVersion + ") differente da impostazioni: " + Retrive_Settings_Version());
					Debug.LogError("Funzione aggiornamento impostazioni non ancora creato, creazione nuovo file eliminando il precedente");
					Create_Standard_Option();

                }
                else if (D) Debug.Log("Versione Impostazioni genuina: V"+ Retrive_Settings_Version());
                break;
            default:
                Debug.LogError("Errore nel controllo dell'esistenza delle impostazioni: Switch exception");
                return;

        }
		switch (InOut.Check_Path_Exist("Language.xml"))
		{
			case 0:
                Debug.LogError("Language file doesn't exist. you must download it");
                return;
			case 2:
				if(D) Debug.Log("Language file found");
				LanguageFile = new XmlDocument();
				LanguageFile.Load("Language.xml");
				break;
			default:
				Debug.LogError("Error when retriving language file information: Switch exception");
				return;
		}
		Language = Retrive_InnerText(1,"Settings/Base/Language");
    }

    string Retrive_Settings_Version()
    {
        XmlElement elt = SettingsFile.SelectSingleNode("Settings") as XmlElement;
        if (elt != null)
            return  elt.GetAttribute("Ver") as string;
        return null;
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
				return null;
		}
        if (child != null)
            return child.InnerText;
        return null;
	}

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
    }
}
