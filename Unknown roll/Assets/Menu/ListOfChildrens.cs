using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListOfChildrens : MonoBehaviour
{
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
