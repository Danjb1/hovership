using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PowerShardCounterController : MonoBehaviour, IPowerShardListener {

    private TextMeshProUGUI elem;

    void Start() {
        elem = GetComponent<TextMeshProUGUI>();
        StateManager.Instance.AddPowerShardListener(this);
    }

    void OnDisable() {
        StateManager.Instance.RemovePowerShardListener(this);
    }

    void Update() {

    }

    public void PowerShardCollected(int total) {
        elem.text = total.ToString();
    }

}
