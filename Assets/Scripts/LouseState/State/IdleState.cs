using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IdleState : State {
    public float minX = -5.0f, maxX = 5.0f, minY = -5.0f, maxY = 5.0f;
    public float movementSpeed = 2.0f;


    public override void enter() {
        transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
    }

    public override void exit() {
    }


    private void updatePosition() {
        Vector3 newPos = transform.position + transform.forward * movementSpeed * Time.fixedDeltaTime;

        Vector3 direction = transform.forward;
        // Out of world detection
        if (newPos.x > maxX || newPos.x < minX) {
            direction.x *= -1;
            newPos.x = newPos.x > maxX ? maxX : minX;
        }
        if (newPos.z > maxY || newPos.z < minY) {
            direction.z *= -1;
            newPos.z = newPos.z > maxY ? maxY : minY;
        }

        transform.LookAt(transform.position + direction);
        transform.position = newPos;
    }

    private void lookForTarget() {
        List<LeafBehaviour> leafs = new List<LeafBehaviour>(GameObject.FindGameObjectsWithTag("Leaf").Select(go => go.GetComponent<LeafBehaviour>()));
        leafs.RemoveAll(lb => lb == null ||!lb.isAlive);

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
        updatePosition();
        lookForTarget();
        if (stateMachine.checkForMouse()) stateMachine.transit(GetComponent<FleeState>());
    }
}
