using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Patroller {
    internal class AIChaseState : AIState {

        public float moveSpeed = 4.0f;
        public float energyMult = 2.0f;

        private float prevSpeed;

        public override void enter() {
            prevSpeed = stateMachine.agent.speed;
            stateMachine.agent.speed = moveSpeed;
            stateMachine.energyDrain *= energyMult;
            new List<Light>(GetComponentsInChildren<Light>()).ForEach(light => light.color = Color.red);
        }

        public override void exit() {
            new List<Light>(GetComponentsInChildren<Light>()).ForEach(light => light.color = Color.yellow);
            stateMachine.agent.speed = prevSpeed;
            stateMachine.energyDrain /= energyMult; 
        }

        public override void update() {
            // TODO: Check for LOS or range of player
            if (stateMachine.canSeePlayer()) {
                stateMachine.agent.SetDestination(stateMachine.playerObj.transform.position);
            }
            else if (!stateMachine.agent.hasPath) {
                stateMachine.transit(GetComponent<AILostPlayerState>());
            }
            // Check for game over

            if((stateMachine.playerObj.transform.position - transform.position).magnitude <= 2.0f) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            if (stateMachine.energy == 0.0f) {
                stateMachine.transit(GetComponent<AISleepState>());
            }
        }
    }
}
