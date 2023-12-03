using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwaController : MonoBehaviour {

    public GameObject awaPrefab;

    public static AwaController instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    public void SpawnAwa(Vector3 position) {
        Instantiate(awaPrefab, position, Quaternion.identity);
        SoundFX.instance.PlaySound("SplashWater");
    }

}
