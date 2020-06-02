using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LouseBehaviour : MonoBehaviour {
    public float movementSpeed = 2.0f;
    public float minX = -5.0f, maxX = 5.0f, minY = -5.0f, maxY = 5.0f;
    public float collectionDistance = 0.0001f, eatingTime = 0.0f;
    private LeafBehaviour target;
    public ParticleSystem eatingParticles;
    private float sleepTimer = 0.0f;
    public float minFleeDistance = 1.0f;
    // Use this for initialization
    void Start() {
        updateTarget();
    }

    void updateTarget() {
        // TODO: updated priorityqueue here instead of linear seach would make this function constant instead of linear (relative to amount of leafs in scene) which would be a performance improvement. Might increase dependencies though (someone would have to have this list updated)
        // This function needs to be called every update as someone might have eaten the target. This could be avoided by using events instead that would inform every louse that is chasing that leaf that it has been eaten and they need to fetch a new target
        List<LeafBehaviour> leafs = new List<LeafBehaviour>(GameObject.FindGameObjectsWithTag("Leaf").Select(go => go.GetComponent<LeafBehaviour>()));
        leafs.RemoveAll(lb =>  lb != null && !lb.isAlive);

        if (leafs.Count == 0) {
            // Scatter behaviour if there are no avaliable leafs and the leaf was just taken by someone else (avoids everyone just crossing each others path
            if (target != null && !target.isAlive) {
                transform.rotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
                target = null;
            }
            return;
        }
        leafs.RemoveAll(lb => lb == null);

        target = leafs.OrderByDescending(lb => (lb.transform.position - transform.position).sqrMagnitude).Last();
        transform.LookAt(target.transform.position);
    }

    void updatePosition() {
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

    void checkIfAtTarget() {
        if(target != null && (target.transform.position - transform.position).sqrMagnitude < collectionDistance){
            target.isAlive = false;
            Destroy(target.gameObject);
            sleepTimer = eatingTime;
            if(eatingParticles != null) {
                Destroy(Instantiate(eatingParticles, transform.position, Quaternion.identity, null), eatingTime);
            }
        }
    }

    private void checkForMouse() {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.y;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mousePosition);
        mousePos.y = 0.0f;
        if ((mousePos - transform.position).sqrMagnitude <= minFleeDistance * minFleeDistance) {
            transform.LookAt(mousePos);
            transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (sleepTimer > 0.0f) {
            sleepTimer -= Time.deltaTime;
            return;
        }
        updateTarget();
        checkForMouse();
        updatePosition();
        checkIfAtTarget();
    }


}
