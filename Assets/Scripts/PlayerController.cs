using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour {

    public SpriteRenderer playerSprite;
    public float speed = 5f;
    [Header("Interactor")]
    public Transform interactionZone;
    public static bool blockInputs;
    public ServiceType serviceInHand = null;
    public Animator animator;
    public bool _isServing;


    private Vector3 _direction;
    private Collider[] _interactables = new Collider[3];
    private bool _movingToNextLevel;
    private string _lastAnimationName;

    public static Vector3 NextlevelTarget { get; set; }

    private void Update() {

        if (blockInputs) { return; }

        Movement();
        CheckInteractionZone();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(interactionZone.position, interactionZone.localScale);
    }

    private void Movement() {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (x == 0 && y == 0) { if (!_isServing) { PlayAnimation("Idle"); }; return; }
        if (!_isServing) { PlayAnimation("Run"); }
        _direction = new Vector3(x, 0, y).normalized;

        FlipCharacter(x);

        transform.position += speed * Time.deltaTime * _direction;
    }

    private void Rotation() {
        transform.forward = _direction;
    }

    private void FlipCharacter(float x) {
        if (x != 0) {
            playerSprite.flipX = x > 0;
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
        for (int i = 0; i < _interactables.Length; i++) {
            if (_interactables[i] != null && _interactables[i].TryGetComponent(out IInteractable interactable)) {
                serviceInHand = interactable.Interact(serviceInHand);
                _isServing = true;
                PlayAnimation("ServeDrink");
            }
        }
    }

    private void CheckInteractionZone() {
        Physics.OverlapBoxNonAlloc(interactionZone.position, interactionZone.localScale / 2f, _interactables);
    }

    public IEnumerator NextLevelMovement() {

        if (!_movingToNextLevel) {
            _movingToNextLevel = true;
            Vector3 _startPosition = transform.position;

            while (Vector3.Distance(transform.position, NextlevelTarget) > 1) {
                //Cosas que podrían pasar con el movimiento al siguiente nivel
                // => Que se quede atrapado contra una pared => quitar el collider
                // => Que nunca llegue a esta distancia => Si se pasa del ángulo
                // => Que la dirección sea atravesando el edificio y nunca pueda entrar => Nav Mesh???
                transform.position = Vector3.MoveTowards(_startPosition, NextlevelTarget, speed * Time.deltaTime);
                yield return null;
            }

            blockInputs = false;
            _movingToNextLevel = false;
        }
    }

    public void PlayAnimation(string animationName) {
        if (animationName != _lastAnimationName) {
            animator.Play(animationName);
        }
        _lastAnimationName = animationName;
    }

}
