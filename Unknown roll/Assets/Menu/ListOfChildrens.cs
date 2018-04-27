using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfChildrens : MonoBehaviour
{
    /*
     * 
                           __ _     _     ___  __   ___ _     _ _     _                          
                          / /(_)___| |_  /___\/ _| / __\ |__ (_) | __| |_ __ ___ _ __    ___ ___ 
                         / / | / __| __|//  // |_ / /  | '_ \| | |/ _` | '__/ _ \ '_ \  / __/ __|
                        / /__| \__ \ |_/ \_//|  _/ /___| | | | | | (_| | | |  __/ | | || (__\__ \
                        \____/_|___/\__\___/ |_| \____/|_| |_|_|_|\__,_|_|  \___|_| |_(_)___|___/
                                                                         
        
        Questo file ha la funzione di modificare in tempo reale la dimensione di scrollrect con all'interno svariati figli.
        viene utilizzato nel menu principale => lobby per avere una lista di dimensione X senza creare problemi di qualsivoglia definiti dallo schermo dell'utilizzatore.
     */
    int Previuos = 0;
    public int ChildOffset;


    private void Update()
    {
        if (Previuos != gameObject.transform.childCount)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(gameObject.GetComponent<RectTransform>().sizeDelta.x, gameObject.transform.childCount * ChildOffset);
        }
    }
}
