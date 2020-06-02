using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : ScriptableObject {
    protected GameBoard gboard;


    public abstract void Update();
    public abstract void Start();

    public void setGameBoard(GameBoard board) {
        gboard = board;
    }
}
