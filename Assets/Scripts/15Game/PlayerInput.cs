using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "HumanController", menuName = "Controllers/HumanController", order = 1)]
public class PlayerInput : PlayerController
{

    public override void Update()
    {

        if (Input.GetMouseButtonUp(0))
        {
            gboard.shuffleBoard(1);
        }

        GameBoard.Direction moveDir = GameBoard.Direction.NONE;
        if (Input.GetKeyUp(KeyCode.W))
        {
            moveDir = GameBoard.Direction.NORTH;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            moveDir = GameBoard.Direction.SOUTH;
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            moveDir = GameBoard.Direction.WEST;
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            moveDir = GameBoard.Direction.EAST;
        }
        if (moveDir != GameBoard.Direction.NONE)
        {
            string outputstr = "";
            gboard.saveState().ForEach(a => outputstr += a + ", ");
            outputstr += moveDir.ToString();
            outputstr += '\n';
            File.AppendAllText("TrainingData.dat", outputstr);
        }
        gboard.moveCell(moveDir);

    }
    public override void Start()
    {

    }
}
