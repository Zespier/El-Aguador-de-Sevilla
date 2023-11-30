using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

    public List<Level> levels = new List<Level>();
    public List<Transform> levelsPositions = new List<Transform>();
    public int currentLevel = 0;
    public PlayerController player;
    public List<int> points = new List<int>();

    [HideInInspector] public bool _transitioning;
    public float _levelTimer;

    public int CurrentLevel { get => currentLevel; set { currentLevel = value; if (currentLevel >= levels.Count) { currentLevel = levels.Count - 1; EndGame(); } } }

    public static LevelController instance;
    private void Awake() {
        if (instance == null) {
            instance = this;
        }

        StartLevel();
        Events.OnClientServed += AddPoints;
    }

    private void Update() {
        LevelTimer();
        CheckInputs();
    }

    public void StartLevel() {
        Service.availableServices = levels[CurrentLevel].servicesAtLevel;
    }

    private void CheckInputs() {
        if (Input.GetKeyDown(KeyCode.C)) { CompleteLevel(); }

    }

    /// <summary>
    /// Blocks player inputs and start a transition to the next level
    /// </summary>
    public void CompleteLevel() {

        if (!_transitioning) {
            TransitionToNextLevel();
        }
    }

    private void TransitionToNextLevel() {
        _transitioning = true;
        PlayerController.blockInputs = true;


        CurrentLevel++;
        PlayerController.NextlevelTarget = levelsPositions[CurrentLevel].position;
        player.StartNextLevelMovement();

    }

    private void LevelTimer() {
        if (_transitioning) { return; }
        _levelTimer += Time.deltaTime;
        if (_levelTimer > levels[CurrentLevel].time) {
            CompleteLevel();
        }

    }

    private void EndGame() {

        Debug.LogWarning("Game Ended bruv you can go now");

    }

    private void AddPoints(int points) {
        this.points[currentLevel] += points;
    }

}
