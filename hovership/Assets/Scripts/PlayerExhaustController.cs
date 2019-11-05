using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExhaustController : MonoBehaviour, IStateListener {

    /**
     * Sound to use for the ship's engine.
     */
    public AudioClip engineSound;

    /**
     * Multiplier governing how quickly the exhaust emission rate increases.
     */
    private const float EXHAUST_SPOOL_UP_MULTIPLIER = 1000;

    /**
     * Multiplier governing how quickly the exhaust emission rate decreases.
     */
    private const float EXHAUST_SPOOL_DOWN_MULTIPLIER = 2000;

    /**
     * Minimum emission rate of the exhaust ParticleSystem.
     */
    private const float EXHAUST_MIN_EMISSION_RATE = 100;

    /**
     * Maximum emission rate of the exhaust ParticleSystem.
     */
    private const float EXHAUST_MAX_EMISSION_RATE = 300;

    /**
     * Lowest permitted engine pitch.
     */
    private const float ENGINE_MIN_PITCH = 0.9f;

    /**
     * Highest permitted engine pitch.
     */
    private const float ENGINE_MAX_PITCH = 1.1f;

    /**
     * Player's ParticleSystem component.
     */
    private ParticleSystem exhaust;

    /**
     * AudioSource for the engine sounds.
     */
    private AudioSource engineAudioSource;

    /**
     * Time since the engine sound was last played, in milliseconds.
     */
    private int msSinceEngineSoundPlayed;

    /**
     * Emission rate of the exhaust ParticleSystem.
     */
    private float exhaustEmissionRate;

    void Start() {

        // Acquire components
        exhaust = GetComponent<ParticleSystem>();
        engineAudioSource = GetComponent<AudioSource>();

        // Subscribe to state changes
        StateManager.Instance.AddStateListener(this);
    }

    void OnDisable() {
        // Unsubscribe from state changes
        StateManager.Instance.RemoveStateListener(this);
    }

    void FixedUpdate() {
        UpdateParticleSystem();
        UpdateAudio();
    }

    void UpdateParticleSystem() {

        if (Input.GetAxisRaw("Vertical") > 0) {
            // Player is holding forward
            exhaustEmissionRate +=
                    EXHAUST_SPOOL_UP_MULTIPLIER * Time.deltaTime;
        } else {
            // Player is not holding forward
            exhaustEmissionRate -=
                    EXHAUST_SPOOL_DOWN_MULTIPLIER * Time.deltaTime;
        }

        exhaustEmissionRate = Mathf.Clamp(exhaustEmissionRate,
                EXHAUST_MIN_EMISSION_RATE,
                EXHAUST_MAX_EMISSION_RATE);

        var emission = exhaust.emission;
        emission.rateOverTime = exhaustEmissionRate;
    }

    void UpdateAudio() {

        // Engine spool-up progress from 0-1
        float engineProgress = (exhaustEmissionRate - EXHAUST_MIN_EMISSION_RATE)
                / (EXHAUST_MAX_EMISSION_RATE - EXHAUST_MIN_EMISSION_RATE);

        // Set the pitch from MIN_PITCH to MAX_PITCH
        engineAudioSource.pitch = ENGINE_MIN_PITCH +
                (engineProgress * (ENGINE_MAX_PITCH - ENGINE_MIN_PITCH));

        // Play after some interval
        if (msSinceEngineSoundPlayed > 70) {
            engineAudioSource.PlayOneShot(engineSound);
            msSinceEngineSoundPlayed = 0;
        } else {
            msSinceEngineSoundPlayed += (int)(Time.deltaTime * 1000);
        }
    }

    public void StateChanged(GameState newState) {
        if (newState == GameState.PLAYING) {
            exhaust.Play();
        } else {
            exhaust.Pause();
        }
    }

}
