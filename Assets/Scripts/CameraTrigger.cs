using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour {

    public CameraBehaviour cameraBehaviour;
    public bool tabern;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            cameraBehaviour.onTabern = tabern;
        }
    }

}
