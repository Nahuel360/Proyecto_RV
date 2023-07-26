using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Ensamblador : MonoBehaviour
{
    public TMP_Dropdown Selector_eslabones;
    public TMP_Text Eslabon_elegido;
    public TMP_Text Desplazamiento_elegido;
    public GameObject Camara;
    
    private GameObject[] Eslabones;
    private GameObject eslabon_actual;

    private GameObject cylinder;
    private HingeJoint articulacion;

    private Vector3 posicion_inicial;
    private Quaternion orientacion_inicial;

    bool saliendo;

    bool Vista_x;
    bool Vista_y;
    bool Vista_z;

    // Start is called before the first frame update
    void Start()
    {
        //Posicion de la camara
        posicion_inicial = Camara.transform.position;
        orientacion_inicial = Camara.transform.rotation;
        Debug.Log(posicion_inicial);

        Vista_x = false;
        Vista_y = false;
        Vista_z = true;

        //Armamos una lista con los eslabones (No olvidar poner el tag al eslabon)
        Eslabones = GameObject.FindGameObjectsWithTag("Eslabon");

        //Actualizamos la lista de seleccion de eslabones
        Selector_eslabones.ClearOptions();
        //Create la lista con las nuevas opciones
        List<string> Options = new();
        foreach (GameObject eslabon in Eslabones)
        {
            Options.Add(eslabon.name);
        }
        //Aderimos las nuevas opciones a la lista
        Selector_eslabones.AddOptions(Options);
        //Comenzamos con el primer eslabon
        eslabon_actual = Eslabones[0];

        //Instanciamos un cilindro q representa el eje de rotacion
        cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cylinder.tag = "Eje";
        cylinder.transform.SetParent(eslabon_actual.transform);
        cylinder.GetComponent<Renderer>().material.color = Color.red;
        HingeJoint articulacion = eslabon_actual.GetComponent<HingeJoint>();
        cylinder.transform.localPosition = articulacion.anchor;
        cylinder.transform.localRotation = Quaternion.Euler(0, 0, 90);
        cylinder.transform.localScale = new Vector3(0.2f, 2f, 0.2f);

        //Variables q indica el proceso de salida
        saliendo = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (saliendo==false) 
        {
            foreach (GameObject eslabon in Eslabones)
            {
                if (Eslabon_elegido.text == eslabon.name)
                {
                    //Especificamos el eslabon actual
                    eslabon_actual = eslabon;

                    //Ajustamos el eje al eslabon correspondiente
                    cylinder.transform.SetParent(eslabon_actual.transform);
                    if (eslabon_actual.name != cylinder.transform.parent.name)
                    {
                        cylinder.transform.SetParent(eslabon_actual.transform);
                    }
                    articulacion = eslabon_actual.GetComponent<HingeJoint>();
                    cylinder.transform.localPosition = articulacion.anchor;
                    cylinder.transform.localScale = new Vector3(0.2f, 5f, 0.2f);

                    //Ajustamos el cilindro de acuerdo al eje de giro
                    if (articulacion.axis == new Vector3(1, 0, 0))
                    {
                        cylinder.transform.localRotation = Quaternion.Euler(0, 0, 90);
                    }
                    else if (articulacion.axis == new Vector3(0, 1, 0))
                    {
                        cylinder.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    }
                    else if (articulacion.axis == new Vector3(0, 0, 1))
                    {
                        cylinder.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    }

                    //Ajustamos la camara a la altura del eje
                    if (Vista_z == true) {
                        Camara.transform.position = new Vector3(cylinder.transform.position.x, cylinder.transform.position.y, Camara.transform.position.z);
                    }
                    else if (Vista_x == true)
                    {
                        Camara.transform.position = new Vector3(Camara.transform.position.x, cylinder.transform.position.y, cylinder.transform.position.z);
                    }
                    else if (Vista_y == true)
                    {
                        Camara.transform.position = new Vector3(cylinder.transform.position.x, Camara.transform.position.y, cylinder.transform.position.z);
                    }
                }
            }
        }
    }

    public void Mover_Xpos()
    {
        if (Desplazamiento_elegido.text == "Trasladar")
        {
            eslabon_actual.transform.Translate(new Vector3(0.1f, 0, 0));
        }

        if (Desplazamiento_elegido.text == "Rotar")
        {
            eslabon_actual.transform.Rotate(new Vector3(2, 0, 0));
        }
    }

    public void Mover_Xneg()
    {
        if (Desplazamiento_elegido.text == "Trasladar")
        {
            eslabon_actual.transform.Translate(new Vector3(-0.1f, 0, 0));
        }

        if (Desplazamiento_elegido.text == "Rotar")
        {
            eslabon_actual.transform.Rotate(new Vector3(-2, 0, 0));
        }
    }

    public void Mover_Ypos()
    {
        if (Desplazamiento_elegido.text == "Trasladar")
        {
            eslabon_actual.transform.Translate(new Vector3(0, 0.1f, 0));
        }
    }

    public void Mover_Yneg()
    {
        if (Desplazamiento_elegido.text == "Trasladar")
        {
            eslabon_actual.transform.Translate(new Vector3(0, -0.1f, 0));
        }
    }

    public void Mover_Zpos()
    {
        if (Desplazamiento_elegido.text == "Trasladar")
        {
            eslabon_actual.transform.Translate(new Vector3(0, 0, 0.1f));
        }

        if (Desplazamiento_elegido.text == "Rotar")
        {
            eslabon_actual.transform.Rotate(new Vector3(0, 0, 2));
        }
    }

    public void Mover_Zneg()
    {
        if (Desplazamiento_elegido.text == "Trasladar")
        {
            eslabon_actual.transform.Translate(new Vector3(0, 0, -0.1f));
        }

        if (Desplazamiento_elegido.text == "Rotar")
        {
            eslabon_actual.transform.Rotate(new Vector3(0, 0, -2));
        }
    }

    public void Eje_x()
    {
        articulacion.axis = new Vector3(1, 0, 0);
    }

    public void Eje_y()
    {
        articulacion.axis = new Vector3(0, 1, 0);
    }

    public void Eje_z()
    {
        articulacion.axis = new Vector3(0, 0, 1);
    }

    public void Cam_x()
    {
        Vista_x = true;
        Vista_y = false;
        Vista_z = false;

        Camara.transform.SetPositionAndRotation(new Vector3(-5, 1.5f, 0), Quaternion.Euler(0, 90, 0));
    }

    public void Cam_y()
    {
        Vista_x = false;
        Vista_y = true;
        Vista_z = false;

        Camara.transform.SetPositionAndRotation(new Vector3(0, 7, 0), Quaternion.Euler(90, 0, 0));
    }

    public void Cam_z()
    {
        Vista_x = false;
        Vista_y = false;
        Vista_z = true;

        Camara.transform.SetPositionAndRotation(posicion_inicial, orientacion_inicial);
    }

    public void Zoom_neg()
    {
        Camara.transform.Translate(0, 0, -0.50f);
    }

    public void Zoom_pos()
    {
        Camara.transform.Translate(0, 0, 0.50f);
    }

    public void Guardar_prefab()
    {
        saliendo = true;

        //Eliminamos todos los ejes
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Eje"))
        {
            Destroy(obj);
        }

        if (GameObject.FindGameObjectsWithTag("Eje").Length != 0)
        {
            //Cambiamos de escena
            SceneManager.LoadScene("Simular");
        }
    }

    public void Go_Menu()
    {
        saliendo = true;

        //Eliminamos todos los ejes
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Eje"))
        {
            Destroy(obj);
        }

        if (GameObject.FindGameObjectsWithTag("Eje").Length != 0)
        {
            //Cambiamos de escena
            SceneManager.LoadScene("Menu");
        }
    }
}
