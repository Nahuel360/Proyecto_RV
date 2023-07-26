using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Instrucciones : MonoBehaviour
{
    // Start is called before the first frame update
    public void Go_Crear()
    {
        SceneManager.LoadScene("Crear");
    }
    public void Go_Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
