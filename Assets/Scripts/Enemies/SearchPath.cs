using UnityEngine;

public class SearchPath : MonoBehaviour
{
    public Transform[] waypoints;

    private void OnValidate()
    {
        // Auto-populate waypoints from children
        waypoints = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            waypoints[i] = transform.GetChild(i);
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < waypoints.Length; i++)
        {
            Gizmos.DrawSphere(waypoints[i].position, 0.2f);
            if (i < waypoints.Length - 1)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
