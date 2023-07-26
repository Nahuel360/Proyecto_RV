using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Crear : MonoBehaviour
{
    public void Go_Dimensionar()
    {
        if (GameObject.Find("Robot") != null)
        {
            if (GameObject.FindGameObjectWithTag("ABB") != null)
            {
                // El game object existe y es un prefab ABB
                SceneManager.LoadScene("Simular");
            }
            else
            {
                // El game object existe y es uno personalizado
                SceneManager.LoadScene("Dimensionar");
            }
            
        }
    }
    public void Go_Menu()
    {
        SceneManager.LoadScene("Menu");
    }
}
