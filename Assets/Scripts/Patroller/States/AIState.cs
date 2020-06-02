using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patroller {
    internal abstract class AIState : MonoBehaviour {
        protected AIStateMachine stateMachine;

        protected virtual void Awake() {
            stateMachine = GetComponent<AIStateMachine>();
        }

        public abstract void enter();
        public abstract void exit();
        public abstract void update();
    }
}