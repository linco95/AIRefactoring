using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GameManager2 : MonoBehaviour {
    public GameObject leafPrefab, lousePrefab;

    public int numberOfLice = 10;
    public int leafsPerSpawn = 20;

    public float minX = 0.0f, maxX = 4.0f, minY = 0.0f, maxY = 4.0f;

    public Vector2 spawnInterval;
    private float nextSpawn;
    private float timeSinceLastSpawn = 0.0f;

    // Use this for initialization
    void Start() {
        spawnObjectInGame(lousePrefab, numberOfLice);
        updateNextSpawn();
    }

    void updateNextSpawn() {
        nextSpawn = Random.Range(spawnInterval.x, spawnInterval.y);
    }

    void spawnObjectInGame(GameObject gobj, int amount) {
        for (int i = 0; i < amount; i++) {
            Vector3 spawnCoord = new Vector3(Random.Range(minX, maxX), 0.0f, Random.Range(minY, maxY));
            Quaternion spawnRotation = Quaternion.Euler(0.0f, Random.Range(0.0f, 360.0f), 0.0f);
            Instantiate(gobj, spawnCoord, spawnRotation, null);
        }
    }


    // Update is called once per frame
    void Update() {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= nextSpawn) {
            spawnObjectInGame(leafPrefab, leafsPerSpawn);

            timeSinceLastSpawn = 0.0f;
        }

        // Debug
        if (Input.GetKey(KeyCode.Space)) {
            foreach(var go in GameObject.FindGameObjectsWithTag("Leaf")) {
                go.GetComponent<LeafBehaviour>().isAlive = false;
            }
        }
    }



}
