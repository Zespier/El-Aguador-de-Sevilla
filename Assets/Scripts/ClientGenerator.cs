using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGenerator : MonoBehaviour {

    public GameObject clientPrefab;
    public float timeToSpawnClients = 1.5f;

    private GameObject _newClient;
    private float _timerToSpawnClients;
    private List<Client> _generatedClients = new List<Client>();

    private void Awake() {
        Events.OnLevelCompleted += AllClientsGoTOFuckingHome;
    }

    private void Start() {
        SpawnClient();
    }

    private void Update() {
        _timerToSpawnClients += Time.deltaTime;
        if (_timerToSpawnClients > timeToSpawnClients) {
            _timerToSpawnClients = 0;
            SpawnClient();
        }
    }

    private void SpawnClient() {

        if (LevelController.instance.levels[LevelController.instance.currentLevel].occupied != null &&
            LevelController.instance.levels[LevelController.instance.currentLevel].occupied.Count > 0 &&
            LevelController.instance.levels[LevelController.instance.currentLevel].maxClients > _generatedClients.Count &&
            !LevelController.instance._showingScoreToPlayer) {

            bool _canSpawn = false;

            List<bool> occupied = LevelController.instance.levels[LevelController.instance.currentLevel].occupied;
            for (int i = 0; i < occupied.Count; i++) {
                if (!occupied[i]) {
                    _canSpawn = true;
                }
            }

            if (_canSpawn) {
                _newClient = Instantiate(clientPrefab);
                _newClient.transform.position = LevelController.instance.levels[LevelController.instance.currentLevel].spawnPoint.position;
                Client newClient = _newClient.GetComponent<Client>();
                newClient.generator = this;
                _generatedClients.Add(newClient);
            }
        }
    }

    public void RemoveClientFromList(Client client) {
        _generatedClients.Remove(client);
    }

    public void AllClientsGoTOFuckingHome() {
        for (int i = 0; i < _generatedClients.Count; i++) {
            _generatedClients[i].GetOut();
        }
    }
}
