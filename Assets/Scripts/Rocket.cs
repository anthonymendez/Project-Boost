using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;

	// Use this for initialization
	void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInput();
	}

    private void ProcessInput() {

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) {
            rigidBody.AddRelativeForce(Vector3.up);
            if(!audioSource.isPlaying)
                audioSource.Play();
        } else {
            audioSource.Pause();
        }
        if (!(Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))) {
            if (Input.GetKey(KeyCode.A)) {
                transform.Rotate(Vector3.forward);
            } else if (Input.GetKey(KeyCode.D)) {
                transform.Rotate(-Vector3.forward);
            }
        }
    }
}
