using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "CheckIfPlayerNotBetweenWayPoints", story: "[Target] position not between [Waypoints]", category: "Conditions", id: "345da84cbca1b9920c8955f9485f2c7d")]
public partial class CheckIfPlayerBetweenWayPointsCondition : Condition
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;

    public override bool IsTrue()
    {
        if (Target.Value == null || Waypoints.Value == null || Waypoints.Value.Count < 2)
        {
            // Invalid setup (no target or insufficient waypoints)
            return false;
        }

        Vector3 targetPosition = Target.Value.transform.position;

        // Iterate through each pair of consecutive waypoints
        for (int i = 0; i < Waypoints.Value.Count - 1; i++)
        {
            Vector3 waypointA = Waypoints.Value[i].transform.position;
            Vector3 waypointB = Waypoints.Value[i + 1].transform.position;

            // Check if target is between waypointA and waypointB
            if (IsNotPointBetween(targetPosition, waypointA, waypointB))
            {
                return true;
            }
        }

        return false; // Target is not between any waypoints
    }

    private bool IsNotPointBetween(Vector3 point, Vector3 start, Vector3 end)
    {
        // Calculate the projection of the point onto the line segment
        Vector3 line = end - start;
        Vector3 toPoint = point - start;

        // Dot product to check alignment
        float dotProduct = Vector3.Dot(toPoint, line.normalized);

        // Check if the target point is within the segment bounds
        if (dotProduct < 0 || dotProduct > line.magnitude)
        {
            return false; // Target is outside the segment
        }

        // Check if the point lies on the line (allow small tolerance for floating-point errors)
        float distanceToLine = Vector3.Cross(line.normalized, toPoint).magnitude;
        return ! (distanceToLine <= 0.5f); // Adjust tolerance if needed
    }
    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
