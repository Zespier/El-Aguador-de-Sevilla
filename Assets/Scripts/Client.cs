using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour {

    public Servicio desiredService;

    //Los clientes piden cosas
    // => Piden algo que esté disponible en ese momento
    // => Tienen una interación, para que al igual que recogemos agua de un sitio concreto, si pulsamos el espacio pues le entregamos el agua

    public void ChooseService() {
        int randomIndex = Random.Range(0, Service.availableServices.Count);
        desiredService = Service.availableServices[randomIndex];
    }
}
