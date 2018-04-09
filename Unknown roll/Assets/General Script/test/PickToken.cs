using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickToken : MonoBehaviour
{

    public bool D = true;
    public bool Picked = false;

    public LayerMask mask;
    public float up;
    private Vector3 startingPos = Vector3.zero;
    private Vector3 lastPosHit = Vector3.zero;
    RaycastHit hit;

    private void OnMouseDrag()
    {
        if (!Picked)
        {
            //transform.Translate(0, 0.3f, 0);
            Picked = true;
            startingPos = transform.position;
        }

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(camRay, out hit, Mathf.Infinity, mask))
        {
            transform.position = hit.point + hit.normal * up;
            lastPosHit = hit.point;
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.red);
        }
    }

    private void Update()
    {
        if (!Input.GetMouseButton(0) && Picked)
        {
            transform.position = new Vector3(transform.position.x, lastPosHit.y + transform.localScale.y / 2, transform.position.z);
            //transform.Translate(0, -0.3f, 0);
            Picked = false;
        }
    }
}