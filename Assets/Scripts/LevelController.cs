using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

    public List<Level> levels = new List<Level>();
    public List<Transform> levelsPositions = new List<Transform>();
    public int currentLevel = 0;
    public PlayerController player;

    private bool _transitioning;

    private void Awake() {
        StartLevel();
    }

    private void Update() {
        CheckInputs();
    }

    public void StartLevel() {
        Service.availableServices = levels[currentLevel].servicesAtLevel;
    }

    private void CheckInputs() {
        if (Input.GetKeyDown(KeyCode.C)) { CompleteLevel(); }

    }

    /// <summary>
    /// Blocks player inputs and start a transition to the next level
    /// </summary>
    public void CompleteLevel() {

        if (!_transitioning) {
            StartCoroutine(TransitionToNextLevel());
        }
    }

    private IEnumerator TransitionToNextLevel() {
        _transitioning = true;
        PlayerController.blockInputs = true;


        currentLevel++;
        PlayerController.NextlevelTarget = levelsPositions[currentLevel].position;
        yield return StartCoroutine(player.NextLevelMovement());

        _transitioning = false;
    }

    //private void MoveWalls(string direction) {
    //    if (direction.Contains("Up") || direction.Contains("up")) {

    //    } else if (direction.Contains("Down") || direction.Contains("down")) {

    //    }
    //}

}
