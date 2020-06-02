using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

using Random = UnityEngine.Random;
//using System.Linq;
//using System.Linq.Expressions;

public class GameBoard : MonoBehaviour {

    public GameObject cellPrefab;
    public float offset = 2.5f;

    public List<int> winState;
    private List<GameObject> cells;
    private int emptySpot { get { return state.IndexOf(0); } } // refactoring set to do nothing
    private List<int> state;
    public enum Direction {
        NORTH = 0,
        WEST,
        SOUTH,
        EAST,
        NONE
    }

    /// <summary>
    /// Create a board with size*size cells
    /// </summary>
    /// <param name="size">Width of the gameboard</param>
    /// <returns></returns>
    public bool createBoard(int size) {
        print("Creating board with size: " + size);
        cells = new List<GameObject>();
        state = new List<int>();

        for (int i = 0; i < size * size; i++) {
            GameObject newCell = Instantiate(cellPrefab, new Vector3(i % size, -i / size, 0.0f) * offset, Quaternion.identity, transform);
            newCell.GetComponentInChildren<TextMesh>().text = (i + 1).ToString();
            cells.Add(newCell);
            state.Add(i + 1);
        }
        state.RemoveAt(state.Count - 1);
        state.Add(0);
        state.TrimExcess();
        updateCells();
        winState = new List<int>(state);
        return true;
    }
    public void Update() {

    }
    /// <summary>
    /// Shuffle the board by randomly doing "steps" moves
    /// </summary>
    /// <param name="steps">Number of random moves to shuffle</param>
    public void shuffleBoard(int steps) {
        //randomShuffle();
        //string output = "";
        for (int i = 0; i < steps; ++i) {
            while (!moveCell((Direction)Random.Range(0, 4))) { }
            //output += heuristicCost().ToString() + Environment.NewLine;
        }
        //File.WriteAllText("dump.log", output);
    }

    public void randomShuffle() {
        List<int> pickedValues = new List<int>();

        for (int i = 0; i < state.Count; ++i) {
            int nextIndex;
            do {
                nextIndex = Random.Range(0, state.Count);
            } while (pickedValues.Contains(nextIndex));
            state[i] = nextIndex;
            pickedValues.Add(nextIndex);

            //List<string> asd = new List<string>();
            //asd.Where(item => item.StartsWith("hej")).ToList();
        }
    }
    public bool moveCell(Direction dir) {
        if (dir == Direction.NONE) return true;
        // Check if valid move
        // move (swap) and return true if valid, return false if invalid move
        Vector2 coords = indexToCoords(emptySpot) + directionToVec2(dir);
        if (coords.x >= Mathf.Sqrt(state.Count) || coords.y >= Mathf.Sqrt(state.Count) || coords.x < 0 || coords.y < 0) return false;
        swapCells(coords);
        return true;
    }

    public List<int> saveState() {
        // return current state
        return new List<int>(state);
    }

    // Returns the heuristic cost to solve this state (sum of all cell's heuristic costs).
    public int heuristicCost {
        get {
            int cost = 0;
            for (int i = 0; i < state.Count; i++) {
                cost += heuristicCellCost(state[i], i);
            }
            return cost;
        }
    }

    // Returns the heuristic cost of one cell to get to it's desired position
    private int heuristicCellCost(int desiredIndex, int currentIndex) {
        if (desiredIndex == 0) {
            return manhattanDist(indexToCoords(currentIndex), indexToCoords(state.Count - 1));
        }
        return manhattanDist(indexToCoords(currentIndex), indexToCoords(desiredIndex - 1));
    }

    private void updateCells() {
        //  Update cells! (Invariant: state ~ cells && cells[emptySpot].active == false)
        for (int i = 0; i < state.Count; ++i) {
            cells[i].GetComponentInChildren<TextMesh>().text = state[i].ToString();
            cells[i].SetActive(true);
            float heuristic = heuristicCellCost(state[i], i) / Mathf.Sqrt(2 * Mathf.Sqrt(state.Count));
            cells[i].GetComponent<MeshRenderer>().material.color = new Color(heuristic, 1.0f - heuristic, 0.0f);
            //cells[i].GetComponent<MeshRenderer>().color = Color.red;
            if (state[i] == 0) {
                cells[i].SetActive(false);
            }
        }
    }

    public void loadState(List<int> newState) {
        // set the new state
        state = new List<int>(newState);
        updateCells();
    }

    public void printState() {
        int size = (int)Mathf.Sqrt(state.Count);
        string output = "";
        for (int i = 0; i < state.Count; ++i) {
            if (i % size == 0)
                output += '\n';
            output += state[i] + " ";
        }
        print(output + '\n');
    }

    // END Interface (Private functions)
    private void swapCells(Vector2 coords) {
        int targetValue = state[emptySpot];
        state[emptySpot] = state[coordsToIndex(coords)];
        state[coordsToIndex(coords)] = targetValue;
        updateCells();
    }

    private static Vector2 directionToVec2(Direction dir) {
        switch (dir) {
            case Direction.NORTH:
                return new Vector2(0.0f, -1.0f);
            case Direction.EAST:
                return new Vector2(1.0f, 0.0f);
            case Direction.WEST:
                return new Vector2(-1.0f, 0.0f);
            case Direction.SOUTH:
                return new Vector2(0.0f, 1.0f);
        }
        return Vector2.zero;
    }

    private int coordsToIndex(Vector2 coord) {
        int size = (int)Mathf.Sqrt(state.Count);
        return (int)coord.x % state.Count + (int)coord.y * size;
    }
    private Vector2 indexToCoords(int index) {
        int size = (int)Mathf.Sqrt(state.Count);
        return new Vector2(index % size, (int)(index / size));
    }

    public int manhattanDist(Vector2 from, Vector2 to) {
        from -= to;
        return (int)(Mathf.Abs(from.x) + Mathf.Abs(from.y));
    }
}
