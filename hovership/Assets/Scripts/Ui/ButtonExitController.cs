using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonExitController : MonoBehaviour, IPointerClickHandler {

    void Start() { }

    void Update() { }

    public void OnPointerClick(PointerEventData eventData) {
        Application.Quit();
    }

}
