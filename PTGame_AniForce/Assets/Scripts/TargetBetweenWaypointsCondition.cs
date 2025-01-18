using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Target between Waypoints", story: "[Target] between [Waypoints]", category: "Conditions", id: "2e112a3a0cd5069c096c62d867821684")]
public partial class TargetBetweenWaypointsCondition : Condition
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
            if (IsPointBetween(targetPosition, waypointA, waypointB))
            {
                return true;
            }
        }

        return false; // Target is not between any waypoints
    }

    private bool IsPointBetween(Vector3 point, Vector3 start, Vector3 end)
    {
        if (start.x >= end.x)
        {
            if (point.x > start.x)
                return false;
            if (point.x < end.x)
                return false;

            if (Mathf.Abs(point.y - start.y) <= 3f)
            {
                return true;
            }

            return false;
        }

        else if (start.x < end.x)
        {
            return IsPointBetween(point, end, start);
        }

        return false;
    }
    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
