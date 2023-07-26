using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class Dimensionar : MonoBehaviour
{
    public TMP_Text Radio;
    public TMP_Text Altura;
    public TMP_Dropdown Selector_eslabones;
    public TMP_Text Eslabon_elegido;
    public GameObject Camara;

    private GameObject Base;
    private GameObject[] Eslabones;
    private GameObject eslabon_actual;

    bool saliendo;

    // Start is called before the first frame update
    void Start()
    {
        //Armamos una lista con los eslabones (No olvidar poner el tag al eslabon)
        Base = GameObject.Find("Base");
        Base.tag = "Eslabon";
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

        //Variables q indica el proceso de salida
        saliendo = false;
    }

    // Update is called once per frame
    void Update()
    {   
        if (saliendo == false)
        {
            foreach (GameObject eslabon in Eslabones)
            {
                if (Eslabon_elegido.text == eslabon.name)
                {
                    eslabon_actual = eslabon;
                    eslabon.SetActive(true);
                    Camara.transform.position = new Vector3(Camara.transform.position.x, eslabon.transform.position.y, Camara.transform.position.z);
                }
                else
                {
                    eslabon.SetActive(false);
                }
            }
        }
        
    }

    public void Aplicar_cambios()
    {
        //Tomamos solo los numeros del inputfield
        string soloNumeros_Radio = new((Radio.text).Where(char.IsDigit).ToArray());
        string soloNumeros_Altura = new((Altura.text).Where(char.IsDigit).ToArray());
        //Modificamos el tamaño del eslabon
        eslabon_actual.transform.localScale = new Vector3(float.Parse(soloNumeros_Radio)/100, float.Parse(soloNumeros_Altura)/100/2, float.Parse(soloNumeros_Radio)/100);
    }

    public void Guardar_prefab()
    {
        saliendo = true;

        foreach (GameObject eslabon in Eslabones)
        {
            eslabon.SetActive(true);
        }

        Base = GameObject.Find("Base");
        Base.tag = "Base";

        //Cambiamos de escena
        SceneManager.LoadScene("Ensamblar");
    }

    public void Go_Menu()
    {
        saliendo = true;

        foreach (GameObject eslabon in Eslabones)
        {
            eslabon.SetActive(true);
        }

        Base = GameObject.Find("Base");
        Base.tag = "Base";

        //Cambiamos de escena
        SceneManager.LoadScene("Menu");
    }
}
