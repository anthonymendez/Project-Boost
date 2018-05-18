﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        ProcessInput();
	}

    private void ProcessInput() {

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W)) {
            print("Thrusting!");
        }
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            print("Not Rotating!");
        } else if (Input.GetKey(KeyCode.A)) {
            print("Rotating Left!");
        } else if (Input.GetKey(KeyCode.D)) {
            print("Rotating Right!");
        }
    }
}
