using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camara : MonoBehaviour
{
    private Vector3 posicion_inicial;
    private Quaternion orientacion_inicial;

    // Start is called before the first frame update
    void Start()
    {
        posicion_inicial = transform.position;
        orientacion_inicial = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rest_Pos()
    {
        transform.position = posicion_inicial;
        transform.rotation = orientacion_inicial;
    }

    public void Cam_x()
    {
        transform.position = new Vector3(-5, 1.5f, 0);
        transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void Cam_y()
    {
        transform.position = new Vector3(0, 7, 0);
        transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    public void Cam_z()
    {
        transform.position = posicion_inicial;
        transform.rotation = orientacion_inicial;
    }

    public void Zoom_neg()
    {
        if (transform.position.z > -10) {
            transform.Translate(0, 0, -0.5f);
        }
    }

    public void Zoom_pos()
    {
        transform.Translate(0, 0, 0.5f);
    }
}
