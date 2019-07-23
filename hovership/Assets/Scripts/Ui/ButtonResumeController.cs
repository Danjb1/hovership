using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonResumeController : MonoBehaviour, IPointerClickHandler {

    void Start() {}

    void Update() {}

    public void OnPointerClick(PointerEventData eventData) {
        StateManager.Instance.SetState(GameState.PLAYING);
    }

}
