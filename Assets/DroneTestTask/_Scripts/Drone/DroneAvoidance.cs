using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DroneAvoidance : MonoBehaviour
{
    [SerializeField] private float _avoidanceRadius = 3f;
    [SerializeField] private LayerMask _droneLayer;

    private NavMeshAgent _agent;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        _agent.avoidancePriority = Random.Range(30, 70);
    }

    private void Update()
    {
        AvoidNearbyDrones();
    }

    private void AvoidNearbyDrones()
    {
        var nearbyDrones = Physics.OverlapSphere(transform.position, _avoidanceRadius, _droneLayer);
        var avoidanceDirection = Vector3.zero;

        foreach (var drone in nearbyDrones)
        {
            if (drone.gameObject == gameObject) 
                continue;

            var awayFromDrone = transform.position - drone.transform.position;
            var distance = awayFromDrone.magnitude;

            if (distance > 0f)
                avoidanceDirection += awayFromDrone.normalized / distance;
        }

        if (avoidanceDirection != Vector3.zero && _agent.isOnNavMesh)
        {
            var steer = avoidanceDirection.normalized * _agent.speed * Time.deltaTime;
            _agent.Move(steer);
        }
    }
}