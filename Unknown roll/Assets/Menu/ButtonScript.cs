using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonScript : MonoBehaviour {

    public Settings settings;
    public string Location;

    private void Start()
    {
        settings = GameObject.Find("Main Camera").GetComponent<Settings>();
    }
    // Update is called once per frame
    void Update ()
    {
        gameObject.GetComponentInChildren<UnityEngine.UI.Text>().text = settings.Retrive_InnerText(0, "language/" + settings.Language + Location + gameObject.name);
    }
}
