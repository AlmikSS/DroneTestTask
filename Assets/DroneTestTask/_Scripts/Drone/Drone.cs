using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AgentPathRenderer))]
public class Drone : MonoBehaviour
{
    private Base _base;
    private Team _team;
    private NavMeshAgent _agent;
    private DroneState _state;
    private Resource _currentResource;
    private ResourcesSpawner _resourcesSpawner;
    private AgentPathRenderer _agentPathRenderer;
    
    public Team Team => _team;
    public AgentPathRenderer AgentPathRenderer => _agentPathRenderer;

    public void Initialize(ResourcesSpawner resourcesSpawner, Base @base, Team team)
    {
        _resourcesSpawner = resourcesSpawner;
        _base = @base;
        _team = team;
        _agent = GetComponent<NavMeshAgent>();
        _agentPathRenderer = GetComponent<AgentPathRenderer>();
    }

    public void StartGameCycle()
    {
        StartCoroutine(GoToResState());
    }

    public void SetSpeed(float speed)
    {
        _agent.speed = speed;
    }
    
    private IEnumerator GoToBaseState()
    {
        _state = DroneState.GoToBase;
        _agent.SetDestination(_base.Position);
        yield return new WaitWhile(() => 
            Vector3.Distance(transform.position, _base.Position) > 0.5f);
        
        _base.GiveRes(this);
        _currentResource = null;
    }

    private IEnumerator GoToResState()
    {
        _state = DroneState.GoToRes;
        yield return StartCoroutine(FindRes());
        
        if (_currentResource == null)
        {
            StartGameCycle();
            yield break;
        }
        
        _currentResource.Book();
        _agent.SetDestination(_currentResource.Position);
        yield return new WaitWhile(() => Vector3.Distance(transform.position, _currentResource.Position) > _agent.stoppingDistance);

        StartCoroutine(PickUpResState());
    }
    
    private IEnumerator PickUpResState()
    {
        _state = DroneState.PickUpRes;
        yield return new WaitForSeconds(_currentResource.PickUpTime);
        _currentResource.PickUp();
        StartCoroutine(GoToBaseState());
    }

    private IEnumerator FindRes()
    {
        var list = _resourcesSpawner.Resources;
        var notBookedRes = list.Where(resource => !resource.IsBooked).ToList();

        if (notBookedRes.Count == 0)
            yield break;

        notBookedRes.Sort((a, b) =>
            Vector3.Distance(a.Position, transform.position)
                .CompareTo(Vector3.Distance(b.Position, transform.position)));

        var closest = notBookedRes[0];

        yield return new WaitForEndOfFrame();

        if (!closest.IsBooked)
            _currentResource = closest;
    }
}

public enum DroneState
{
    GoToBase,
    GoToRes,
    PickUpRes
}