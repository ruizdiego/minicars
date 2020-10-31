using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class CarBasicMover : MonoBehaviour
{
    public WaypointManager waypoints;

    // Max Speed
    public float maxSpeed = 25f;

    // Acceleration
    public float acceleration = 10f;
    
    // Current node
    private int _currentNode;

    private bool _driving = false;

    private Vector3 _dirv;

    // Current car speed
    private float _speed;

    private float _prevDistanceToNode;
    
    /// <summary>
    /// Unity Start Method
    /// </summary>
    void Start()
    {
        _currentNode = 0;
        _speed = 0f;
        
        SetCarPos(_currentNode);

        StartDriving();
    }

    /// <summary>
    /// Unity Update Method
    /// </summary>
    void Update()
    {
        if (_driving)
        {
            var disToNextNode = GetDistanceToNextNode();
            
            if (disToNextNode < 0.1f || disToNextNode > _prevDistanceToNode)
            {
                _currentNode++;
                var nextPos = waypoints.GetPosition(_currentNode + 1);
                transform.LookAt(new Vector3(nextPos.x, transform.position.y, nextPos.z));
                _dirv = (new Vector3(nextPos.x, 0f, nextPos.z) - new Vector3(transform.position.x, 0f, transform.position.z)).normalized;
                
                _prevDistanceToNode = float.MaxValue;
            }
            else
            {
                _prevDistanceToNode = disToNextNode;
            }
            
            var dt = Time.deltaTime;
            
            _speed = Mathf.Min(_speed + acceleration * dt, maxSpeed);
            
            var newPosX = transform.position.x + _dirv.x * _speed * dt;
            var newPosZ = transform.position.z + _dirv.z * _speed * dt;

            transform.position = new Vector3(newPosX, transform.position.y, newPosZ);
        }
    }

    private float GetDistanceToNextNode()
    {
        var nextPos = waypoints.GetPosition(_currentNode + 1);
        return Vector3.Distance(new Vector3(transform.position.x, 0f, transform.position.z), new Vector3(nextPos.x, 0f, nextPos.z));
    }
    
    private void SetCarPos(int nodeNum)
    {
        var pos = waypoints.GetPosition(_currentNode);
        transform.position = new Vector3(pos.x, transform.position.y, pos.z);
        
        var nextPos = waypoints.GetPosition(_currentNode + 1);
        transform.LookAt(new Vector3(nextPos.x, transform.position.y, nextPos.z));

        _dirv = (nextPos - pos).normalized;

        _prevDistanceToNode = float.MaxValue;
    }
    
    public void StartDriving()
    {
        _driving = true;
    }

    public void StopDriving()
    {
        _driving = false;
    }
}
