using System;
using System.Collections.Generic;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using UnityEngine.AI;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Navigate to Target 2", story: "[Agent] navigates to [Target]", category: "Action/Navigation", id: "9f73d2cb91e487655291198ee92c623b")]
public partial class NavigateToTarget2Action : Action
{
        [SerializeReference] public BlackboardVariable<GameObject> Agent;
        [SerializeReference] public BlackboardVariable<GameObject> Target;
        [SerializeReference] public BlackboardVariable<float> Speed = new BlackboardVariable<float>(1.0f);
        [SerializeReference] public BlackboardVariable<float> DistanceThreshold = new BlackboardVariable<float>(0.2f);
        [SerializeReference] public BlackboardVariable<string> AnimatorSpeedParam = new BlackboardVariable<string>("SpeedMagnitude");

        // This will only be used in movement without a navigation agent.
        [SerializeReference] public BlackboardVariable<float> SlowDownDistance = new BlackboardVariable<float>(1.0f);
        
        [SerializeReference] public BlackboardVariable<List<GameObject>> Waypoints;


        private NavMeshAgent m_NavMeshAgent;
        private Animator m_Animator;
        private float m_PreviousStoppingDistance;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_ColliderAdjustedTargetPosition;
        private float m_ColliderOffset;

        protected override Status OnStart()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            return Initialize();
        }

        protected override Status OnUpdate()
        {
            if (Agent.Value == null || Target.Value == null)
            {
                return Status.Failure;
            }

            // Check if the target position has changed.
            bool boolUpdateTargetPosition = !Mathf.Approximately(m_LastTargetPosition.x, Target.Value.transform.position.x) || !Mathf.Approximately(m_LastTargetPosition.y, Target.Value.transform.position.y) || !Mathf.Approximately(m_LastTargetPosition.z, Target.Value.transform.position.z);
            if (boolUpdateTargetPosition)
            {
                m_LastTargetPosition = Target.Value.transform.position;
                m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();
            }

            float distance = GetDistanceXZ();
            if (distance <= (DistanceThreshold + m_ColliderOffset))
            {
                return Status.Success;
            }
            
            if (CheckOutsideWaypoints())
            {
                return Status.Success;
            }

            if (m_NavMeshAgent != null)
            {
                if (boolUpdateTargetPosition)
                {
                    m_NavMeshAgent.SetDestination(m_ColliderAdjustedTargetPosition);
                }

                if (m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
                {
                    return Status.Success;
                }
            }
            else
            {
                float speed = Speed;

                if (SlowDownDistance > 0.0f && distance < SlowDownDistance)
                {
                    float ratio = distance / SlowDownDistance;
                    speed = Mathf.Max(0.1f, Speed * ratio);
                }

                Vector3 agentPosition = Agent.Value.transform.position;
                Vector3 toDestination = m_ColliderAdjustedTargetPosition - agentPosition;
                toDestination.y = 0.0f;
                toDestination.Normalize();
                agentPosition += toDestination * (speed * Time.deltaTime);
                Agent.Value.transform.position = agentPosition;

                // Update Y rotation for sprite flipping.
                if (toDestination.x > 0)
                {
                    // Facing right (default)
                    Agent.Value.transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (toDestination.x < 0)
                {
                    // Facing left (flipped)
                    Agent.Value.transform.rotation = Quaternion.Euler(0, 180, 0);
                }
            }

            return Status.Running;
        }

        protected override void OnEnd()
        {
            if (m_Animator != null)
            {
                m_Animator.SetFloat(AnimatorSpeedParam, 0);
            }

            if (m_NavMeshAgent != null)
            {
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.ResetPath();
                }
                m_NavMeshAgent.stoppingDistance = m_PreviousStoppingDistance;
            }

            m_NavMeshAgent = null;
            m_Animator = null;
        }

        protected override void OnDeserialize()
        {
            Initialize();
        }

        private Status Initialize()
        {
            m_LastTargetPosition = Target.Value.transform.position;
            m_ColliderAdjustedTargetPosition = GetPositionColliderAdjusted();

            // Add the extents of the colliders to the stopping distance.
            m_ColliderOffset = 0.0f;
            Collider agentCollider = Agent.Value.GetComponentInChildren<Collider>();
            if (agentCollider != null)
            {
                Vector3 colliderExtents = agentCollider.bounds.extents;
                m_ColliderOffset += Mathf.Max(colliderExtents.x, colliderExtents.z);
            }

            if (GetDistanceXZ() <= (DistanceThreshold + m_ColliderOffset))
            {
                return Status.Success;
            }

            // If using animator, set speed parameter.
            m_Animator = Agent.Value.GetComponentInChildren<Animator>();
            if (m_Animator != null)
            {
                m_Animator.SetFloat(AnimatorSpeedParam, Speed);
            }

            // If using a navigation mesh, set target position for navigation mesh agent.
            m_NavMeshAgent = Agent.Value.GetComponentInChildren<NavMeshAgent>();
            if (m_NavMeshAgent != null)
            {
                if (m_NavMeshAgent.isOnNavMesh)
                {
                    m_NavMeshAgent.ResetPath();
                }
                m_NavMeshAgent.speed = Speed;
                m_PreviousStoppingDistance = m_NavMeshAgent.stoppingDistance;

                m_NavMeshAgent.stoppingDistance = DistanceThreshold + m_ColliderOffset;
                m_NavMeshAgent.SetDestination(m_ColliderAdjustedTargetPosition);
            }

            return Status.Running;
        }


        private Vector3 GetPositionColliderAdjusted()
        {
            Collider targetCollider = Target.Value.GetComponentInChildren<Collider>();
            if (targetCollider != null)
            {
                return targetCollider.ClosestPoint(Agent.Value.transform.position);
            }
            return Target.Value.transform.position;
        }

        private float GetDistanceXZ()
        {
            Vector3 agentPosition = new Vector3(Agent.Value.transform.position.x, m_ColliderAdjustedTargetPosition.y, Agent.Value.transform.position.z);
            return Vector3.Distance(agentPosition, m_ColliderAdjustedTargetPosition);
        }
        
        private bool CheckOutsideWaypoints()
        {
            if (Waypoints == null || Waypoints.Value == null || Waypoints.Value.Count < 2)
            {
                return false; // Not enough waypoints to form a bounding area.
            }

            // Calculate the bounding box from waypoints
            float minX = float.MaxValue, maxX = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (var waypoint in Waypoints.Value)
            {
                if (waypoint != null)
                {
                    Vector3 position = waypoint.transform.position;
                    minX = Mathf.Min(minX, position.x);
                    maxX = Mathf.Max(maxX, position.x);
                    minZ = Mathf.Min(minZ, position.z);
                    maxZ = Mathf.Max(maxZ, position.z);
                }
            }

            // Get agent position in XZ plane
            Vector3 agentPosition = Agent.Value.transform.position;
            float agentX = agentPosition.x;
            float agentZ = agentPosition.z;

            // Check if agent is outside the bounding box
            if (agentX < minX - DistanceThreshold - m_ColliderOffset || 
                agentX > maxX + DistanceThreshold + m_ColliderOffset || 
                agentZ < minZ - DistanceThreshold - m_ColliderOffset || 
                agentZ > maxZ + DistanceThreshold + m_ColliderOffset)
            {
                return true; // Agent is outside the waypoints' bounding area.
            }

            return false; // Agent is inside the waypoints' bounding area.
        }
}

