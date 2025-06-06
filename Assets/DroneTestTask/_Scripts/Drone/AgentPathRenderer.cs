using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(LineRenderer))]
public class AgentPathRenderer : MonoBehaviour
{
    [SerializeField] private Color _lineColor = Color.cyan;
    [SerializeField] private float _lineWidth = 0.2f;

    private NavMeshAgent _agent;
    private LineRenderer _line;
    private bool _showPath;
    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _line = GetComponent<LineRenderer>();

        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.widthMultiplier = _lineWidth;
        _line.positionCount = 0;
        _line.startColor = _lineColor;
        _line.endColor = _lineColor;
    }

    public void ChangeShowPath(bool value)
    {
        _showPath = value;
        _line.enabled = value;
    }
    
    private void Update()
    {
        if (!_showPath)
            return;
        
        if (_agent.path == null || _agent.path.corners.Length < 2)
        {
            _line.positionCount = 0;
            return;
        }

        var corners = _agent.path.corners;
        _line.positionCount = corners.Length;
        for (var i = 0; i < corners.Length; i++)
        {
            _line.SetPosition(i, corners[i] + Vector3.up * 0.1f);
        }
    }
}