using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public SpriteRenderer playerSprite;
    public float speed = 5f;
    [Header("Interactor")]
    public Transform interactionZone;
    public static bool blockInputs;


    private Vector3 _direction;
    private Collider[] _interactables = new Collider[3];
    private bool _movingToNextLevel;

    public static Vector3 NextlevelTarget { get; set; }

    private void Update() {

        if (blockInputs) { return; }

        Movement();
        Rotation();
        TestInteractionZone();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(interactionZone.position, interactionZone.localScale);
    }

    private void Movement() {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x == 0 && y == 0) { return; }

        _direction = new Vector3(x, 0, y);

        FlipCharacter(x);

        transform.position += speed * Time.deltaTime * _direction;
    }

    private void Rotation() {
        transform.forward = _direction;
    }

    private void FlipCharacter(float x) {
        if (x != 0) {
            playerSprite.flipX = x < 0;
        }
    }

    public void MovementInputSystem(InputAction.CallbackContext context) {
    }

    public void PickUp(InputAction.CallbackContext context) {
        if (context.started) {
            PickUp();
        }
    }

    private void PickUp() {

    }

    private void TestInteractionZone() {
        if (Physics.OverlapBoxNonAlloc(interactionZone.position, interactionZone.localScale / 2f, _interactables) > 0) {
            for (int i = 0; i < _interactables.Length; i++) {
                //Debug.Log(_interactables[i].gameObject.name + " Order: " + i);
            }
        }
    }

    /// <summary>
    /// Moves to the next level and stops blocking playerInputs
    /// </summary>
    public void NextLevelMovement() {
        if (!_movingToNextLevel) {
            StartCoroutine(MoveToNextLevel());
        }
    }

    private IEnumerator MoveToNextLevel() {
        _movingToNextLevel = true;


        while (Vector3.Distance(transform.position, NextlevelTarget) > 1) {
            //Cosas que podrían pasar con el movimiento al siguiente nivel
            // => Que se quede atrapado contra una pared => quitar el collider
            // => Que nunca llegue a esta distancia => Si se pasa del ángulo
            // => Que la dirección sea atravesando el edificio y nunca pueda entrar => Nav Mesh???

            yield return null;
        }

        blockInputs = false;
        _movingToNextLevel = false;
    }

}
