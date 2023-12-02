using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MANCHA : MonoBehaviour {

    public BoxCollider boxCollider;
    public Collider[] contacts = new Collider[4];

    private void Update() {
        if (Physics.OverlapBoxNonAlloc(boxCollider.transform.position, boxCollider.size / 2f, contacts) > 0) {
            for (int i = 0; i < contacts.Length; i++) {
                if (contacts[i] != null && contacts[i].name.Contains("Player")) {
                    Debug.LogError("HITTING PLAYER");
                }
            }
        }
    }

}
