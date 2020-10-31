using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

[ExecuteInEditMode]
public class CarMover : MonoBehaviour
{
    private const float MinDistanceToNode = 0.1f;
    
    // Reference to the waypoint manager
    public WaypointManager waypoints;

    // Max Speed
    public float maxSpeed = 25f;

    // Acceleration
    public float acceleration = 10f;

    // Steering
    public float steering = 1f;

    // Draw path?
    public bool drawPath = true;
    public float timeStep = 0.2f;

    public Color debugLinesColor = Color.red;
    
    private bool _driving = false;

    private Vector3 initialPos;
    private Quaternion initialRot;
    
    class MoveData
    {
        public float speed;
        public int currentNode;
        public float prevDistanceToNode;
        public Vector3 pos;
        public Quaternion rot;
    }

    private MoveData _moveData;
    
    // Start is called before the first frame update
    void Start()
    {
        MoveToStartPos();

        initialPos = transform.position;
        initialRot = transform.localRotation;
        
        StartDriving();
    }

    // Update is called once per frame
    void Update()
    {
        if (_driving && Application.isPlaying)
        {
            UpdateMovement(Time.deltaTime, _moveData);

            transform.position = _moveData.pos;
            transform.localRotation = _moveData.rot;
        }
    }

    private void OnDrawGizmos()
    {
        if (drawPath)
        {
            var moveData = new MoveData
            {
                speed = 0f,
                currentNode = 0,
                prevDistanceToNode = float.MaxValue,
                pos = initialPos,
                rot = initialRot
            };
            
            Vector3 pos1 = waypoints.GetPosition(0);

            int iterations = 0;
            while (moveData.currentNode < waypoints.GetNodesCount())
            {
                UpdateMovement(timeStep, moveData);

                Gizmos.color = debugLinesColor;
                Gizmos.DrawLine(pos1, moveData.pos);

                pos1 = moveData.pos;

                iterations++;
                if (iterations > 5000) break;
            }
        }
        
    }

    
    

    private void UpdateMovement(float dt, MoveData moveData)
    {
        var disToNextNode = GetDistanceToNextNode(moveData);
            
        // Check if we already reach the following node
        if (disToNextNode < MinDistanceToNode || disToNextNode > moveData.prevDistanceToNode)
        {
            // Move to the next node
            moveData.currentNode++;
                
            // Reset some values
            moveData.prevDistanceToNode = float.MaxValue;
        }
        else
        {
            moveData.prevDistanceToNode = disToNextNode;
        }
           
        // Increase the speed
        moveData.speed = Mathf.Min(moveData.speed + acceleration * dt, maxSpeed);

        // Steer the car
        float curAngle = moveData.rot.eulerAngles.y;
        var fromAngle = moveData.rot.eulerAngles.y;
        var toAngle = GetClosestAngle(moveData.rot.eulerAngles.y, GetAngleToNextNode(moveData));
        if (fromAngle < toAngle)
        {
            curAngle = Mathf.Min(curAngle + steering * dt, toAngle);
        }
        else
        {
            curAngle = Mathf.Max(curAngle - steering * dt, toAngle);
        }
            
        moveData.rot = Quaternion.Euler(moveData.rot.eulerAngles.x, curAngle, moveData.rot.eulerAngles.z);
            
        var dirX = Mathf.Sin(curAngle * Mathf.Deg2Rad);
        var dirZ = Mathf.Cos(curAngle * Mathf.Deg2Rad);
            
        var newPosX = moveData.pos.x + dirX * moveData.speed * dt;
        var newPosZ = moveData.pos.z + dirZ * moveData.speed * dt;

        moveData.pos = new Vector3(newPosX, moveData.pos.y, newPosZ);
    }

    /// <summary>
    /// Returns the distance between the current position and the specified node
    /// </summary>
    private float GetDistanceToNextNode(MoveData moveData)
    {
        var nextPos = waypoints.GetPosition(moveData.currentNode + 1);
        return Vector3.Distance(new Vector3(moveData.pos.x, 0f, moveData.pos.z), new Vector3(nextPos.x, 0f, nextPos.z));
    }

    public void MoveToStartPos()
    {
        _moveData = new MoveData
        {
            speed = 0f,
            currentNode = 0,
            prevDistanceToNode = float.MaxValue,
            accumTime = 0f,
            pos = transform.position,
            rot = transform.localRotation
        };

        var pos = waypoints.GetPosition(_moveData.currentNode);
        _moveData.pos = new Vector3(pos.x, _moveData.pos.y, pos.z);
        
        var nextPos = waypoints.GetPosition(_moveData.currentNode + 1);
        var dir = (nextPos - _moveData.pos).normalized;
        float angle = (Mathf.Atan2(dir.z, dir.x) * -Mathf.Rad2Deg) + 90f;
        _moveData.rot = Quaternion.Euler(0f, angle, 0f);
        
        _moveData.prevDistanceToNode = float.MaxValue;
        _moveData.accumTime = 0f;
        
        transform.position = _moveData.pos;
        transform.localRotation = _moveData.rot;
    }
    
    public void StartDriving()
    {
        _driving = true;
    }

    public void StopDriving()
    {
        _driving = false;
    }

    private float GetAngleToNextNode(MoveData moveData)
    {
        var nextPos = waypoints.GetPosition(moveData.currentNode + 1);
        var dir = (nextPos - moveData.pos).normalized;
        return Mathf.Atan2(dir.z, dir.x) * -Mathf.Rad2Deg + 90f;
    }

    private float GetClosestAngle(float angleFrom, float angleTo)
    {
        float d1 = Mathf.Abs(angleFrom - angleTo);
        float d2 = Mathf.Abs(angleFrom - (angleTo + 360f));
        float d3 = Mathf.Abs(angleFrom - (angleTo - 360f));

        if (d1 < d2)
        {
            return (d1 < d3 ? angleTo : angleTo - 360f);
        }
        else
        {
            return (d2 < d3 ? angleTo + 360f : angleTo - 360f);
        }
    }

}
