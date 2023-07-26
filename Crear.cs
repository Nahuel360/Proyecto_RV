using UnityEngine;
using TMPro;
using UnityEditor;



public class Crear : MonoBehaviour
{
    public GameObject Robot;
    public TMP_Text Num_elegido;

    private GameObject eslabon;
    private int N_eslabones;
    private GameObject Base;
    private GameObject[] Eslabones;
    private bool nuevo = false;

    public void Boton_Crear()
    {
        //Eliminamos el robot prefabricado
        if (GameObject.Find("Robot") != null)
        {
            Robot = GameObject.Find("Robot");
            Destroy(Robot.GetComponent<No_Destruir>());
            Destroy(Robot);
        }

        // Instanciamos el nuevo robot
        Robot = new GameObject
        {
            name = "Robot"
        };

        //Generamos la base
        GameObject myPrefab = Resources.Load<GameObject>("Base");
        eslabon = Instantiate(myPrefab);
        eslabon.name = ("Base");
        eslabon.tag = "Base";
        eslabon.transform.position = new Vector3(0, 0, 0);
        eslabon.transform.localScale = new Vector3(1, 0.2f, 1);
        eslabon.transform.SetParent(Robot.transform);

        //Generamos los eslabones
        N_eslabones = int.Parse(Num_elegido.text);
        for (int i = 1; i < N_eslabones + 2; i++)
        {
            if (i == N_eslabones+1)
            {
                myPrefab = Resources.Load<GameObject>("Efector");
                eslabon = Instantiate(myPrefab);
                eslabon.name = ("Eslabon " + i);
                eslabon.tag = "Eslabon";
                eslabon.transform.position = new Vector3(0, 0.8f + 1.2f * (i - 1)-0.4f, 0);
                eslabon.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                eslabon.transform.SetParent(Robot.transform);
            }
            else
            {
                myPrefab = Resources.Load<GameObject>("Eslabon");
                eslabon = Instantiate(myPrefab);
                eslabon.name = ("Eslabon " + i);
                eslabon.tag = "Eslabon";
                eslabon.transform.position = new Vector3(0, 0.8f + 1.2f * (i - 1), 0);
                eslabon.transform.localScale = new Vector3(0.25f, 0.6f, 0.25f);
                eslabon.transform.SetParent(Robot.transform);
            }
        }
        //Activamos el flag de que es un nuevo robot
        nuevo = true;
    }

    public void Boton_ABB()
    {
        //Eliminamos el robot prefabricado
        if (GameObject.Find("Robot") != null)
        {
            Robot = GameObject.Find("Robot");
            Destroy(Robot.GetComponent<No_Destruir>());
            Destroy(Robot);
        }

        // Instanciamos el nuevo robot
        GameObject myPrefab = Resources.Load<GameObject>("Robot ABB");
        Robot = Instantiate(myPrefab);
        Robot.name = "Robot"; 
        // Adds a component named "NombreDelScript" to the GameObject named "Robot"
        Robot.AddComponent<No_Destruir>();
    }

        public void Guardar_prefab() {
        if (nuevo == true)
        {
            //Configuramos las hingejoints y los rigidbodys
            //Tomamos el gameobject de la base
            Base = GameObject.FindGameObjectWithTag("Base");
            Base.AddComponent<Rigidbody>();
            Rigidbody rb_anterior = Base.GetComponent<Rigidbody>();
            rb_anterior.useGravity = false;
            rb_anterior.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            //Armamos una lista con los eslabones (No olvidar poner el tag al eslabon)
            Eslabones = GameObject.FindGameObjectsWithTag("Eslabon");
            foreach (GameObject eslabon in Eslabones)
            {
                //Rigidbody
                eslabon.AddComponent<HingeJoint>();
                Rigidbody rb = eslabon.GetComponent<Rigidbody>();
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

                //Hingejoint
                //Cuerpo conectado
                HingeJoint hinge = eslabon.GetComponent<HingeJoint>();
                hinge.connectedBody = rb_anterior;
                //Anchor
                hinge.anchor = new Vector3(0, -1, 0);
                //Eje de giro
                hinge.axis = new Vector3(0, 0, 1);
                //Activamos spring
                hinge.useSpring = true;
                JointSpring hingeSpring = hinge.spring;
                hingeSpring.spring = 1000000; // Reemplaza 100f con el valor deseado
                hingeSpring.damper = 1000000; // Reemplaza 0f con el valor deseado
                hinge.spring = hingeSpring;
                //Activamos limites
                hinge.useLimits = true;
                JointLimits hingeLimits = hinge.limits;
                hingeLimits.min = -90;
                hingeLimits.max = 90;
                hinge.limits = hingeLimits;
                //Guardamos el eslabon actual para el siguiente eslabon
                rb_anterior = eslabon.GetComponent<Rigidbody>();
            }
            //Cargamos el archivo de no destruir al cambiar de escena
            // Adds a component named "NombreDelScript" to the GameObject named "Robot"
            Robot.AddComponent<No_Destruir>();
        }
    }
}
