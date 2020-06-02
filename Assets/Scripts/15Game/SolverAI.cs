using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetwork;
using System.IO;

[CreateAssetMenu(fileName = "AIController", menuName = "Controllers/AIController", order = 1)]
public class SolverAI : PlayerController {
    //private int debugCheck = 0;
    private const int FOUND = -1;
    List<List<int>> path = new List<List<int>>();
    int currentState = 0;
    
    #region Controls
    public override void Start() {
        path = NewIda();
        if (path == null || path.Count == 0) {
            Debug.Log("Path is null");
            path = new List<List<int>>();
            path.Add(gboard.saveState());
        }
       /* gboard.printState();
        foreach (Node succ in successors(new Node(path[0], 0, 0))) {
            path.Add(succ.state);
        }
        //path = searchedInOrder;*/
        currentState = path.Count - 1;
        updateState(currentState + 1, path.Count, path[currentState]);
    }

    public override void Update() {
        if (Input.GetKeyUp(KeyCode.A)) {
            currentState = Mathf.Max(0, --currentState);
            updateState(currentState + 1, path.Count, path[currentState]);
        }
        else if (Input.GetKeyUp(KeyCode.D)) {
            currentState = Mathf.Min(path.Count - 1, ++currentState);
            updateState(currentState + 1, path.Count, path[currentState]);
        }
        //if (Input.GetKeyUp(KeyCode.Space)) {
        //    gboard.loadState(gboard.winState);
        //    Debug.Log(gboard.heuristicCost());
        //}
    }
    private void updateState(int number, int max, List<int> state) {
        gboard.loadState(state);
        Debug.Log("Step " + number + " of " + max + ". Heuristic cost is " + gboard.heuristicCost);
    }
    #endregion


    #region Neural Network

    private void savePath(List<Node> path) {
        string outputstr = "";
        string filename = "AITrainingSet.nnds";
        for(int i = 0; i < path.Count - 1; ++i) {
            path[i].state.ForEach(element => outputstr += element + ", ");
            outputstr += path[i + 1].move + '\n';
        }
        File.AppendAllText(filename, outputstr);
    }


    //NeuralNet ann = new NeuralNet(16, 3, 4);
    
    

    #endregion


    #region almost Astar

    class AstarNode : Node {
        public Node cameFrom { get { return cameFrom; } set { cameFrom = value; } }

        public AstarNode(List<int> state, int cost, int heuristic, Node cameFrom)  : base(state, cost, heuristic) {
            this.cameFrom = cameFrom;
        }
    }

    private List<int> astar() {
        // The set of nodes already evaluated
        //List<AstarNode> closedSet = new List<AstarNode>();
        // The set of currently discovered nodes that are not evaluated yet.
        List<AstarNode> openSet = new List<AstarNode>();
        // Initially, only the start node is known.
        openSet.Add(new AstarNode(gboard.saveState(), 0, gboard.heuristicCost, null));

        while(openSet.Count != 0) {
            AstarNode current = openSet[openSet.Count - 1];

        }

        return null;
    }

    /*

             function A*(start, goal)
        // The set of nodes already evaluated
        closedSet := {}

        // The set of currently discovered nodes that are not evaluated yet.
        // Initially, only the start node is known.
        openSet := {start}

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        cameFrom := the empty map

        // For each node, the cost of getting from the start node to that node.
        gScore := map with default value of Infinity

        // The cost of going from start to start is zero.
        gScore[start] := 0

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        fScore := map with default value of Infinity

        // For the first node, that value is completely heuristic.
        fScore[start] := heuristic_cost_estimate(start, goal)

        while openSet is not empty
            current := the node in openSet having the lowest fScore[] value
            if current = goal
                return reconstruct_path(cameFrom, current)

            openSet.Remove(current)
            closedSet.Add(current)

            for each neighbor of current
                if neighbor in closedSet
                    continue		// Ignore the neighbor which is already evaluated.

                if neighbor not in openSet	// Discover a new node
                    openSet.Add(neighbor)

                // The distance from start to a neighbor
                tentative_gScore := gScore[current] + dist_between(current, neighbor)
                if tentative_gScore >= gScore[neighbor]
                    continue		// This is not a better path.

                // This path is the best until now. Record it!
                cameFrom[neighbor] := current
                gScore[neighbor] := tentative_gScore
                fScore[neighbor] := gScore[neighbor] + heuristic_cost_estimate(neighbor, goal) 

        return failure

    function reconstruct_path(cameFrom, current)
        total_path := [current]
        while current in cameFrom.Keys:
            current := cameFrom[current]
            total_path.append(current)
        return total_path
             */
    #endregion

    #region IDA star

    private List<List<int>> Ida() {
        // path              current search path (acts like a stack)
        List<Node> path = new List<Node>();
        // Push root to path
        path.Add(new Node(gboard.saveState(), 0, gboard.heuristicCost));

        int bound = path[path.Count - 1].h;
        // Our choosen shuffle method guarantees that there exists a path
        while (true) {
            int t = search(path, 0, bound);
            if (t == FOUND) {
                Debug.Log("FOUND IT!");
                savePath(path);
                #region Convert path to correct datatype
                List<List<int>> retVal = new List<List<int>>();
                foreach (Node nod in path) {
                    retVal.Add(nod.state);
                }
                #endregion
                return retVal; // Return path (List with states in the applied order)
            }
            else if (t == int.MaxValue) {
                return null;
            }
            bound = t;
            // Anti infinite looping DEBUG
            if (bound > 100) {
                Debug.Log("Failed to find solution");
                return null;
            }
        }
        /*
     h(node)           estimated cost of the cheapest path (node..goal) ( gboard.heuristicCost())
     cost(node, succ)  step cost function (=0)

     ida_star(root)    return either NOT_FOUND or a pair with the best path and its cost

     procedure ida_star(root)
       bound := h(root)
       path := [root]
       loop
         t := search(path, 0, bound)
         if t = FOUND then return (path, bound)
         if t = ∞ then return NOT_FOUND
         bound := t
       end loop
     end procedure


     */

    }

    private int search(List<Node> path, int g, int bound) {
        // node              current node (last node in current path)
        Node node = path[path.Count - 1];
        if (node.h + g > bound) {
            Debug.Log("node.f: " + node.f + ", bound: " + bound);
            return node.f;
        }
        if (isGoal(node)) {
            Debug.Log("FOUND hit");
            return FOUND;
        }
        Debug.Log("Search");
        int min = int.MaxValue;
        foreach (Node succ in successors(node)) {
            if (!path.Contains(succ)) {
                path.Add(succ);
                int t = search(path, node.g, bound);
                if (t == FOUND) {
                    return FOUND;
                }
                if (t < min) min = t;
                path.RemoveAt(path.Count - 1);
            }
        }
        /*   function search(path, g, bound)
       node := path.last
       f := g + h(node)
       if f > bound then return f
       if is_goal(node) then return FOUND
       min := ∞
       for succ in successors(node) do
         if succ not in path then
           path.push(succ)
           t := search(path, g + cost(node, succ), bound)
           if t = FOUND then return FOUND
           if t < min then min := t
           path.pop()
         end if
       end for
       return min
     end function
     */
        return min;
    }

    //  is_goal(node)     goal test
    private bool isGoal(Node state) {
        for(int i = 0;  i < state.state.Count; i++) {
            if (state.state[i] != gboard.winState[i]) return false;
        }
        return true;
    }



    private List<Node> successors(Node node) {
        List<Node> successors = new List<Node>();

        for (int i = 0; i < 4; i++) {
            gboard.loadState(node.state);
            if (gboard.moveCell((GameBoard.Direction)i)) {
                successors.Add(new Node(gboard.saveState(), node.g + 1, gboard.heuristicCost, (GameBoard.Direction)i));
            }
        }
        return successors;
    }

    private class Node {
        public List<int> state;
        public int g, h;
        public GameBoard.Direction? move = null;
        // f, estimated cost of the cheapest path (root..node..goal) (f = g + h)
        public int f { get { return g + h; } }

        public Node(List<int> state, int cost, int h, GameBoard.Direction? move = null) {
            this.state = state;
            g = cost;
            this.h = h;
            this.move = move;
        }
    }

    private int search(Node node, int threshold, List<List<int>> path, int depth) {
        //if (++debugCheck > 1000000) {
        //    Debug.Log("Infinite loop?");
        //  //  return -1;
        //}
        if (node.f > threshold) {
            return node.f;
        }
        if (isGoal(node)) {
            // Backpropagate path recursively
            path.Add(node.state);
            Debug.Log("Found the solution! Depth = " + depth);
            return FOUND;
        }
        int min = int.MaxValue;
        foreach (Node succ in successors(node)) {
            int temp = search(succ, threshold, path, depth + 1);
            if(temp == FOUND) {
                path.Add(node.state);
                return FOUND;
            }
            if (temp < min) min = temp;
        }
        return min;

        /*
         *  public static Result Search(Node node, double g, double max, int sysout_level) {
        Result result;
        double f = node.heuristic + g;

        for (int i = 0; i < sysout_level; i++) {
            System.out.print("-");
        }
        System.out.println(node);

        if (f > max) {
            result = new Result();
            result.type = 2;
            result.result1 = new Double(f);
            return result;
        }

        if (node.heuristic == 0) {
            result = new Result();
            result.type = 1;
            //result.result1 = new Double(f);
            System.out.println("FOUND - use stack to reverse the order :)");
            System.out.println(String.format("Total time: %.0f minutes", g));
            System.out.println("Cities:");
            System.out.println(node);
            return result;
        }

        double min = Double.MAX_VALUE;
        for (Edge e : node.adjacencies) {
            Result result1 = Search(e.node, g + e.cost, max, sysout_level + 1);
            if (result1.type == 1) {
                System.out.println(node);
                return result1;
            }
            else if (result1.type == 2) {
                double newMin = (Double)result1.result1;
                if (newMin < min) {
                    min = newMin;
                }
            }
        }

        result = new Result();
        result.type = 2;
        result.result1 = new Double(min);
        return result;
    }
         * 
         * 
         * 
         * 
         * 
         * 
         * 
          f=g+heuristic(node);
        if(f>threshold)             //greater f encountered
                 return f;
        if(node==Goal)               //Goal node found
                 return FOUND;
        integer min=MAX_INT;     //min= Minimum integer
        foreach(tempnode in nextnodes(node))
        {

        //recursive call with next node as current node for depth search
        integer temp=search(tempnode,g+cost(node,tempnode),threshold);  
        if(temp==FOUND)            //if goal found
        return FOUND;
        if(temp<min)     //find the minimum of all ‘f’ greater than threshold encountered
        min=temp;

        }
        return min;  //return the minimum ‘f’ encountered greater than threshold*/

    }

    private List<List<int>> NewIda() {
        Node root = new Node(gboard.saveState(), 0, gboard.heuristicCost);
        int threshold = gboard.heuristicCost;
        while(true) {
            int temp = search(root, threshold, path, 0);
            if(temp == FOUND) {
                return path;
            }
            else if (temp == int.MaxValue) {
                return null;
            }
            threshold = temp;
        }
    }

    /*
public class IDAStar {
public static void IDAStar(Node node) {
        double max = node.heuristic;
        while (true) {
            System.out.println("==========Iteracija==========");
            Result result = Search(node, 0, max, 0);
            if (result.type == 1) {
                //System.out.println(node);
                return;
            }
            else if (result.type == 2) {
                double min = (Double)result.result1;
                if (min == Double.MAX_VALUE) {
                    System.out.println("Not found");
                    return;
                }
            }
            max = (Double)result.result1;
            System.out.println("=============================");
        }
    }

   
}*/
    /*
         root=initial node;
    Goal=final node;
    function IDA*()                                             //Driver function
    {

    threshold=heuristic(Start);
    while(1)             //run for infinity
    {

    integer temp=search(Start,0,threshold); //function search(node,g score,threshold)
    if(temp==FOUND)                                 //if goal found
             return FOUND;                                             
    if(temp== ∞)                               //Threshold larger than maximum possible f value
             return;                               //or set Time limit exceeded
    threshold=temp;

    }

    }
    function Search(node, g, threshold)              //recursive function
    {

    f=g+heuristic(node);
    if(f>threshold)             //greater f encountered
             return f;
    if(node==Goal)               //Goal node found
             return FOUND;
    integer min=MAX_INT;     //min= Minimum integer
    foreach(tempnode in nextnodes(node))
    {

    //recursive call with next node as current node for depth search
    integer temp=search(tempnode,g+cost(node,tempnode),threshold);  
    if(temp==FOUND)            //if goal found
    return FOUND;
    if(temp<min)     //find the minimum of all ‘f’ greater than threshold encountered                                min=temp;

    }
    return min;  //return the minimum ‘f’ encountered greater than threshold

    }
    function nextnodes(node)
    {
                 return list of all possible next nodes from node;
    }*/
    #endregion
}
