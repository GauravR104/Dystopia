using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class DetetcionEnemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private SearchPath currentSearchPath;
    private Transform[] waypoints;
    public float searchWaitTime = 2f;
    public Transform player;
    public Transform alertZone;
    public float detectionRange = 10f;
    public float fieldOfView = 120f;

    private Vector3 initialPosition;
    private int currentWaypoint = 0;
    private bool isSearching = false;

    private enum State { Idle, MovingToSound, Searching, Returning }
    private State currentState = State.Idle;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        initialPosition = transform.position;
        agent.stoppingDistance = 0f;
    }

    private void Update()
    {
        DetectPlayer();

        switch (currentState)
        {
            case State.MovingToSound:
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                    StartCoroutine(BeginSearch());
                break;

            case State.Searching:
                if (!agent.pathPending && agent.remainingDistance < 0.1f && !isSearching)
                {
                    currentWaypoint++;
                    if (waypoints != null && currentWaypoint < waypoints.Length)
                    {
                        agent.SetDestination(waypoints[currentWaypoint].position);
                    }
                    else
                    {
                        currentState = State.Returning;
                        agent.SetDestination(initialPosition);
                    }
                }
                break;

            case State.Returning:
                if (!agent.pathPending && agent.remainingDistance < 0.1f)
                {
                    currentState = State.Idle;
                    currentWaypoint = 0;
                }
                break;
        }
    }

    public void AlertEnemy(Vector3 soundPosition)
    {
        Debug.Log("Enemy heard something!");
        currentState = State.MovingToSound;
        agent.SetDestination(soundPosition);
    }

    private IEnumerator BeginSearch()
    {
        isSearching = true;
        yield return new WaitForSeconds(searchWaitTime);

        currentSearchPath = FindNearestSearchPath();
        if (currentSearchPath != null && currentSearchPath.waypoints.Length > 0)
        {
            waypoints = currentSearchPath.waypoints;
            currentState = State.Searching;
            currentWaypoint = 0;
            agent.SetDestination(waypoints[0].position);
        }
        else
        {
            currentState = State.Returning;
            agent.SetDestination(initialPosition);
        }

        isSearching = false;
    }

    private SearchPath FindNearestSearchPath()
    {
        SearchPath[] paths = FindObjectsByType<SearchPath>(FindObjectsSortMode.None);
        SearchPath nearest = null;
        float closestDist = Mathf.Infinity;

        foreach (SearchPath path in paths)
        {
            if (path.waypoints == null || path.waypoints.Length == 0) continue;

            foreach (Transform waypoint in path.waypoints)
            {
                float dist = Vector3.Distance(transform.position, waypoint.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    nearest = path;
                }
            }
        }

        return nearest;
    }

    private void DetectPlayer()
    {
        if (Vector3.Distance(player.position, alertZone.position) <= detectionRange)
        {
            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToPlayer) < fieldOfView / 2f)
            {
                Ray ray = new Ray(transform.position + Vector3.up, dirToPlayer);
                if (Physics.Raycast(ray, out RaycastHit hit, detectionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        Debug.Log("Player detected! Game Over!");
                       // GameManager.Instance.GameOver();
                    }
                }
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (alertZone != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(alertZone.position, detectionRange);
        }

        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -fieldOfView / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, fieldOfView / 2, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + leftBoundary * detectionRange);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary * detectionRange);

        Gizmos.color = Color.white;
        Gizmos.DrawCube(initialPosition + Vector3.up * 0.5f, Vector3.one * 0.3f);
    }
}
