using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour, IInteractable {

    public ServiceType desiredService;
    public GameObject bubble;
    public Image serviceImage;

    private bool _reachedDestination;
    private int _currentSeat;

    public Vector3 MovementTarget { get; set; }

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
            Debug.LogError("Couldn't find any sit available");
            return -Vector3.forward * 100;
        }

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

        if (order) {
            ChooseService();
        }

    }

    private void GetOut() {
        StartCoroutine(MovementCoroutine(-Vector3.forward * 100, false));
    }

    private void UnOccupieSeat() {
        LevelController.instance.levels[LevelController.instance.currentLevel].occupied[_currentSeat] = false;
    }

}
