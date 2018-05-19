using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Rocket : MonoBehaviour {
    [SerializeField] float rcsThrust = 100f,
                           mainThrust = 100f;
    [SerializeField] AudioClip mainEngine,
                               levelCompleted,
                               obstacleHit;
    [SerializeField] ParticleSystem mainEngineParticles,
                                    levelCompletedParticles,
                                    obstacleHitParticles;
    Rigidbody rigidBody;
    AudioSource audioSource;
    float invokeWaitTime = 2.5f; // In Seconds

    enum State { Alive, Dying, Transcending };
    State gameState = State.Alive;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (gameState == State.Alive) {
            RespondToThrustInput();
            RespondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision) {
        if(gameState != State.Alive) {
            return;
        }

        switch (collision.gameObject.tag) {
            case "Friendly":
                // DO NOTHING
                break;
            case "Finish":
                StartSuccessTransition();
                break;
            default:
                StartDeathTransition();
                break;
        }
    }

    private void StartDeathTransition() {
        gameState = State.Dying;
        audioSource.Stop();
        rigidBody.constraints = RigidbodyConstraints.None;
        audioSource.PlayOneShot(obstacleHit);
        obstacleHitParticles.Play();
        Invoke("LoadFirstScene", invokeWaitTime);
    }

    private void StartSuccessTransition() {
        gameState = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleted);
        levelCompletedParticles.Play();
        Invoke("LoadNextScene", invokeWaitTime);
    }

    private void LoadFirstScene() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene() {
        SceneManager.LoadScene(2); //TODO: Allow for more than two levels
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) {
            ApplyThrust();
        } else {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(mainEngine);
        if(!mainEngineParticles.isPlaying)
            mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {

        // Take Manual Control of Rotation
        rigidBody.freezeRotation = true;

        // If we're not pressing A and D at the same time but one of them is being pressed then...
        if (!(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))) {
            float rotationThrustThisFrame = rcsThrust * Time.deltaTime;

            // ...we check if we're pressing the A key to rotate left
            if (Input.GetKey(KeyCode.A)) {
                transform.Rotate(Vector3.forward * rotationThrustThisFrame);
            
            // ...if not we check if we're pressing the D key to rotate right
            } else if (Input.GetKey(KeyCode.D)) {
                transform.Rotate(-Vector3.forward * rotationThrustThisFrame);
            }
        }

        // Release Manual Control of Rotation
        rigidBody.freezeRotation = false;

    }
}
