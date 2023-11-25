using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelector : MonoBehaviour
{

    public Pathfinding pathfinder; // Reference to the pathfinding script

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        { // On left mouse click
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Transform selectedTarget = hit.transform;
                // Check if the clicked object is a valid target
                if (selectedTarget.CompareTag("Target"))
                { // Make sure targets have the tag "Target"
                    pathfinder.SetTarget(selectedTarget);
                }
            }
        }
    }

}