using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    [Header("Next Level")]
    public CanvasGroup scoreCanvas;
    public TMP_Text scoreText;
    public Animator doorAnimator;

    public List<Level> levels = new List<Level>();
    public List<Transform> levelsPositions = new List<Transform>();
    public int currentLevel = 0;
    public PlayerController player;
    public List<int> points = new List<int>();

    [HideInInspector] public bool _transitioning;
    public float _levelTimer;
    [HideInInspector] public bool _showingScoreToPlayer;
    private int _lastLevel;

    public int CurrentLevel { get => currentLevel; set { _lastLevel = currentLevel; currentLevel = value; if (currentLevel >= levels.Count) { currentLevel = levels.Count - 1; EndGame(); } } }

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
        StartCoroutine(TransitionToNextLevelCoroutine());
    }

    private void LevelTimer() {
        if (_transitioning) { return; }
        _levelTimer += Time.deltaTime;
        if (_levelTimer > levels[CurrentLevel].time) {
            CompleteLevel();
        }
    }

    private IEnumerator TransitionToNextLevelCoroutine() {

        yield return StartCoroutine(ShowScoreToPlayer());

        doorAnimator.Play("Open");

        player.StartNextLevelMovement();


    }

    private IEnumerator ShowScoreToPlayer() {
        if (!_showingScoreToPlayer) {
            _showingScoreToPlayer = true;
            ActivateScoreCanvas(true);

            while (_showingScoreToPlayer) {
                yield return null;
            }

            ActivateScoreCanvas(false);
        }
    }

    public void CanMoveToNextLevel() {
        _showingScoreToPlayer = false;

        if (_lastLevel == levels.Count - 1) {
            SceneManager.LoadScene(0);
        }
    }

    private void EndGame() {

        Debug.LogWarning("Game Ended bruv you can go now");
    }

    private void AddPoints(int points) {
        this.points[currentLevel] += points;
    }

    private void ActivateScoreCanvas(bool activate) {
        scoreCanvas.alpha = activate ? 1 : 0;
        scoreCanvas.interactable = activate;
        scoreCanvas.blocksRaycasts = activate;

        scoreText.text = levels[currentLevel - 1].score.ToString();

        if (activate) {
            SoundFX.instance.PlaySound("LevelFinish");
        }
    }
}
