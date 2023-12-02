using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBook : MonoBehaviour {

    public Animator bookAnimator;
    public Animator pageAnimator;
    public List<CanvasGroup> canvasLeftPages;
    public List<CanvasGroup> canvasRightPages;
    public GameObject anyKeyText;
    public List<string> _lastAnimation = new List<string>();

    private void Start() {
        StartCoroutine(WaitForAnyKey());
    }

    private IEnumerator WaitForAnyKey() {

        anyKeyText.SetActive(true);

        while (!Input.anyKeyDown) {
            yield return null;
        }

        bookAnimator.Play("Open");
        pageAnimator.Play("BookOpen");
        anyKeyText.SetActive(false);
    }

    public void OpenPage(int index) {
        ActiveCanvasGroup(index);
        PlayPageAnimation("Open", index);
    }

    public void ClosePage(int index) {
        PlayPageAnimation("Close", index);
    }

    /// <summary>
    /// 1 == Options | 2 == Credits
    /// </summary>
    /// <param name="index"></param>
    public void ActiveCanvasGroup(int index) {
        index--;
        for (int i = 0; i < canvasLeftPages.Count; i++) {
            if (i == index) {
                canvasLeftPages[i].alpha = 1;
                canvasLeftPages[i].interactable = true;
                canvasLeftPages[i].blocksRaycasts = true;

                canvasRightPages[i].alpha = 1;
                canvasRightPages[i].interactable = true;
                canvasRightPages[i].blocksRaycasts = true;

            } else {
                canvasLeftPages[i].alpha = 0;
                canvasLeftPages[i].interactable = false;
                canvasLeftPages[i].blocksRaycasts = false;

                canvasRightPages[i].alpha = 0;
                canvasRightPages[i].interactable = false;
                canvasRightPages[i].blocksRaycasts = false;
            }
        }
    }

    public void PlayPageAnimation(string animationName, int index) {
        index--;
        if (_lastAnimation[index] != animationName) {
            pageAnimator.Play(animationName);
        }

        _lastAnimation[index] = animationName;
    }
}
