using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    State activeState;
    public LeafBehaviour target;
    public float minFleeDistance = 1.0f;

    private void Start() {
        activeState = GetComponent<IdleState>();
        activeState.enter();
    }

    private void Update() {
        transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        activeState.UpdateState();
    }

    public void transit(State newState) {
        activeState.exit();
        activeState = newState;
        activeState.enter();
    }

    // Global transition (from any state)
    public bool checkForMouse() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePos.y = 0.0f;
        if ((mousePos - transform.position).sqrMagnitude <= minFleeDistance * minFleeDistance) {
            return true;
        }
        return false;
    }
}
