using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour, IInteractable {

    public ServiceType desiredService;
    public GameObject bubble;

    //Los clientes piden cosas
    // => Piden algo que est� disponible en ese momento
    // => Tienen una interaci�n, para que al igual que recogemos agua de un sitio concreto, si pulsamos el espacio pues le entregamos el agua

    private void Start() {
        ChooseService();
    }

    public void ChooseService() {
        int randomIndex = Random.Range(0, Service.availableServices.Count);
        desiredService = Service.availableServices[randomIndex];
    }

    public ServiceType Interact(ServiceType service) {
        if (desiredService.name == service.name) {
            Events.OnClientServed?.Invoke();
            desiredService = null;
            return null;
        }
        return service;
    }
}
