using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour {

    public List<Servicio> allServices;
    public static List<Servicio> availableServices;

    public static Service instance;

    private void Awake() {
        if (instance == null) { instance = this; }
    }

}
