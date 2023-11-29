using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour, IInteractable {

    public ServiceType desiredService;
    public GameObject bubble;
    public Image serviceImage;

    private bool _reachedDestination;

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
        yield return StartCoroutine(MovementCoroutine(LevelController.instance.levels[LevelController.instance.currentLevel].door.position, false));
        Debug.Log("Moving to my seat");
        yield return StartCoroutine(MovementCoroutine(ChoosePlaceToSit(), true));
        Debug.Log("Reached my seat");
    }

    private Vector3 ChoosePlaceToSit() {

        List<Transform> sits = LevelController.instance.levels[LevelController.instance.currentLevel].sits;
        List<bool> occupied = LevelController.instance.levels[LevelController.instance.currentLevel].occupied;

        int count = sits.Count;

        do {
            int randomIndex = Random.Range(0, sits.Count);

            if (!occupied[randomIndex]) {
                return sits[randomIndex].position;
            }
            count--;

        } while (count > 0);

        Debug.LogError("Couldn't find any sit available");
        return Vector3.zero;
    }

    private IEnumerator MovementCoroutine(Vector3 towards, bool order) {

        while (!_reachedDestination) {

            transform.position = Vector3.MoveTowards(transform.position, towards, 3 * Time.deltaTime);

            if (Vector3.Distance(transform.position, towards) < 0.25f) {
                _reachedDestination = true;
            }
            yield return null;
        }
        _reachedDestination = false;
    }

}
