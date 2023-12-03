using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour {

    public SpriteRenderer playerSprite;
    public float speed = 5f;
    public float timeToReachSpeed = 0.2f;
    public Rigidbody rb;
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
    public Transform waterRotationUI;
    public Image waterFill;
    public NavMeshAgent agent;

    [HideInInspector] public float _currentTimeToReachSpeed;
    private Vector3 _direction;
    public Collider[] _interactables = new Collider[20];
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

        _currentTimeToReachSpeed = timeToReachSpeed;
        CurrentAmountOfWater = 0;
    }

    private void Update() {
        Movement();
        CheckInteractionZone();
        RotateInteractionZone();

        UpdateWaterUI();
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

            _direction = new Vector3(x, 0, y).normalized;

            FlipCharacter(x);

            rb.velocity = LerpSpeed(_direction * speed);

            if (x == 0 && y == 0) {
                PlayAnimation("Idle");
            } else {
                PlayAnimation("Run");
            }


        } else if (LevelController.instance._showingScoreToPlayer) {

            _direction = Vector3.zero;

            rb.velocity = LerpSpeed(_direction * speed);

            PlayAnimation("Idle");

        } else {
            _direction = (NextlevelTarget - transform.position).normalized;
            PlayAnimation("Run");

            FlipCharacter(_direction);
        }
        //transform.position += speed * Time.deltaTime * _direction;
    }

    private Vector3 LerpSpeed(Vector3 to) {
        return Vector3.Lerp(rb.velocity, to, Time.deltaTime / _currentTimeToReachSpeed);
    }

    private void CameraMovement(Vector3 playerPosition, Vector3 target, float speedTime) {
        transform.position = Vector3.Lerp(transform.position,
            playerPosition + target,
            Time.deltaTime / speedTime);
    }

    private void FlipCharacter(float x) {
        if (x != 0) {
            playerSprite.flipX = x > 0;
            waterRotationUI.localRotation = Quaternion.Euler(0, x > 0 ? 180 : 0, 0);
        }
    }
    private void FlipCharacter(Vector3 direction) {
        playerSprite.flipX = direction.x > 0;
        waterRotationUI.localRotation = Quaternion.Euler(0, direction.x > 0 ? 180 : 0, 0);
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
        if (Physics.OverlapBoxNonAlloc(interactionZone.position, interactionZone.localScale / 2f, _interactables) > 1) {

        } else {
            _interactables = new Collider[20];
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

            agent.isStopped = false;
            agent.updateRotation = false;
            agent.SetDestination(LevelController.instance.levels[LevelController.instance.currentLevel].door.position);

            while (Vector3.Distance(transform.position, LevelController.instance.levels[LevelController.instance.currentLevel].door.position) > 1) {
                //transform.position = Vector3.MoveTowards(transform.position, LevelController.instance.levels[LevelController.instance.currentLevel].door.position, speed * Time.deltaTime);
                yield return null;
            }

            agent.SetDestination(NextlevelTarget);

            while (Vector3.Distance(transform.position, NextlevelTarget) > 1) {
                //transform.position = Vector3.MoveTowards(transform.position, NextlevelTarget, speed * Time.deltaTime);
                yield return null;
            }

            agent.isStopped = true;

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
        interactionParent.forward = playerSprite.flipX ? Vector3.right : Vector3.left;
    }

    private void UpdateWaterUI() {
        WaterAmountCanvas.alpha = CurrentAmountOfWater > 0 ? 1 : 0;
        if (CurrentAmountOfWater > 0) {
            waterFill.fillAmount = CurrentAmountOfWater / (float)maxAmountOfWater;
        }

    }
}
