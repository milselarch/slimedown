using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Cursor = UnityEngine.Cursor;

public class PauseMenuManager : MonoBehaviour {
    public GameObject pauseCanvas;
    public UIDocument pauseUI;
    
    private Button _playButton;
    private Button _manuMenuButton;

    private void Initialize() {
        var root = pauseUI.rootVisualElement;
        this._playButton = root.Q<Button>("PlayButton");
        this._manuMenuButton = root.Q<Button>("MainMenuButton");
            
        Assert.IsNotNull(this._playButton);
        this._playButton.clickable.clicked += OnPlayButtonClicked;
        this._manuMenuButton.clickable.clicked += OnMainMenuButtonClicked;
    }
    
    private void Show() {
        pauseUI.rootVisualElement.style.visibility = Visibility.Visible;
        pauseCanvas.SetActive(true);
        Cursor.visible = true;
    }

    private void Hide() {
        pauseUI.rootVisualElement.style.visibility = Visibility.Hidden;
        pauseCanvas.SetActive(false);
        Cursor.visible = false;
    }
    
    private void OnPlayButtonClicked() {
        GameState.Paused = false;
        Hide();
    }

    private void OnMainMenuButtonClicked() {
        SceneManager.LoadSceneAsync("Menu", LoadSceneMode.Single);
    }
    
    private IEnumerator LaunchInitializer() {
        // wait for UI document to render
        yield return new WaitForEndOfFrame();
        Initialize();
    }
    
    private void Start() {
        Hide();
        // wait for UI document to render
        StartCoroutine(LaunchInitializer());
    }
    
    public void TogglePause(InputAction.CallbackContext context) {
        if (GameState.Dead) {
            // don't pause game if game over
            return;
        }

        GameState.Paused = !GameState.Paused;
        if (GameState.Paused) {
            Show();
        } else {
            Hide();
        }
    }
}

