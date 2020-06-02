using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "AIController", menuName = "Controllers/AIController", order = 1)]
public class SolverAI : PlayerController
{
    private const int FOUND = -1;
    List<List<int>> path = new List<List<int>>();
    int currentState = 0;

    #region Controls
    public override void Start()
    {
        path = NewIda();
        if (path == null || path.Count == 0)
        {
            Debug.Log("Path is null");
            path = new List<List<int>>();
            path.Add(gboard.saveState());
        }
        currentState = path.Count - 1;
        updateState(currentState + 1, path.Count, path[currentState]);
    }

    public override void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            currentState = Mathf.Max(0, --currentState);
            updateState(currentState + 1, path.Count, path[currentState]);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            currentState = Mathf.Min(path.Count - 1, ++currentState);
            updateState(currentState + 1, path.Count, path[currentState]);
        }
    }
    private void updateState(int number, int max, List<int> state)
    {
        gboard.loadState(state);
        Debug.Log("Step " + number + " of " + max + ". Heuristic cost is " + gboard.heuristicCost);
    }
    #endregion

    #region IDA star
    private int search(List<Node> path, int g, int bound)
    {
        // node              current node (last node in current path)
        Node node = path[path.Count - 1];
        if (node.h + g > bound)
        {
            Debug.Log("node.f: " + node.f + ", bound: " + bound);
            return node.f;
        }
        if (isGoal(node))
        {
            Debug.Log("FOUND hit");
            return FOUND;
        }
        Debug.Log("Search");
        int min = int.MaxValue;
        foreach (Node succ in successors(node))
        {
            if (!path.Contains(succ))
            {
                path.Add(succ);
                int t = search(path, node.g, bound);
                if (t == FOUND)
                {
                    return FOUND;
                }
                if (t < min) min = t;
                path.RemoveAt(path.Count - 1);
            }
        }
        return min;
    }

    private bool isGoal(Node state)
    {
        for (int i = 0; i < state.state.Count; i++)
        {
            if (state.state[i] != gboard.winState[i]) return false;
        }
        return true;
    }

    private List<Node> successors(Node node)
    {
        List<Node> successors = new List<Node>();
        for (int i = 0; i < 4; i++)
        {
            gboard.loadState(node.state);
            if (gboard.moveCell((GameBoard.Direction)i))
            {
                successors.Add(new Node(gboard.saveState(), node.g + 1, gboard.heuristicCost, (GameBoard.Direction)i));
            }
        }
        return successors;
    }

    private class Node
    {
        public List<int> state;
        public int g, h;
        public GameBoard.Direction? move = null;
        // f, estimated cost of the cheapest path (root..node..goal) (f = g + h)
        public int f { get { return g + h; } }

        public Node(List<int> state, int cost, int h, GameBoard.Direction? move = null)
        {
            this.state = state;
            g = cost;
            this.h = h;
            this.move = move;
        }
    }

    private int search(Node node, int threshold, List<List<int>> path, int depth)
    {
        if (node.f > threshold)
        {
            return node.f;
        }
        if (isGoal(node))
        {
            // Backpropagate path recursively
            path.Add(node.state);
            Debug.Log("Found the solution! Depth = " + depth);
            return FOUND;
        }
        int min = int.MaxValue;
        foreach (Node succ in successors(node))
        {
            int temp = search(succ, threshold, path, depth + 1);
            if (temp == FOUND)
            {
                path.Add(node.state);
                return FOUND;
            }
            if (temp < min) min = temp;
        }
        return min;
    }

    private List<List<int>> NewIda()
    {
        Node root = new Node(gboard.saveState(), 0, gboard.heuristicCost);
        int threshold = gboard.heuristicCost;
        while (true)
        {
            int temp = search(root, threshold, path, 0);
            if (temp == FOUND)
            {
                return path;
            }
            else if (temp == int.MaxValue)
            {
                return null;
            }
            threshold = temp;
        }
    }
    #endregion
}
