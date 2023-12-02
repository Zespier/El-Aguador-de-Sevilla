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

    private bool _reachedDestination;
    private int _currentSeat;
    private string _lastAnimationName;

    public Vector3 MovementTarget { get; set; }

    private void Start() {
        StartCoroutine(ClientMovement());
        agent.updateRotation = false;
    }

    public void ChooseService() {
        int randomIndex = Random.Range(0, Service.availableServices.Count);
        desiredService = Service.availableServices[randomIndex];

        bubble.SetActive(true);
        serviceImage.sprite = desiredService.sprite;
    }

    public ServiceType Interact(ServiceType service) {
        if (service != null && desiredService.name == service.name) {
            Served(service.points);
            Debug.Log("Client served with: " + service.name);
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
        yield return StartCoroutine(MovementCoroutine(ChoosePlaceToSit(), true));
    }

    private Vector3 ChoosePlaceToSit() {

        List<Transform> seats = LevelController.instance.levels[LevelController.instance.currentLevel].seats;
        List<bool> occupied = LevelController.instance.levels[LevelController.instance.currentLevel].occupied;

        List<int> _availableSitsIndex = new List<int>();
        for (int i = 0; i < occupied.Count; i++) {
            if (!occupied[i]) {
                _availableSitsIndex.Add(i);
            }
        }

        if (_availableSitsIndex.Count > 0) {

            int randomIndex = Random.Range(0, _availableSitsIndex.Count);

            occupied[_availableSitsIndex[randomIndex]] = true;
            _currentSeat = _availableSitsIndex[randomIndex];
            return seats[_availableSitsIndex[randomIndex]].position;

        } else {
            //Debug.LogError("Couldn't find any sit available");
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

    private void GetOut() {
        StartCoroutine(MovementCoroutine(LevelController.instance.levels[LevelController.instance.currentLevel].backHomePoint.position, false));
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
