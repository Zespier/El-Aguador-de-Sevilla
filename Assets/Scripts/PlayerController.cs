using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 5f;

    private Vector3 _direction;

    private void Update() {
        Movement();
        Rotation();
    }

    private void Movement() {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x == 0 && y == 0) { return; }

        _direction = new Vector3(x, 0, y);

        transform.position += speed * Time.deltaTime * _direction;
    }

    private void Rotation() {
        transform.forward = _direction;
    }

}
