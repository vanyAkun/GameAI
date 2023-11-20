using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentController : MonoBehaviour
{
    public float spawnRadius = 10f;

    void Start()
    {
        Vector3 spawnPosition = transform.position; // Current position as the desired position
        if (FindNearestNavMeshPoint(spawnPosition, out Vector3 nearestPosition, spawnRadius))
        {
            transform.position = nearestPosition;
            // Additional setup for the agent
        }
        else
        {
            Debug.LogError("Failed to find a valid position on the NavMesh!");
            // Handle the error (e.g., retry spawning, destroy the agent, etc.)
        }
    }

    bool FindNearestNavMeshPoint(Vector3 sourcePosition, out Vector3 result, float maxDistance)
    {
        if (NavMesh.SamplePosition(sourcePosition, out NavMeshHit hit, maxDistance, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
