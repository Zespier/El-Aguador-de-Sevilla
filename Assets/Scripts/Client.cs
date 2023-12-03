using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Client : MonoBehaviour, IInteractable {

    public ServiceType desiredService;
    public GameObject bubble;
    public CanvasGroup angryCanvas;
    public UnityEngine.UI.Image serviceImage;
    public SpriteRenderer clientSprite;
    public Animator animator;
    public NavMeshAgent agent;
    public float probabilityOfPouringWater = 30f;
    public float timeToGetAngry = 20f;
    public float timeToLeave = 30f;

    [HideInInspector] public ClientGenerator generator;
    [HideInInspector] public int ID;
    private bool _reachedDestination;
    private int _currentSeat;
    private string _lastAnimationName;
    private float _angryTime;
    private bool _angry;

    public Vector3 MovementTarget { get; set; }

    private void Awake() {
        clientSprite.sprite = Resources.Load<Sprite>("NormalClient" + Random.Range(1, 8));
        angryCanvas.alpha = 0;
    }

    private void Start() {
        StartCoroutine(ClientMovement());
        agent.updateRotation = false;
        bubble.SetActive(false);
    }

    public void ChooseService() {
        int randomIndex = Random.Range(0, Service.availableServices.Count);
        desiredService = Service.availableServices[randomIndex];

        bubble.SetActive(true);
        serviceImage.sprite = desiredService.sprite;

        _angryTime = 0;
        StartCoroutine(GetAngryCoroutine());
    }

    public ServiceType Interact(ServiceType service) {
        if (service != null && desiredService != null && desiredService.name == service.name) {
            LevelController.instance.levels[LevelController.instance.currentLevel].score += service.score;
            Served(service.score);
            return null;
        }

        return service;
    }

    public void Served(int points) {
        Events.OnClientServed?.Invoke(points);
        desiredService = null;
        serviceImage.sprite = null;
        bubble.SetActive(false);
        GetOut();
        UnOccupieSeat();
        SoundFX.instance.PlaySound("ServeWater");
        MaybeSpawnAwa();
    }

    private void MaybeSpawnAwa() {
        float random = Random.Range(0, 100f);
        if (random < probabilityOfPouringWater) {
            AwaController.instance.SpawnAwa(transform.position, true);
        }
    }

    private IEnumerator ClientMovement() {
        _reachedDestination = false;
        yield return StartCoroutine(MovementCoroutine(LevelController.instance.levels[LevelController.instance.currentLevel].door.position, false));
        Vector3 _newSeatPosition = ChoosePlaceToSit(out bool serve);
        if (serve) {
            yield return StartCoroutine(MovementCoroutine(_newSeatPosition, serve));
        } else {
            GetOut();
        }
    }

    private Vector3 ChoosePlaceToSit(out bool serve) {

        List<Transform> seats = LevelController.instance.levels[LevelController.instance.currentLevel].seats;
        List<bool> occupied = LevelController.instance.levels[LevelController.instance.currentLevel].occupied;

        List<int> _availableSitsIndex = new List<int>();
        for (int i = 0; i < occupied.Count; i++) {
            if (!occupied[i]) {
                _availableSitsIndex.Add(i);
            }
        }

        if (_availableSitsIndex.Count > 0 && !LevelController.instance._showingScoreToPlayer) {

            int randomIndex = Random.Range(0, _availableSitsIndex.Count);

            occupied[_availableSitsIndex[randomIndex]] = true;
            _currentSeat = _availableSitsIndex[randomIndex];
            serve = true;
            return seats[_availableSitsIndex[randomIndex]].position;

        } else {
            serve = false;
            return LevelController.instance.levels[LevelController.instance.currentLevel].backHomePoint.position;
        }

    }

    private IEnumerator MovementCoroutine(Vector3 towards, bool order) {

        PlayAnimation("Walk");
        agent.SetDestination(towards);

        while (!_reachedDestination) {

            //Vector3 step = 3 * Time.deltaTime * (towards - transform.position).normalized;
            //transform.position = Vector3.MoveTowards(transform.position, towards, 3 * Time.deltaTime);
            FlipCharacter((towards - transform.position).x);

            if (Vector3.Distance(transform.position, towards) < 0.25f) {
                _reachedDestination = true;
            }
            yield return null;
        }
        _reachedDestination = false;

        if (order) {
            ChooseService();
            PlayAnimation("Idle");
        }

    }

    public void GetOut() {
        StartCoroutine(GetOutCoroutine());
        bubble.SetActive(false);
    }

    private IEnumerator GetOutCoroutine() {
        yield return StartCoroutine(MovementCoroutine(LevelController.instance.levels[LevelController.instance.currentLevel].backHomePoint.position, false));
        generator.RemoveClientFromList(this.ID);
        Destroy(gameObject);
    }

    private void UnOccupieSeat() {
        LevelController.instance.levels[LevelController.instance.currentLevel].occupied[_currentSeat] = false;
    }

    private void FlipCharacter(float x) {
        if (x != 0) {
            clientSprite.flipX = x > 0;
        }
    }

    private void PlayAnimation(string animationName) {

        if (animationName != _lastAnimationName) {
            animator.Play(animationName);
        }
        _lastAnimationName = animationName;
    }

    private IEnumerator GetAngryCoroutine() {
        while (_angryTime < timeToLeave) {
            _angryTime += Time.deltaTime;

            if (_angryTime >= timeToGetAngry) {
                GetAngry();
            }

            if (_angryTime >= timeToLeave) {
                GetOut();
            }
            yield return null;
        }
    }

    private void GetAngry() {
        if (!_angry) {
            _angry = true;
            angryCanvas.alpha = 1;

        }
    }
}
