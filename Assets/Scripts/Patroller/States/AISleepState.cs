using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patroller {
    internal class AISleepState : AIState {
        Vector3 sleepSpot;
        List<Light> lights;
        bool turnedOfLights = false;
        Animator anim;

        protected override void Awake() {
            base.Awake();
            anim = anim ?? GetComponent<Animator>();
            sleepSpot = GameObject.FindGameObjectWithTag("Sleep").transform.position;
            lights = new List<Light>(GetComponentsInChildren<Light>());

        }

        private void toggleLights() {

        }

        public override void enter() {
            lights.ForEach(light => light.color = Color.blue);
            turnedOfLights = false;
            stateMachine.agent.SetDestination(sleepSpot);
        }

        public override void exit() {
            anim.Play("Idle");
            lights.ForEach(light =>  light.color = Color.yellow);
        }

        public override void update() {
            if (!stateMachine.agent.hasPath) {
                if (!turnedOfLights) {
                    anim.Play("FlickeringLights");
                    turnedOfLights = true;
                }
                anim.speed = Random.Range(1.0f, 2.0f);
                stateMachine.energy += stateMachine.energyGain * Time.deltaTime;
            }
            if (stateMachine.energy >= 99.999f) {
                stateMachine.transit(GetComponent<AIPatrolState>());
            }
        }
    }
}
