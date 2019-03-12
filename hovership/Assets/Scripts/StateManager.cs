
using System.Collections.Generic;
/**
* Singleton for managing overall game state.
*/
public class StateManager {

    // Singleton instance
    private static StateManager instance;

    /**
     * Current game state.
     */
    private GameState gameState;

    private List<IStateListener> listeners = new List<IStateListener>();

    // Accessor for singleton instance
    public static StateManager Instance {
        get {
            if (instance == null) {
                instance = new StateManager();
            }
            return instance;
        }
    }

    private StateManager(){ }

    public void SetState(GameState gameState) {
        this.gameState = gameState;

        foreach (IStateListener listener in listeners) {
            listener.StateChanged(gameState);
        }
    }

    public GameState GetState() {
        return gameState;
    }

    public void AddListener(IStateListener listener) {
        listeners.Add(listener);
    }

    public void RemoveListener(IStateListener listener) {
        listeners.Remove(listener);
    }

}
