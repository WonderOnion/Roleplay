using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour
{

    public Settings settings;
    public string Location;
    public string Language = null;

    private void Start()
    {
        settings = GameObject.Find("Main Camera").GetComponent<Settings>();
    }
    // Update is called once per frame
    void Update()
    {
        if (Language != GameObject.Find("Main Camera").GetComponent<Settings>().Language)
        {
            Language = GameObject.Find("Main Camera").GetComponent<Settings>().Language;
            if (GameObject.Find("Main Camera").GetComponent<Settings>().Language.Equals("en"))
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = gameObject.name;
            else
                gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = settings.Retrive_InnerText(0, "language/" + Location + gameObject.name);

        }
    }
}
