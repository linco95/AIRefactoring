using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State {
    public float movementSpeed = 2.0f;

    public override void enter() {
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.cyan;

    }

    public override void exit() {
        gameObject.GetComponentInChildren<Renderer>().material.color = Color.red;

    }

    private void updateMousPos() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePos.y = 0.0f;
        transform.LookAt(mousePos);
        transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
    }

    public override void UpdateState() {
        updateMousPos();
        transform.position += transform.forward * movementSpeed * Time.fixedDeltaTime;

        if (!stateMachine.checkForMouse()) stateMachine.transit(GetComponent<IdleState>());
    }
}
