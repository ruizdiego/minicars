// .NET Framework
using System.Collections.Generic;

// Unity Framework
using UnityEngine;

[ExecuteInEditMode]
public class WaypointManager : MonoBehaviour
{
    // Show gizmos nodes?
    public bool showNodes = true;
    
    // Show gizmos lines?
    public bool showLines = false;

    public Color debugLinesColor = Color.red;
    
    // Nodes collection
    private List<Transform> _nodes;
    
    // Size of the nodes
    private Vector3 _nodeSize = Vector3.one;
    
    /// <summary>
    /// Unity Start Method
    /// </summary>
    void Start()
    {
        PopulateNodesList();
    }
    
    /// <summary>
    /// Unity OnDrawGizmos Method
    /// </summary>
    private void OnDrawGizmos()
    {
        if (_nodes.Count > 1)
        {
            Gizmos.color = debugLinesColor;
            for (int i = 0; i < _nodes.Count - 1; i++)
            {
                if (showNodes) Gizmos.DrawCube(_nodes[i].position, _nodeSize);
                if (showLines) Gizmos.DrawLine(_nodes[i].position, _nodes[i + 1].position);
            }
            
            if (showNodes) Gizmos.DrawCube(_nodes[_nodes.Count - 1].position, _nodeSize);
            
            Gizmos.color = Color.green;
            if (showLines) Gizmos.DrawLine(_nodes[_nodes.Count - 1].position, _nodes[0].position);
        }
    }

    private void PopulateNodesList()
    {
        _nodes = new List<Transform>();
        
        var nodes = GetComponentsInChildren<Transform>();
        if (nodes != null)
        {
            int nodeNum = 0;
            for (int i = 0; i < nodes.Length; i++)
            {
                if (nodes[i] != transform)
                {
                    _nodes.Add(nodes[i]);
                    nodes[i].name = $"Node-{nodeNum}";
                    nodeNum++;
                }
            }
        }
    }

    /// <summary>
    /// Add a new node in the end of the list
    /// </summary>
    public void AddNode()
    {
        var newNode = new GameObject();
        newNode.transform.SetParent(transform);
        
        if (_nodes.Count > 0)
        {
            var lastNodeName = _nodes[_nodes.Count - 1].name;
            var lastNodeNumber = int.Parse(lastNodeName.Substring(lastNodeName.IndexOf('-') + 1));
            newNode.name = $"Node-{lastNodeNumber + 1}";
            newNode.transform.position = new Vector3(_nodes[_nodes.Count - 1].transform.position.x, 0.1f, _nodes[_nodes.Count - 1].transform.position.z);
        }
        else
        {
            newNode.name = "Node-1";
            newNode.transform.position = new Vector3(0f, 0.1f, 0f);
        }
        _nodes.Add(newNode.transform);
    }

    public void ClearAll()
    {
        if (_nodes != null)
        {
            for (int i = 0; i < _nodes.Count; i++)
            {
                GameObject.DestroyImmediate(_nodes[i].gameObject);
            }
            
            _nodes.Clear();
        }
    }

    /// <summary>
    /// Get node position
    /// </summary>
    public Vector3 GetPosition(int nodeNum)
    {
        if (_nodes == null) PopulateNodesList();
        return _nodes[nodeNum % _nodes.Count].position;
    }

    /// <summary>
    /// Get the distance from the specified node to the next one
    /// </summary>
    public float GetDistanceToNextNode(int nodeNum)
    {
        return Vector3.Distance(GetPosition(nodeNum), GetPosition(nodeNum + 1));
    }

    /// <summary>
    /// Returns the number of nodes
    /// </summary>
    /// <returns></returns>
    public int GetNodesCount()
    {
        return _nodes.Count;
    }
    
}
