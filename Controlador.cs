using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Controlador : MonoBehaviour
{
    public TMP_Dropdown Selector_eslabones;
    public TMP_Text Selector;
    public Slider Deslizador;
    public GameObject BaseAR;

    private GameObject Robot_prefab;
    private GameObject Robot;
    private GameObject Base;
    private GameObject[] Eslabones;

    private float[] posiciones_deslizador;
    private int N_eslabon_anterior;
    private float posicion;

    //Variables para control PID
    private float orientacion_inicial;
    private float Consigna_PID;
    private float Lectura_Sensor;
    private float Lectura_Sensor_Anterior;
    private float lamda = (float)0.5;
    private float ruido = (float)0.1;

    private float Angulo1;
    private float Angulo2;

    private float e;
    private float e_ant;
    private float de;
    private float ie;

    private float kp;
    private float ki;
    private float kd;

    private float kpmin = (float)60;
    private float kpmax = (float)150;
    private float kimin = (float)31.57;
    private float kimax = (float)30.24;
    private float kdmin = (float)30;
    private float kdmax = (float)100;

    private float tiempo;
    private float tiempoAnt;
    private float deltaTime;

    private float Torque;

    // Start is called before the first frame update
    void Start()
    {
        //Hacemos una copia del robot y desactivamos el robot original
        Robot_prefab = GameObject.Find("Robot");
        Robot_prefab.SetActive(false);

        //Instanciamos una copia del robot que sirve para simular
        Robot = Instantiate(Robot_prefab);
        Destroy(Robot.GetComponent<No_Destruir>());
        Robot.transform.SetParent(BaseAR.transform);
        Robot.SetActive(true);

        //Obtenemos el gameobject de la base
        Base = GameObject.FindGameObjectWithTag("Base");
        //Armamos una lista con los eslabones (No olvidar poner el tag al eslabon)
        Eslabones = GameObject.FindGameObjectsWithTag("Eslabon");

        //Anulamos el congelamiento de las posiciones
        foreach (GameObject eslabon in Eslabones)
        {
            Rigidbody rb = eslabon.GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
        }

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

        //Armamos lista q almacena los valores de posicion de cada eslabon
        posiciones_deslizador = new float[Robot.transform.childCount];
        for (int i = 1; i < posiciones_deslizador.Length; i++)
        {
            posiciones_deslizador[i] = (float)0.5;
        }
        //Comenzamos seleccionando el eslabon 1
        N_eslabon_anterior = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Vuforia activa los renderers de todos los objetos
        //Desactivamos el cilindro de la base durante la ejecucion
        Base.GetComponent<Renderer>().enabled = false;

        foreach (GameObject eslabon in Eslabones)
        {
            //Desactivamos las capsulas de los eslabones
            eslabon.GetComponent<Renderer>().enabled = false;

            //Obtenemos el eslabon correspondiente
            if (Selector.text == eslabon.name)
            {
                int N_eslabon_actual = (int)char.GetNumericValue(eslabon.name[8]);

                //Actualizamos el valor de la barra deslizadora si cambiamos de eslabon
                if (N_eslabon_actual != N_eslabon_anterior)
                {
                    Deslizador.value = posiciones_deslizador[N_eslabon_actual];
                }

                //Almacenamos la posicion del deslizador para la articulacion en cuestion
                posiciones_deslizador[N_eslabon_actual] = Deslizador.value;

                //Calculamos la posicion objetivo y almacenamos esta posicion al deslizador correspondiente
                posicion = (eslabon.GetComponent<HingeJoint>().limits.max - eslabon.GetComponent<HingeJoint>().limits.min) * Deslizador.value + eslabon.GetComponent<HingeJoint>().limits.min;

                //Posicion objetivo aplicada al eslabon
                HingeJoint hinge = eslabon.GetComponent<HingeJoint>();
                JointSpring hingeSpring = hinge.spring;
                hingeSpring.targetPosition = posicion;
                hinge.spring = hingeSpring;

                //Actualizamos el setpoint para el caso donde haya Control PID en el eje 1 (En caso contrario ignorar esta seccion)
                if (N_eslabon_actual == 1)
                {
                    Consigna_PID = orientacion_inicial + (eslabon.GetComponent<HingeJoint>().limits.max - eslabon.GetComponent<HingeJoint>().limits.min) * Deslizador.value + eslabon.GetComponent<HingeJoint>().limits.min;
                    Debug.Log(Consigna_PID);
                }

                //Almacenamos el eslabon actual para la siguiente iteracion
                N_eslabon_anterior = N_eslabon_actual;
            }
        }

        //Aplicamos el control PID al eslabon 1
        //Tomamos la lectura del sensor
        if (Eslabones[0].transform.localRotation.eulerAngles.y < 180)
        {
            Lectura_Sensor = Eslabones[0].transform.rotation.eulerAngles.y + ((Random.value * (2 * ruido)) - ruido);
            tiempo = Time.time;
        }
        else
        {
            Lectura_Sensor = Eslabones[0].transform.rotation.eulerAngles.y - 360 + ((Random.value * (2 * ruido)) - ruido);
            tiempo = Time.time;
        }

        //Aplicamos el filtro leaky integrator
        Lectura_Sensor = lamda * Lectura_Sensor_Anterior + (1 - lamda) * Lectura_Sensor;

        //Obtenemos la posicion angular de los 2 eslabones mas relevantes
        if (Eslabones[1].transform.localRotation.eulerAngles.z < 180)
        {
            Angulo1 = Eslabones[1].transform.rotation.eulerAngles.z;
        }
        else
        {
            Angulo1 = Eslabones[1].transform.rotation.eulerAngles.z - 360;
        }

        if (Eslabones[2].transform.localRotation.eulerAngles.z < 180)
        {
            Angulo2 = Eslabones[2].transform.rotation.eulerAngles.z;
        }
        else
        {
            Angulo2 = (Eslabones[2].transform.rotation.eulerAngles.z - 360);
        }

        //Ajustamos las ganancias por metodo interpolacion
        kp = Mathf.Abs(Mathf.Sin(Angulo1 * Mathf.PI / 180) + Mathf.Sin(Angulo2 * Mathf.PI / 180)) / 2 * (kpmax - kpmin) + kpmin;
        ki = Mathf.Abs(Mathf.Sin(Angulo1 * Mathf.PI / 180) + Mathf.Sin(Angulo2 * Mathf.PI / 180)) / 2 * (kimax - kimin) + kimin;
        kd = Mathf.Abs(Mathf.Sin(Angulo1 * Mathf.PI / 180) + Mathf.Sin(Angulo2 * Mathf.PI / 180)) / 2 * (kdmax - kdmin) + kdmin;

        //Calculamos el paso
        deltaTime = tiempo - tiempoAnt;

        //Aplicamos el control PID
        if (deltaTime > 0)
        {
            //Calculamos el error
            e = Consigna_PID - Lectura_Sensor;
            //Calculamos la integral del error
            ie = (e + e_ant) * deltaTime / 2 + ie;
            //Calculamos la derivada del error
            de = (e - e_ant) / deltaTime;

            //Aplicamos el control en unity
            HingeJoint hinge2 = Eslabones[0].GetComponent<HingeJoint>();
            JointMotor hingeMotor = hinge2.motor;

            //Aplicamos el Torque 
            Torque = kp * e + kd * de + ki * ie;

            //limitamos el torque maximo por saturacion (Consideramos torque max 3010Nm)
            if (Mathf.Abs(Torque) > 3010)
            {
                if (Torque >= 0)
                {
                    Torque = 3010;
                }
                else
                {
                    Torque = -3010;
                }
            }

            //Aplicamos el torque en unity considerando su magnitud y sentido
            if (Torque >= 0)
            {
                hingeMotor.targetVelocity = 75;
                hingeMotor.force = Mathf.Abs(Torque);
            }
            else
            {
                hingeMotor.targetVelocity = -75;
                hingeMotor.force = Mathf.Abs(Torque);
            }
            hinge2.motor = hingeMotor;
        }

        //Almacenamos el error anterior y el tiempo
        e_ant = e;
        tiempoAnt = tiempo;
        Lectura_Sensor_Anterior = Lectura_Sensor;
    }

        public void Cerrar_simulacion()
    {
        //Al salir de la simulacion volvemos a activar el prefab
        Robot_prefab.SetActive(true);
    }
}
