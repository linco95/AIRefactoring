using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class CarControls : MonoBehaviour {

    private Vector2 controls = new Vector2();
    private Rigidbody rb;

	// Use this for initialization
	void Awake () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.MoveRotation(Quaternion.Euler(new Vector3(0, controls.y * 45 * Time.fixedDeltaTime * 2.5f, 0) + rb.rotation.eulerAngles));
        rb.AddRelativeForce(new Vector3(0,0, controls.x) * Time.fixedDeltaTime * 1500.0f);
        controls = Vector2.zero;
	}

    // [forward, brake, left, right]
    public void controlCar(Vector2 controls) {
        // Assert non negative values (replace brake with negative forward? Different brake spead?)
        for(int i = 0; i < 4; i++) {
            if (controls[i] < 0) return;
        }
        this.controls = controls;
    }


    private void Update() {
        controls.y = Input.GetAxis("Horizontal");
        controls.x =  Input.GetAxis("Vertical");
    }
}
