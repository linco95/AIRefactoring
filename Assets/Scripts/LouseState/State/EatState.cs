using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatState : State {

    private float sleepTimer = 0.0f;
    public ParticleSystem eatingParticles;
    public float eatingTime = 0.0f;
    private ParticleSystem psInstance;
    private Color prevColor;

    public override void enter() {
        sleepTimer = eatingTime;
        if (eatingParticles != null) {
            psInstance = Instantiate(eatingParticles, transform.position, Quaternion.identity, null);
        }
        stateMachine.target.isAlive = false;

        gameObject.GetComponentInChildren<Renderer>().material.color = Color.black;
    }

    public override void exit() {
        // TODO: null check?
        Destroy(psInstance);
        // TODO: Destroy leaf if timer == 0
        if (sleepTimer <= 0 && stateMachine.target != null) {
            Destroy(stateMachine.target.gameObject);
        } else if(stateMachine.target != null) {
            stateMachine.target.isAlive = true;
        }
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;
    }

    public override void UpdateState() {
        sleepTimer -= Time.deltaTime;
        if(sleepTimer <= 0) {
            stateMachine.transit(GetComponent<IdleState>());
        }
        if (stateMachine.checkForMouse()) stateMachine.transit(GetComponent<FleeState>());
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
