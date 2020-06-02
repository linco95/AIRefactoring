using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameBoard gBoard;
    public PlayerController controller;
    public int boardWidth = 4;
    public int shuffleSteps = 1000;

    void Start()
    {
        gBoard = Instantiate(gBoard, Vector3.zero, Quaternion.identity, transform);
        gBoard.createBoard(boardWidth);
        gBoard.shuffleBoard(shuffleSteps);

        controller.setGameBoard(gBoard);
        controller.Start();
    }

    public void Update()
    {
        // Check for input
        controller.Update();
    }
}
