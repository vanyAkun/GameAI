using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class Pathfinding : MonoBehaviour
{

    public Transform seeker;
    public Transform currentTarget;
    public GameObject markerPrefab;

    Grid grid;

    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        if (currentTarget != null)
        {
            FindPath(seeker.position, currentTarget.position);

            // Check distance between seeker and target
            if (Vector3.Distance(seeker.position, currentTarget.position) < 1.0f)
            { 
                DestroyTarget();
            }
        }
    }
    public void SetTarget(Transform newTarget)
    {
        currentTarget = newTarget;
        MoveToTarget();
    }
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node neighbour in grid.GetNeighbours(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                    {
                        //openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;

      
        ShowPathMarkers(path);
    }
    private void ShowPathMarkers(List<Node> path)
    {
        float elevationAboveTerrain = 5f;

       
        ClearPathMarkers();

        foreach (Node node in path)
        {
            Vector3 elevatedPosition = node.worldPosition + Vector3.up * elevationAboveTerrain;
            GameObject marker = Instantiate(markerPrefab, elevatedPosition, Quaternion.identity);
            marker.transform.parent = this.transform; 
        }
    }
    private void ClearPathMarkers()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("PathMarker")) 
            {
                Destroy(child.gameObject);
            }
        }
    }
    private void DestroyTarget()
    {
        if (currentTarget != null)
        {
            Destroy(currentTarget.gameObject); 
            currentTarget = null; 

            var agent = seeker.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.ResetPath(); 
                agent.isStopped = true; 
            }
        }
    }
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
    public void MoveToTarget()
    {
        if (currentTarget != null)
        {
            var agent = seeker.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.isStopped = false; 
                agent.SetDestination(currentTarget.position);
            }
        }
    }
}
