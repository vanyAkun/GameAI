using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{
    public Pathfinding pathfinder;
    private Transform selectedTarget = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click for selecting target
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform hitTarget = hit.transform;
                if (hitTarget.CompareTag("Target"))
                {
                    selectedTarget = hitTarget;
                    pathfinder.SetTarget(selectedTarget);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedTarget != null) // Right click for moving to target
        {
            pathfinder.MoveToTarget();
        }
    }
}


