using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    // IDA*

    public GameBoard gBoard;
    public PlayerController controller;
    public int boardWidth = 4;
    public int shuffleSteps = 1000;

    void Awake() {
        //Random.InitState(0);
    }

    // Use this for initialization
    void Start () {
        gBoard = Instantiate(gBoard, Vector3.zero, Quaternion.identity, transform);
        gBoard.createBoard(boardWidth);
        gBoard.shuffleBoard(shuffleSteps);

        controller.setGameBoard(gBoard);
        controller.Start();
    }

    private void testLoadState() {
        List<int> testState = new List<int>();
        testState.Add(3);
        testState.Add(7);
        testState.Add(9);
        testState.Add(2);
        testState.Add(4);
        testState.Add(1);
        testState.Add(8);
        testState.Add(10);
        testState.Add(15);
        testState.Add(0);
        testState.Add(5);
        testState.Add(12);
        testState.Add(11);
        testState.Add(6);
        testState.Add(13);
        testState.Add(14);
        gBoard.loadState(testState);
    }

	// Update is called once per frame
	public void Update () {
        // Check for input
        controller.Update();
	}
}
