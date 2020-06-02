using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patroller {

    internal class AILostPlayerState : AIState {
        public float lookingAngle = 45.0f;
        public AnimationCurve lostPlayerRotation;
        public float playbackTime = 4.0f;
        private float currentTime = 0.0f;
        private float startRotation = 0.0f;

        protected override void Awake() {
            base.Awake();
            lostPlayerRotation = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(33.0f, lookingAngle), new Keyframe(66.0f, -lookingAngle), new Keyframe(100.0f, 0.0f));
        }

        public override void enter() {
            stateMachine.agent.isStopped = true;
            startRotation = transform.rotation.eulerAngles.y;
            currentTime = 0.0f;
        }

        public override void exit() {
            stateMachine.agent.isStopped = false;
        }

        public override void update() {
            currentTime += Time.deltaTime;

            transform.rotation = Quaternion.Euler(0.0f, startRotation + lostPlayerRotation.Evaluate(currentTime/playbackTime * 100), 0.0f);

            if (currentTime >= playbackTime) {
                stateMachine.transit(GetComponent<AIPatrolState>());
            }
            if (stateMachine.canSeePlayer(stateMachine.viewingRange)) {
                stateMachine.transit(GetComponent<AIChaseState>());
            }
        }
    }
}
