using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererOrientation : MonoBehaviour {

    private Camera _mainCamera;

    private void Awake() {
        _mainCamera = Camera.main;

        transform.forward = _mainCamera.transform.forward;
    }

}
