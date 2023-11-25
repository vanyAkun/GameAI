using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{

    public Pathfinding pathfinder;
    private Transform selectedTarget = null;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Removed the type declaration from selectedTarget to update the class member
                selectedTarget = hit.transform;
                if (selectedTarget.CompareTag("Target"))
                {
                    pathfinder.SetTarget(selectedTarget);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && selectedTarget != null)
        {
            // Right click functionality
            pathfinder.MoveToTarget();
        }
    }
}

