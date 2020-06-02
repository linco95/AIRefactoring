using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafBehaviour : MonoBehaviour {
    private float lifeTimer = 0.0f;
    public float lifeTime = 2.0f;
    public float variation = 0.1f;
    // Is someone already eating this? (taken by another louse?)
    public bool isAlive { get; set; }

	// Use this for initialization
	void Start () {
        isAlive = true;
        lifeTime += lifeTime * Random.Range(-variation/2, variation/2);
    }


	// Update is called once per frame
	void Update () {
        lifeTimer += Time.deltaTime;
        if(lifeTimer >= lifeTime) {
            Destroy(gameObject);
        }
	}
}
