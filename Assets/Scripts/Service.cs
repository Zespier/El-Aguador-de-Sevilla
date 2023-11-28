using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour {

    public List<ServiceType> allServices;
    public static List<ServiceType> availableServices;

    public static Service instance;

    private void Awake() {
        if (instance == null) { instance = this; }
    }

}
