using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwaController : MonoBehaviour {

    public GameObject awaPrefab;
    public Transform area;
    public float timeToSpawnAwa = 10;
    public float _timerToSpawnAwa;
    public float numberOfRandomWaters = 2;

    public static AwaController instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }
    }

    private void Update() {
        _timerToSpawnAwa += Time.deltaTime;
        if (_timerToSpawnAwa >= timeToSpawnAwa) {
            _timerToSpawnAwa = 0;
            for (int i = 0; i < numberOfRandomWaters; i++) {
                Vector3 newPosition = new Vector3(Random.Range(area.position.x - area.localScale.x / 2f, area.position.x + area.localScale.x / 2f), 0, Random.Range(area.position.z - area.localScale.z / 2f, area.position.z + area.localScale.z));
                SpawnAwa(newPosition, false);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(area.position, area.localScale);
    }

    public void SpawnAwa(Vector3 position, bool sound) {
        Instantiate(awaPrefab, position, Quaternion.identity);
        if (sound) {
            SoundFX.instance.PlaySound("SplashWater");
        }
    }

}
