using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarEngine : MonoBehaviour
{
    [Header("Input")]
    public float _distanceToBreak = 10;
    public float _maxBreakTorque = 150;
    public float _torquePower;
    public float _currentSpeed;
    public float _maxSpeed = 100;
    public float _distanceToSlowdown;
    public Vector3 _centerOfMass;
    public Transform path;
    private List<Transform> _nodes;
    public float _maxSteeringAngle = 40;
    public WheelCollider wheelFL, wheelFR;
    public WheelCollider wheelBL, wheelBR;

    [Header("ReadOnly")]    
    public bool _stopSign = false;
    public float _DistNextNode, _DistPrevNode;
    public Path _pathRef;       
    public float _origMaxSpeed;            
    public bool _isBreaking = false;
    public bool _isStopped = false;
    public int _currentNode = 0;
    public float _distanceToNextWayPoint;

    //====================================

    void Start ()
    {
        InitiatePath();
    }

    //====================================

    private void InitiatePath()
    {
        GetComponent<Rigidbody>().centerOfMass = _centerOfMass;

        _origMaxSpeed = _maxSpeed;

        _distanceToSlowdown = _distanceToBreak * 2;

        Transform[] _pathTransform = path.GetComponentsInChildren<Transform>();
        _nodes = new List<Transform>();

        for (int i = 0; i < _pathTransform.Length; i++)
        {
            if (_pathTransform[i] != path.transform)
            {
                _nodes.Add(_pathTransform[i]);
            }
        }
    }

    //====================================

    private void FixedUpdate ()
    {     
        CheckWayPointDistance();
        CheckSlowdown();
        ApplySteer();
        Drive();                
        CheckNextNode();
        Breaking();
	}

    //====================================

    private void ApplySteer()
    {
        //Steer front wheels towards next waypoint
        Vector3 relativeVector = transform.InverseTransformPoint(_nodes[_currentNode].position);

        float newSteer = (relativeVector.x / relativeVector.magnitude) * _maxSteeringAngle;

        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
    }

    //====================================

    private void Drive()
    {
        _currentSpeed = 2 * Mathf.PI * wheelFL.radius * wheelFL.rpm * 60 / 1000;
    }

    //====================================

    public void SetMotorTorque(float newTorque)
    {
        wheelFL.motorTorque = newTorque;
        wheelFR.motorTorque = newTorque;
    }

    //====================================

    private void CheckSlowdown()
    {        
        //Slowdown car when approaching a waypoint, to ease in and ease out of turns.
        _DistNextNode = Vector3.Distance(transform.position, _nodes[_currentNode].position);

        if (_currentNode == 0)
        {
            _DistPrevNode = Vector3.Distance(transform.position, _nodes[_nodes.Count - 1].position);
        }
        else
        {
            _DistPrevNode = Vector3.Distance(transform.position, _nodes[_currentNode - 1].position);
        }

        if ((_DistNextNode < _distanceToSlowdown) || (_DistPrevNode < (_distanceToSlowdown / 2)))
        {
            _maxSpeed = _origMaxSpeed * 0.5f;
        }
        else
        {
            _maxSpeed = _origMaxSpeed;;
        }
    }

    //====================================

    private void CheckWayPointDistance()
    {
        //Switch to the next waypoint in the list once you reach the waypoint you were heading towards
        _distanceToNextWayPoint = Vector3.Distance(transform.position, _nodes[_currentNode].position);
        if (_distanceToNextWayPoint < _pathRef._minDistBeforeNextWaypoint)
        {
            //Loop around
            if (_currentNode == _nodes.Count - 1)
            {
                _currentNode = 0;
            }
            else
            {
                _currentNode++;
            }
        }
    }

    //====================================

    private void Breaking()
    {
       //Logic for applying brakes
        if ((_isBreaking) || (_isStopped))
        {
            wheelBL.brakeTorque = _maxBreakTorque;
            wheelBR.brakeTorque = _maxBreakTorque;
            SetMotorTorque(0f);
        }
        else
        {
            wheelBL.brakeTorque = 0;
            wheelBR.brakeTorque = 0;

            //Only accelerate when below max speed, to sustain speed
            if (_currentSpeed < _maxSpeed)
            {
                SetMotorTorque(_torquePower);
            }
            else
            {
                SetMotorTorque(0f);
            }
        }
       
    }

    //====================================

    private void CheckNextNode()
    {
        //Check if the next waypoint has a traffic attachment, such as a stop sign or traffic light
        if (_nodes[_currentNode].gameObject.tag == "TrafficRobot")
        {
            _stopSign = false;

            TrafficSignAttachment _trafficSignAttachmentRef = _nodes[_currentNode].gameObject.GetComponent<TrafficSignAttachment>();
            TrafficRobot _trafficRobotRef = _trafficSignAttachmentRef._attachment.GetComponent<TrafficRobot>();
             
            //Traffic light logic
            switch (_trafficRobotRef._trafficColor)
            {
                case TrafficRobot.TrafficColor.Red:
                    if (_distanceToNextWayPoint < _distanceToBreak)
                    {
                        _isBreaking = true;
                        StartCoroutine(CloseEnoughToTrafficLight());
                    }
                    else
                    {
                        StartCoroutine(CloseEnoughToTrafficLight());
                    }
                    break;
                case TrafficRobot.TrafficColor.Green:
                    _isBreaking = false;
                    break;
                case TrafficRobot.TrafficColor.Orange:
                    if ((_distanceToNextWayPoint < _distanceToBreak) && (_distanceToNextWayPoint > (_distanceToBreak * 0.5f)))                    
                    {
                        _isBreaking = true;
                        StartCoroutine(CloseEnoughToTrafficLight());
                    }
                    else 
                    {
                        StartCoroutine(CloseEnoughToTrafficLight());
                    }
                    break;
                default:
                    break;
            }
        }

        //Stop sign logic
        else if (_nodes[_currentNode].gameObject.tag == "StopSign")
        {
            if ((_distanceToNextWayPoint < _distanceToBreak) && (!_stopSign))
            {
                _stopSign = true;
                StartCoroutine(GetPastPrevNode());
            }
        }

        //No attachments to waypoint
        else
        {
            //_stopSign is used to make Stop Sign logic work
            _stopSign = false;
        }
    }

    //====================================

    IEnumerator CloseEnoughToTrafficLight()
    {
        //This is to compensate for the fraction of a second when the car thinks it's close enough to the next waypoint, but hasn't switched
        //-to the next waypoint yet.
        yield return new WaitForSeconds(0.1f);
        if (_distanceToNextWayPoint >= _distanceToBreak)
        {
            _isBreaking = false;
        }
    }

    //====================================

    IEnumerator GetPastPrevNode()
    {
        TrafficSignAttachment _trafficSignAttachmentRef = _nodes[_currentNode].gameObject.GetComponent<TrafficSignAttachment>();
        StopSign _stopSignRef = _trafficSignAttachmentRef._attachment.GetComponent<StopSign>();

        yield return new WaitForSeconds(0.1f);
        if(((_distanceToNextWayPoint < (_distanceToBreak))) && (_currentSpeed > (_maxSpeed/2f)) )
        {
            StartCoroutine(StopForPeriod(_stopSignRef._secondsToWait));
        }
        else if (((_distanceToNextWayPoint < (_distanceToBreak / 2f))) && (_currentSpeed < (_maxSpeed / 2f)))
        {
            StartCoroutine(StopForPeriod(_stopSignRef._secondsToWait));
        }
        else
        {
            _stopSign = false;
        }
    }

    //====================================

    IEnumerator StopForPeriod(int seconds)
    {

        _isBreaking = true;
        yield return new WaitForSeconds(seconds);
        _isBreaking = false;
    }
}
