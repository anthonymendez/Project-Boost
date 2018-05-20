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
    [SerializeField] float invokeWaitTime = 2.5f;

    bool collisionsDisabled = false,
         isTransitioning = false;

    Rigidbody rigidBody;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!isTransitioning) {
            RespondToThrustInput();
            RespondToRotateInput();
        }

        if (Debug.isDebugBuild) {
            RespondtoDebugInput();
        }
    }

    private void RespondtoDebugInput() {
        if (Input.GetKey(KeyCode.C))
            collisionsDisabled = !collisionsDisabled;
        if (Input.GetKey(KeyCode.L))
            LoadNextLevel();
    }

    void OnCollisionEnter(Collision collision) {
        if(isTransitioning || collisionsDisabled) {
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
        isTransitioning = true;
        audioSource.Stop();
        rigidBody.constraints = RigidbodyConstraints.None;
        audioSource.PlayOneShot(obstacleHit);
        obstacleHitParticles.Play();
        Invoke("LoadFirstLevel", invokeWaitTime);
    }

    private void StartSuccessTransition() {
        isTransitioning = true;
        audioSource.Stop();
        audioSource.PlayOneShot(levelCompleted);
        levelCompletedParticles.Play();
        Invoke("LoadNextLevel", invokeWaitTime);
    }

    private void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel() {
        int currentBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int nextBuildIndex = (currentBuildIndex + 1) % SceneManager.sceneCountInBuildSettings;
        SceneManager.LoadScene(nextBuildIndex);
    }

    private void RespondToThrustInput() {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) {
            ApplyThrust();
        } else {
            StopApplyingThrust();
        }
    }

    private void StopApplyingThrust() {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust() {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(mainEngine);
        if(!mainEngineParticles.isPlaying)
            mainEngineParticles.Play();
    }

    private void RespondToRotateInput() {

        // Stop any spin we had
        rigidBody.angularVelocity = Vector3.zero;

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

    }
}
