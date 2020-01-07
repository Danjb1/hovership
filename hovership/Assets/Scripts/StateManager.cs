using System.Collections.Generic;
using UnityEngine;

/**
* Singleton for managing overall game state.
*/
public class StateManager {

    // Singleton instance
    private static StateManager instance;

    public int numPowerShards;

    /**
     * Current game state.
     */
    private GameState gameState;

    private int powerShardsCollected;

    private float levelGroundHeight;

    private bool flightMode = true;

    private List<IStateListener> stateListeners = new List<IStateListener>();

    private List<IPowerShardListener> powerShardListeners =
            new List<IPowerShardListener>();

    // Accessor for singleton instance
    public static StateManager Instance {
        get {
            if (instance == null) {
                instance = new StateManager();
            }
            return instance;
        }
    }

    private StateManager() {}

    public void SetState(GameState gameState) {
        this.gameState = gameState;

        foreach (IStateListener listener in stateListeners) {
            listener.StateChanged(gameState);
        }
    }

    public GameState GetState() {
        return gameState;
    }

    public void SetFlightMode(bool flightMode) {
        this.flightMode = flightMode;
    }

    public bool IsFlightMode() {
        return flightMode;
    }

    public void AddStateListener(IStateListener listener) {
        stateListeners.Add(listener);
    }

    public void RemoveStateListener(IStateListener listener) {
        stateListeners.Remove(listener);
    }

    public void AddPowerShardListener(IPowerShardListener listener) {
        powerShardListeners.Add(listener);
    }

    public void RemovePowerShardListener(IPowerShardListener listener) {
        powerShardListeners.Remove(listener);
    }

    public void AddPowerShardsCollected(int n) {
        powerShardsCollected += n;

        // Inform listeners
        foreach (IPowerShardListener listener in powerShardListeners) {
            listener.PowerShardCollected(powerShardsCollected);
        }

        // Check for victory
        if (powerShardsCollected >= numPowerShards) {
            SetFlightMode(true);
        }
    }

    public float GetLevelGroundHeight() {
        return levelGroundHeight;
    }

    public void SetLevelGroundHeight(float height) {
        levelGroundHeight = height;
    }

}
