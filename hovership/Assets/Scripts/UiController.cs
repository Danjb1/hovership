using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour, IStateListener {

    private GameObject paused;
    private GameObject levelComplete;

    void Start() {

        // Find UI elements
        paused = transform.Find("Paused").gameObject;
        levelComplete = transform.Find("Level Complete").gameObject;

        // Subscribe to state changes
        StateManager.Instance.AddStateListener(this);

        // Initialise the UI
        UpdateUi(StateManager.Instance.GetState());
    }

    void OnDisable() {
        // Unsubscribe
        StateManager.Instance.RemoveStateListener(this);
    }

    void Update() {
        if (Input.GetButtonUp("Cancel")) {
            togglePaused();
        }
    }

    private void togglePaused() {
        if (StateManager.Instance.GetState() == GameState.PLAYING) {
            StateManager.Instance.SetState(GameState.PAUSED);
        } else {
            StateManager.Instance.SetState(GameState.PLAYING);
        }
    }

    public void StateChanged(GameState newState) {
        UpdateUi(newState);
    }

    private void UpdateUi(GameState state) {

        // Hide all
        paused.SetActive(false);
        levelComplete.SetActive(false);

        // Show the relevant elements
        switch (state) {
            case GameState.CELEBRATING:
                levelComplete.SetActive(true);
                break;
            case GameState.PAUSED:
                paused.SetActive(true);
                break;
        }
    }

}
