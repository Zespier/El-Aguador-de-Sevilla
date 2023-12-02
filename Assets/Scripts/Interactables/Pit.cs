using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Pit : MonoBehaviour, IInteractable {

    public ServiceType water;
    public PlayerController player;
    [Header("Minigame")]
    public List<Transform> sides;
    public CanvasGroup miniGameCanvas;
    public Slider miniGameSlider;
    public float timeToReachTop;
    public Vector2 retrievableSize = new Vector2(0, 1);
    public Vector2 dobleRetrievableSize = new Vector2(0, 1);
    public RectTransform retrievableAreaRect;
    public RectTransform dobleRetrievableAreaRect;

    private float _timerMiniGame;
    private int _timerDirection = 1;
    private bool _retrievingWater;

    public float RetrievableSize { get => retrievableSize.y - retrievableSize.x; }
    public float DobleRetrievableSize { get => dobleRetrievableSize.y - dobleRetrievableSize.x; }

    private void Awake() {
        Events.OnFullWaterRetrieved += StopRetrievingWater;
    }

    public ServiceType Interact(ServiceType service) {
        //Condiciones:
        // => Que tengamos las manos vacías
        // => Que tengamos agua ya pero no esté lleno el botijo
        // => Que te detenga el movimiento


        if (service == null || (service.name.Contains("Water") && PlayerController.CurrentAmountOfWater < PlayerController.maxAmountOfWater)) {
            Events.OnPickUp?.Invoke();
            StartCoroutine(PitMinigame());
            return water;
        } else {
            return service;
        }
    }

    private IEnumerator PitMinigame() {
        if (!_retrievingWater) {
            _retrievingWater = true;

            PlayerController.NextlevelTarget = PlayerController.instance.transform.position;
            PlayerController.blockInputs = true;
            ActivateCanvas(true);
            SetRetrievableArea();

            while (_retrievingWater) {
                Timer();
                miniGameSlider.value = _timerMiniGame / timeToReachTop;
                yield return null;
            }

            ActivateCanvas(false);
            PlayerController.blockInputs = false;
        }
    }

    private void ActivateCanvas(bool activate) {
        miniGameCanvas.alpha = activate ? 1 : 0;

        float maxDistance = 0;
        int index = 0;

        for (int i = 0; i < sides.Count; i++) {
            if (Vector3.Distance(player.transform.position, sides[i].position) > maxDistance) {
                maxDistance = Vector3.Distance(player.transform.position, sides[i].position);
                index = i;
            }
        }

        miniGameCanvas.transform.position = sides[index].transform.position;
    }

    private void Timer() {

        _timerDirection = Input.GetKey(KeyCode.Space) ? 1 : -1;
        _timerMiniGame += Time.deltaTime * _timerDirection;

        if (_timerMiniGame > timeToReachTop) {
            _timerMiniGame = timeToReachTop;
            _retrievingWater = false;
            AwaController.instance.SpawnAwa(player.transform.position);

        } else if (_timerMiniGame < 0) {

            _timerMiniGame = 0;
        }
    }

    public void PickUpWater(InputAction.CallbackContext context) {
        if (_retrievingWater && context.canceled) {
            if (miniGameSlider.value >= dobleRetrievableAreaRect.anchorMin.y && miniGameSlider.value <= dobleRetrievableAreaRect.anchorMax.y) {
                PlayerController.CurrentAmountOfWater += 2;
                PlayerController.instance.serviceInHand = water;
                Debug.Log("WATER ADDE => +2");
            } else if (miniGameSlider.value >= retrievableAreaRect.anchorMin.y && miniGameSlider.value <= retrievableAreaRect.anchorMax.y) {
                PlayerController.CurrentAmountOfWater++;
                PlayerController.instance.serviceInHand = water;
                Debug.Log("WATER ADDE => +1");
            }

            _timerMiniGame = 0;
        }
    }

    private void StopRetrievingWater() {
        _retrievingWater = false;
    }

    private void SetRetrievableArea() {

        float retrievableAreaMin = Random.Range(0, 1 - RetrievableSize);
        float retrievableAreaMax = retrievableAreaMin + RetrievableSize;
        float dobleRetrievableAreaMin = Random.Range(retrievableAreaMin, retrievableAreaMax - DobleRetrievableSize);
        float dobleRetrievableAreaMax = dobleRetrievableAreaMin + DobleRetrievableSize;

        retrievableAreaRect.anchorMin = new Vector2(0, retrievableAreaMin);
        retrievableAreaRect.anchorMax = new Vector2(1, retrievableAreaMax);
        dobleRetrievableAreaRect.anchorMin = new Vector2(0, dobleRetrievableAreaMin);
        dobleRetrievableAreaRect.anchorMax = new Vector2(1, dobleRetrievableAreaMax);
    }
}
