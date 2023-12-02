using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awa : MonoBehaviour {

    public SphereCollider sphereCollider;
    public Collider[] contacts = new Collider[1];
    public float timeToReachSpeed = 0.6f;

    private bool _manchado = false;

    private void Update() {
        contacts = Physics.OverlapSphere(sphereCollider.transform.position, sphereCollider.radius);
        if (contacts.Length > 0) {
            bool foundPlayer = false;
            for (int i = 0; i < contacts.Length; i++) {
                if (contacts[i] != null && contacts[i].name.Contains("Player")) {
                    if (contacts[i].TryGetComponent(out PlayerController player)) {
                        StartCoroutine(MEMANCHO(player));
                        foundPlayer = true;
                    }
                }
            }

            if (!foundPlayer) {
                _manchado = false;
                contacts = new Collider[1];
            }
        } else {

            _manchado = false;
            contacts = new Collider[1];

        }
    }

    private IEnumerator MEMANCHO(PlayerController player) {
        if (!_manchado) {
            _manchado = true;
            player._currentTimeToReachSpeed = timeToReachSpeed;

            while (_manchado) {
                yield return null;
            }

            player._currentTimeToReachSpeed = player.timeToReachSpeed;
        }
    }

}
