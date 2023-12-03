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

    public TMP_Text timeLeftLevelTimer;
    public List<Level> levels = new List<Level>();
    public List<Transform> levelsPositions = new List<Transform>();
    public int currentLevel = 0;
    public PlayerController player;

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

        Events.OnLevelCompleted?.Invoke();

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
        SetIndexicalTimer();
        if (_levelTimer > levels[CurrentLevel].time) {
            CompleteLevel();
        }
    }

    private void SetIndexicalTimer() {


        float timeLeft = levels[CurrentLevel].time - _levelTimer;

        int minutes = (int)timeLeft / 60;
        int seconds = (int)timeLeft % 60;
        string secondsString = seconds >= 10 ? seconds.ToString() : "0" + seconds.ToString();
        timeLeftLevelTimer.text = minutes + ":" + secondsString;

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

    private void ActivateScoreCanvas(bool activate) {
        scoreCanvas.alpha = activate ? 1 : 0;
        scoreCanvas.interactable = activate;
        scoreCanvas.blocksRaycasts = activate;

        int sum = 0;
        for (int i = 0; i < levels.Count; i++) {
            sum += levels[i].score;
        }

        scoreText.text = sum.ToString();

        if (activate) {
            SoundFX.instance.PlaySound("LevelFinish");
        }
    }
}
