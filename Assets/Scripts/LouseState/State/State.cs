using System.Collections;
using UnityEngine;

[RequireComponent(typeof(StateMachine))]
public abstract class State : MonoBehaviour {

    protected StateMachine stateMachine;

    private void Awake() {
        stateMachine = GetComponent<StateMachine>();
    }

    // Use this for initialization
    public abstract void enter();

    public abstract void exit();

    // Update is called once per frame
    public abstract void UpdateState();
}
