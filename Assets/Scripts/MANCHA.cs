using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MANCHA : MonoBehaviour {

    public BoxCollider boxCollider;
    public Collider[] contacts = new Collider[4];
    public float timeToReachSpeed = 0.6f;

    private bool _manchado = false;

    private void Update() {
        if (Physics.OverlapBoxNonAlloc(boxCollider.transform.position, boxCollider.size / 2f, contacts) > 1) {
            for (int i = 0; i < contacts.Length; i++) {
                if (contacts[i] != null && contacts[i].name.Contains("Player")) {
                    Debug.LogError("HITTING PLAYER");
                    if (contacts[i].TryGetComponent(out PlayerController player)) {
                        StartCoroutine(MEMANCHO(player));
                    }
                }
            }
        } else {

            _manchado = false;
            contacts = new Collider[4];

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
