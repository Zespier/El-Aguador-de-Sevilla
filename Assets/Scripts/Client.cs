using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour, IInteractable {

    public ServiceType desiredService;
    public GameObject bubble;
    public Image serviceImage;

    private bool _reachedDestination;
    private bool _isMoving;
    private Vector3 _direction;

    public Vector3 MovementTarget { get; set; }

    private void Awake() {
        Events.OnStartLevel += ChooseService;
    }

    private void Start() {
        StartCoroutine(ClientMovement());
    }

    public void ChooseService() {
        int randomIndex = Random.Range(0, Service.availableServices.Count);
        desiredService = Service.availableServices[randomIndex];

        bubble.SetActive(true);
        serviceImage.sprite = desiredService.sprite;
    }

    public ServiceType Interact(ServiceType service) {
        if (service != null && desiredService.name == service.name) {
            Served();
            Debug.Log("Client served with: " + service.name);
            return null;
        }

        Debug.Log("WRONG SERVICE");
        return service;
    }

    public void Served() {
        Events.OnClientServed?.Invoke();
        desiredService = null;
        serviceImage.sprite = null;
        bubble.SetActive(false);
    }

    private IEnumerator ClientMovement() {
        _reachedDestination = false;
        yield return StartCoroutine(MovementCoroutine(LevelController.instance.levels[LevelController.instance.currentLevel].door.position));
        yield return StartCoroutine(MovementCoroutine(ChoosePlaceToSit()));
    }

    private Vector3 ChoosePlaceToSit() {

        List<Transform> sits = LevelController.instance.levels[LevelController.instance.currentLevel].sits;
        List<bool> occupied = LevelController.instance.levels[LevelController.instance.currentLevel].occupied;
        for (int i = 0; i < sits.Count; i++) {
            if (!occupied[i]) {
                return sits[i].position;
            }
        }

        Debug.LogError("Couldn't find any sit available");
        return Vector3.zero;
    }

    private IEnumerator MovementCoroutine(Vector3 towards) {

        while (!_reachedDestination) {

            transform.position = Vector3.MoveTowards(transform.position, towards, 3 * Time.deltaTime);

            if (Vector3.Distance(transform.position, towards) < 0.25f) {
                _reachedDestination = true;
            }
            yield return null;
        }
    }

}
