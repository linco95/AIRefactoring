using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Patroller {

    internal class AIPatrolState : AIState {
        private PathNode targetNode;
        public float moveSpeed = 1.0f;


        public override void enter() {
            stateMachine.agent.speed = moveSpeed;
            
            // Find closest node
            targetNode = targetNode ?? GameObject.FindGameObjectsWithTag("PathNode").OrderByDescending(node => (node.transform.position - transform.position).sqrMagnitude).Last().GetComponent<PathNode>() as PathNode;
            stateMachine.agent.SetDestination(targetNode.transform.position);
        }

        void updatePath() {
            if (!stateMachine.agent.hasPath) {
                targetNode = targetNode.nextNode;
                stateMachine.agent.SetDestination(targetNode.transform.position);
            }
        }

        

        public override void update() {
            updatePath();
            
            if (stateMachine.canSeePlayer(stateMachine.viewingRange)) {
                stateMachine.transit(GetComponent<AIChaseState>());
            }
            if(stateMachine.energy == 0.0f) {
                stateMachine.transit(GetComponent<AISleepState>());
            }
        }

        public override void exit() { }
    }
}
