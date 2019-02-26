/**
 * Singleton for managing overall game state.
 */
public class StateManager {

    // Singleton instance
    private static StateManager instance;

    /**
     * Current game state.
     */
    public GameState gameState;

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
}
