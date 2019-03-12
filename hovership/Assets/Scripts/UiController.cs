using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour, IStateListener {

    private GameObject levelComplete;

    void Start() {
        Debug.Log("start");

        // Find UI elements
        levelComplete = transform.Find("LevelComplete").gameObject;

        // Subscribe to state changes
        StateManager.Instance.AddListener(this);

        // Initialise the UI
        UpdateUi(StateManager.Instance.GetState());
    }

    void Update() {
        
    }

    public void StateChanged(GameState newState) {
        UpdateUi(newState);
    }

    private void UpdateUi(GameState state) {

        // Hide all
        levelComplete.SetActive(false);

        // Show the relevant elements
        switch (state) {
            case GameState.CELEBRATING:
                levelComplete.SetActive(true);
                break;
        }
    }

}
