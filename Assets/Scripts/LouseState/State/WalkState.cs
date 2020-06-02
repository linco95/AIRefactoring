using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WalkState : State {
    public float movementSpeed = 2.0f;
    public float collectionDistance = 0.0001f;


    public override void enter() {
        transform.LookAt(stateMachine.target.transform.position);
    }

    public override void exit() {

    }


    private void lookForTarget() {
        List<LeafBehaviour> leafs = new List<LeafBehaviour>(GameObject.FindGameObjectsWithTag("Leaf").Select(go => go.GetComponent<LeafBehaviour>()));
        leafs.RemoveAll(lb => lb == null || !lb.isAlive);

        if (leafs.Count == 0) {
            // Scatter behaviour if there are no avaliable leafs and the leaf was just taken by someone else (avoids everyone just crossing each others path
            if (stateMachine.target != null && !stateMachine.target.isAlive) {
                transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                stateMachine.target = null;
            }
            return;
        }

        stateMachine.target = leafs.OrderByDescending(lb => (lb.transform.position - transform.position).sqrMagnitude).Last();
        stateMachine.transit(GetComponent<WalkState>());

    }

    public override void UpdateState() {
        lookForTarget();
        transform.position += transform.forward * movementSpeed * Time.fixedDeltaTime;
        //transform.Rotate(new Vector3(0.0f, Random.Range(-15.0f, 15.0f), 0.0f));

        if (stateMachine.target == null || !stateMachine.target.isAlive) {
            stateMachine.transit(GetComponent<IdleState>());
            return;
        }
        if ((transform.position - stateMachine.target.transform.position).sqrMagnitude <= collectionDistance * collectionDistance) {
            stateMachine.transit(GetComponent<EatState>());
        }

        if (stateMachine.checkForMouse()) stateMachine.transit(GetComponent<FleeState>());
    }
}
