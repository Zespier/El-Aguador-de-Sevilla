using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGenerator : MonoBehaviour {

    public GameObject clientPrefab;

    private Vector3 _direction;
    private GameObject _newClient;

    //Los clientes tienen
    // => Un punto donde aparecen
    // => Van a la puerta
    // => Eligen sitio
    // => Se mueven hasta el sitio

    private void Start() {
        SpawnClient();
    }

    private void SpawnClient() {
        _newClient = Instantiate(clientPrefab);
        _newClient.transform.position = LevelController.instance.levels[LevelController.instance.currentLevel].spawnPoint.position;
    }

}
