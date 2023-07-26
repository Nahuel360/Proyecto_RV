using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{

    public void Go_Instrucciones()
    {
        SceneManager.LoadScene("Instrucciones");
    }
    public void Go_Crear()
    {
        SceneManager.LoadScene("Crear");
    }
    public void Go_Dimensionar()
    {
        if (GameObject.Find("Robot") != null)
        {
            // El game object existe
            SceneManager.LoadScene("Dimensionar");
        }
    }
    public void Go_Ensamblar()
    {
        if (GameObject.Find("Robot") != null)
        {
            // El game object existe
            SceneManager.LoadScene("Ensamblar");
        }
    }
    public void Go_Simular()
    {
        if (GameObject.Find("Robot") != null)
        {
            // El game object existe
            SceneManager.LoadScene("Simular");
        }
    }
    public void Salir(){
        Debug.Log("Saliendo");
        Application.Quit();
    }
}
