using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

    public List<Level> levels = new List<Level>();
    public List<List<Servicio>> servicesAtLevel = new List<List<Servicio>>();
    public int currentLevel = 0;
    public PlayerController player;

    private bool _transitioning;

    public void StartLevel() {
        Service.availableServices = servicesAtLevel[currentLevel];
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

        MoveWalls("up");

        PlayerController.NextlevelTarget = levels[currentLevel].transform.position;
        player.NextLevelMovement();

        yield return null;
        currentLevel++;

        MoveWalls("down");


        _transitioning = false;
    }

    private void MoveWalls(string direction) {
        if (direction.Contains("Up") || direction.Contains("up")) {

        } else if (direction.Contains("Down") || direction.Contains("down")) {

        }
    }

}
