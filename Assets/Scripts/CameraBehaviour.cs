using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour {

    public PlayerController player; //Necessary not for the camera, but for the movement of the target.
    public Transform mainCamera;
    public float cameraLerpingSpeed = 0.4f;
    public float targetLerpingSpeed = 0.4f;
    [Header("CameraLimits")]
    public Transform minLimit;
    public Transform maxLimit;
    public Transform tabernMinLimit;
    public Transform tabernMaxLimit;
    [Header("Shake")]
    public float shakeDuration = 0.2f;
    public float shakeAmplitude = 0.1f;

    private Vector3 _selfieStick;
    private Coroutine _movingCamera;
    private Coroutine _cameraShakeAnimation;
    [HideInInspector] public bool onTabern;

    public Vector3 Target { get; set; }
    public float CameraLerpSpeed { get => cameraLerpingSpeed; set => cameraLerpingSpeed = value; }
    public float TargetLerpingSpeed { get => targetLerpingSpeed; set => targetLerpingSpeed = value; }

    private void Awake() {
        _selfieStick = transform.position - player.transform.position;
    }

    private void FixedUpdate() {
        CameraMovement(NewCameraPosition(), Target, CameraLerpSpeed);
    }

    private Vector3 NewCameraPosition() {
        Vector3 newPosition = player.transform.position;

        if (onTabern) {
            newPosition.x = Mathf.Clamp(newPosition.x, tabernMinLimit.position.x, tabernMaxLimit.position.x);
            newPosition.z = Mathf.Clamp(newPosition.z, tabernMinLimit.position.z, tabernMaxLimit.position.z);
        } else {
            newPosition.x = Mathf.Clamp(newPosition.x, minLimit.position.x, maxLimit.position.x);
            newPosition.z = Mathf.Clamp(newPosition.z, minLimit.position.z, maxLimit.position.z);
        }

        return newPosition + _selfieStick;
    }

    private void CameraMovement(Vector3 playerPosition, Vector3 target, float speedTime) {
        transform.position = Vector3.Lerp(transform.position,
            playerPosition + target,
            Time.deltaTime / speedTime);
    }

    public void StartMovingTarget(Vector3 current, Vector3 target, float time) {
        if (_movingCamera != null) {
            StopCoroutine(_movingCamera);
        }
        _movingCamera = StartCoroutine(MovingTarget(current, target, time));
    }
    public void StartMovingTargeteru(Vector3 current, Vector3 target, float time) {
        if (_movingCamera != null) {
            StopCoroutine(_movingCamera);
        }
        _movingCamera = StartCoroutine(MovingTarget(current, target, time));
    }

    private IEnumerator MovingTarget(Vector3 current, Vector3 target, float totalTime) {

        float timer = totalTime;
        while (timer >= 0) {
            Target = Vector3.Lerp(current, target, 1 - (timer / totalTime));
            timer -= Time.deltaTime;
            yield return null;
        }
    }

    #region Shake

    public void CameraShake() {
        if (_cameraShakeAnimation != null) {
            StopCoroutine(_cameraShakeAnimation);
        }

        _cameraShakeAnimation = StartCoroutine(CameraShakeAnimation());
    }
    private IEnumerator CameraShakeAnimation() {

        float timer = shakeDuration;
        while (timer >= 0) {

            mainCamera.localPosition = new Vector3(UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude), UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude), UnityEngine.Random.Range(-shakeAmplitude, shakeAmplitude));

            timer -= Time.deltaTime;
            yield return null;
        }

        mainCamera.localPosition = Vector3.zero;
    }
    #endregion
}
