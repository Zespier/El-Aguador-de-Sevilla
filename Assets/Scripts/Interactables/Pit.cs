using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pit : MonoBehaviour, IInteractable {

    public ServiceType water;


    public ServiceType Interact(ServiceType service) {
        if (service == null) {
            Events.OnPickUp?.Invoke();
            return water;
        } else {
            return service;
        }
    }
}
