using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBook : MonoBehaviour {

    public Animator animator;

    private void Start() {
        StartCoroutine(WaitForAnyKey());
    }

    private IEnumerator WaitForAnyKey() {

        while (!Input.anyKeyDown) {
            yield return null;
        }

        animator.Play("Open");
    }

}
