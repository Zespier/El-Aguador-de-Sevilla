using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Client : MonoBehaviour, IInteractable {

    public ServiceType desiredService;
    public GameObject bubble;
    public UnityEngine.UI.Image serviceImage;
    public SpriteRenderer clientSprite;
    public Animator animator;
    public NavMeshAgent agent;

    [HideInInspector] public ClientGenerator generator;
    [HideInInspector] public int ID;
    private bool _reachedDestination;
    private int _currentSeat;
    private string _lastAnimationName;

    public Vector3 MovementTarget { get; set; }

    private void Awake() {
        clientSprite.sprite = Resources.Load<Sprite>("NormalClient" + Random.Range(1, 4));
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
    }

    public ServiceType Interact(ServiceType service) {
        if (service != null && desiredService != null && desiredService.name == service.name) {
            Served(service.score);
            Debug.Log("Client served with: " + service.name);
            LevelController.instance.levels[LevelController.instance.currentLevel].score += service.score;
            return null;
        }

        Debug.Log("WRONG SERVICE");
        return service;
    }

    public void Served(int points) {
        Events.OnClientServed?.Invoke(points);
        desiredService = null;
        serviceImage.sprite = null;
        bubble.SetActive(false);
        GetOut();
        UnOccupieSeat();
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

}
