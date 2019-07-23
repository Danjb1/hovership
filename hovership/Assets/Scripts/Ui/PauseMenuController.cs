using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuController : MonoBehaviour, IStateListener {

    private GameObject paused;
    private GameObject buttonResume;
    private GameObject buttonExit;

    void Start() {

        // Find UI elements
        paused = transform.Find("Paused").gameObject;
        buttonResume = transform.Find("ButtonResume").gameObject;
        buttonExit = transform.Find("ButtonExit").gameObject;

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
        SetActive(state == GameState.PAUSED);
    }

    private void SetActive(bool active) {
        paused.SetActive(active);
        buttonResume.SetActive(active);
        buttonExit.SetActive(active);
    }

}
