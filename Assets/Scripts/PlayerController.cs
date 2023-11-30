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
    public Transform interactionParent;
    public Transform interactionZone;
    public static bool blockInputs;
    public ServiceType serviceInHand = null;
    public Animator animator;
    public bool _isServing;
    public LevelController levelController;


    private Vector3 _direction;
    public Collider[] _interactables = new Collider[3];
    private bool _movingToNextLevel;
    private string _lastAnimationName;
    private float x;
    private float y;
    private int _currentAmountWater = 2;

    public static Vector3 NextlevelTarget { get; set; }

    private void Update() {

        Movement();
        CheckInteractionZone();
        RotateInteractionZone();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(interactionZone.position, interactionZone.localScale);
    }

    private void Movement() {

        if (!blockInputs) {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
        } else {
            x = 1;
            y = 1;
        }


        if (x == 0 && y == 0) { if (!_isServing) { PlayAnimation("Idle"); }; return; }
        if (!_isServing) { PlayAnimation("Run"); }

        if (!blockInputs) {
            _direction = new Vector3(x, 0, y).normalized;
        } else {
            _direction = (NextlevelTarget - transform.position).normalized;
        }

        FlipCharacter(x);

        transform.position += speed * Time.deltaTime * _direction;
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
        if (Physics.OverlapBoxNonAlloc(interactionZone.position, interactionZone.localScale / 2f, _interactables) > 0) {

        } else {
            _interactables = new Collider[3];
        }

    }

    public void StartNextLevelMovement() {
        StartCoroutine(NextLevelMovement());
    }

    // Hola soy César. Te deseo lo mejor en la jam, saludaso y bebe mucha agua, wapo
    // subnormal

    public IEnumerator NextLevelMovement() {

        if (!_movingToNextLevel) {
            _movingToNextLevel = true;

            while (Vector3.Distance(transform.position, NextlevelTarget) > 1) {
                //Cosas que podrían pasar con el movimiento al siguiente nivel
                // => Que se quede atrapado contra una pared => quitar el collider
                // => Que nunca llegue a esta distancia => Si se pasa del ángulo
                // => Que la dirección sea atravesando el edificio y nunca pueda entrar => Nav Mesh???
                transform.position = Vector3.MoveTowards(transform.position, NextlevelTarget, speed * Time.deltaTime);
                yield return null;
            }


            NextScenenarioReached();
        }
    }

    private void NextScenenarioReached() {
        blockInputs = false;
        _movingToNextLevel = false;
        levelController._transitioning = false;
        levelController._levelTimer = 0;

        Events.OnStartLevel?.Invoke();
    }

    public void PlayAnimation(string animationName) {
        if (animationName != _lastAnimationName) {
            animator.Play(animationName);
        }
        _lastAnimationName = animationName;
    }

    private void RotateInteractionZone()
    {
        interactionParent.forward = _direction;
    }
}
