using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour {

    public CanvasGroup pauseMenuCanvas;

    public void OpenMenuAction(InputAction.CallbackContext context) {
        if (context.started) {
            OpenMenu();
        }
    }

    private void OpenMenu() {
        bool activate = pauseMenuCanvas.alpha == 0 ? true : false;

        pauseMenuCanvas.alpha = activate ? 1 : 0;
        pauseMenuCanvas.interactable = activate;
        pauseMenuCanvas.blocksRaycasts = activate;
    }

}
