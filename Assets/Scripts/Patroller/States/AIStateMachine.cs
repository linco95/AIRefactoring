using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Patroller {

    internal class AIStateMachine : MonoBehaviour {
        AIState activeState;
        public GameObject playerObj;
        public NavMeshAgent agent;
        public float fieldOfView = 75.0f;
        public float viewingRange = 100.0f;


        public float energy = 100.0f;
        public float energyDrain = 1.0f;
        public float energyGain = 2.0f;

        void Update() {

            energy += Time.deltaTime * (activeState is AISleepState ? 0 : -energyDrain);
            energy = Mathf.Clamp(energy, 0.0f, 100.0f);

            activeState.update();
        }

        private void Awake() {
            playerObj = playerObj ?? GameObject.FindGameObjectWithTag("Player");
            agent = agent ?? GetComponent<NavMeshAgent>();
        }

        private void Start() {
            playerObj = playerObj ?? GameObject.FindGameObjectWithTag("Player");


            activeState = GetComponent<AIPatrolState>();
            activeState.enter();
        }

        public void transit(AIState newState) {
            activeState.exit();
            activeState = newState;
            activeState.enter();
        }


        public bool canSeePlayer(float? range = null) {
            var dirToPlayer = playerObj.transform.position - transform.position;
            range = range ?? dirToPlayer.magnitude;
            RaycastHit hit;
            Ray ray = new Ray(transform.position, dirToPlayer.normalized);
            float angleToPlayer = Vector3.Angle(transform.forward, dirToPlayer);
            if (angleToPlayer > fieldOfView / 2) return false; // No need to continue if not in field of view
            Physics.Raycast(ray, out hit, range.Value);
            if (hit.collider.tag.Equals("Player")) {
                return true;
            }
            return false;
        }
    }
}