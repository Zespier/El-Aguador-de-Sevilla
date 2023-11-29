using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour, IInteractable {

    public ServiceType desiredService;
    public GameObject bubble;

    //Los clientes piden cosas
    // => Piden algo que esté disponible en ese momento
    // => Tienen una interación, para que al igual que recogemos agua de un sitio concreto, si pulsamos el espacio pues le entregamos el agua

    private void Start() {
        ChooseService();
    }

    public void ChooseService() {
        int randomIndex = Random.Range(0, Service.availableServices.Count);
        desiredService = Service.availableServices[randomIndex];
    }

    public ServiceType Interact(ServiceType service) {
        if (service != null && desiredService.name == service.name) {
            Events.OnClientServed?.Invoke();
            desiredService = null;
            Debug.Log("Client served with: " + service.name);
            return null;
        }

        Debug.Log("WRONG SERVICE");
        return service;
    }
}
