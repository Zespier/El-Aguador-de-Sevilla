using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBook : MonoBehaviour {

    public Animator animator;
    public List<Animator> pagesAnimator;
    public List<CanvasGroup> canvasPages;
    public GameObject anyKeyText;

    private List<string> _lastAnimation;

    private void Start() {
        StartCoroutine(WaitForAnyKey());
    }

    private IEnumerator WaitForAnyKey() {

        anyKeyText.SetActive(true);

        while (!Input.anyKeyDown) {
            yield return null;
        }

        animator.Play("Open");
        anyKeyText.SetActive(false);
    }

    public void OpenPage(int index) {
        ActiveCanvasGroup(index);
        PlayAnimation("Open", index);
    }

    public void ClosePage(int index) {
        PlayAnimation("Close", index);
    }

    /// <summary>
    /// 1 == Options | 2 == Credits
    /// </summary>
    /// <param name="index"></param>
    public void ActiveCanvasGroup(int index) {
        for (int i = 1; i <= canvasPages.Count; i++) {
            if (i == index) {
                canvasPages[i].alpha = 1;
                canvasPages[i].interactable = true;
                canvasPages[i].blocksRaycasts = true;
            } else {
                canvasPages[i].alpha = 0;
                canvasPages[i].interactable = false;
                canvasPages[i].blocksRaycasts = false;
            }
        }
    }

    public void PlayAnimation(string animationName, int index) {
        if (_lastAnimation[index] != animationName) {
            pagesAnimator[index].Play(animationName);
        }

        _lastAnimation[index] = animationName;
    }
}
