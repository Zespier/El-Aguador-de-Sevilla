using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour {

    public SpriteRenderer playerSprite;
    public float speed = 5f;
    [Header("Interactor")]
    public Transform interactionParent;
    public Transform interactionZone;
    public static bool blockInputs;
    public ServiceType serviceInHand = null;
    public Animator animator;
    public LevelController levelController;
    public static int maxAmountOfWater = 3;
    public ServiceType water;
    public CanvasGroup WaterAmountCanvas;

    private Vector3 _direction;
    public Collider[] _interactables = new Collider[3];
    private bool _movingToNextLevel;
    private string _lastAnimationName;
    private float x;
    private float y;

    public static Vector3 NextlevelTarget { get; set; }
    public static int CurrentAmountOfWater { get => _currentAmountWater; set => _currentAmountWater = SetCurrentAmountOfWater(value); }
    private static int _currentAmountWater = 0;

    public static PlayerController instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Update() {

        Movement();
        CheckInteractionZone();
        RotateInteractionZone();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(interactionZone.position, interactionZone.localScale);
    }

    private static int SetCurrentAmountOfWater(int value) {
        if (value >= maxAmountOfWater) {
            Events.OnFullWaterRetrieved?.Invoke();
        }
        return Mathf.Clamp(value, 0, maxAmountOfWater);
    }

    private void Movement() {

        if (!blockInputs) {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
        } else {
            x = 1;
            y = 1;
        }


        if (x == 0 && y == 0) { if (CurrentAmountOfWater == 0) { PlayAnimation("Idle"); }; return; }
        if (CurrentAmountOfWater == 0) { PlayAnimation("Run"); }

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

                bool lastWasWater = serviceInHand != null && serviceInHand.name.Contains("Water");

                serviceInHand = interactable.Interact(serviceInHand);

                if (serviceInHand == null && lastWasWater) {
                    CurrentAmountOfWater--;
                    serviceInHand = water;
                    if (CurrentAmountOfWater <= 0) {
                        //TODO: Stop serving water
                        serviceInHand = null;
                    }
                }
                PlayAnimation("ServeDrink");
            }
        }
    }

    //=> if the client wanted water, that means, we receive null
    //=> if the last thing we had was water => reduce water amount
    //=> if we reduce the water amount then check if there is enough water left.
    //=> 

    private void CheckInteractionZone() {
        if (Physics.OverlapBoxNonAlloc(interactionZone.position, interactionZone.localScale / 2f, _interactables) > 0) {

        } else {
            _interactables = new Collider[3];
        }

    }

    public void StartNextLevelMovement() {
        StartCoroutine(NextLevelMovement());
    }

    // Hola soy C�sar. Te deseo lo mejor en la jam, saludaso y bebe mucha agua, wapo
    // subnormal

    public IEnumerator NextLevelMovement() {

        if (!_movingToNextLevel) {
            _movingToNextLevel = true;

            while (Vector3.Distance(transform.position, NextlevelTarget) > 1) {
                //Cosas que podr�an pasar con el movimiento al siguiente nivel
                // => Que se quede atrapado contra una pared => quitar el collider
                // => Que nunca llegue a esta distancia => Si se pasa del �ngulo
                // => Que la direcci�n sea atravesando el edificio y nunca pueda entrar => Nav Mesh???
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

    private void RotateInteractionZone() {
        interactionParent.forward = _direction;
    }
}
