using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CarEngineMultiPath_v02 : MonoBehaviour
{
    [Header("Input")]
    public float _distanceToBreak = 10;
    public float _distanceToSlowdown = 20;
    public float _torquePower;
    public float _currentSpeed;
    public float _maxSpeed = 100;
    public Vector3 _centerOfMass;
    public Transform path;
    private List<Transform> _nodes;
    public float _maxSteeringAngle = 40;
    public WheelCollider wheelFL, wheelFR;
    public WheelCollider wheelBL, wheelBR;

    public enum WhichSideOfRoad
    {
        Left,
        Right
    };

    public WhichSideOfRoad _whichSideOfRoad;

    [Space]

    [Header("References")]
    public MultiPath _multiPathRef;    
    public BumperLogic_v02 _leftDetector, _rightDetector;

    [Space]

    [Header("ReadOnly, Please Ignore")]
    public float _maxBreakTorque = 150;
    public bool _applyHalfBrakes;
    public float _force;
    public float _steeringValue;
    public bool _pullOver = false, _startedPullingOver = false;
    public bool _stopSign = false;
    public float _DistNextNode, _DistPrevNode;
    public float _origMaxSpeed;
    public bool _isBreaking = false;
    public bool _isStopped = false;
    public int _currentNode = 0;
    public float _distanceToNextWayPoint;
    public int _testHowManyNodes;
    public EmergencyBreakLogic_v02 _eBrake02Ref;
    public Rigidbody _rigidBodyRef;

    //====================================

    void Start()
    {   
        InitiatePath();
    }

    //====================================

    private void InitiatePath()
    {
        GetComponent<Rigidbody>().centerOfMass = _centerOfMass;
        _eBrake02Ref = this.gameObject.GetComponent<EmergencyBreakLogic_v02>();
        _rigidBodyRef = this.gameObject.GetComponent<Rigidbody>();

        if (_multiPathRef)
        {
            if (_multiPathRef._whichSideOfRoad == MultiPath.WhichSideOfRoad.Left)
            {
                _whichSideOfRoad = WhichSideOfRoad.Left;
            }
            else
            {
                _whichSideOfRoad = WhichSideOfRoad.Right;
            }

            _origMaxSpeed = _maxSpeed;

            Transform[] _pathTransform = path.GetComponentsInChildren<Transform>();
            _nodes = new List<Transform>();

            for (int i = 0; i < _pathTransform.Length; i++)
            {
                if (_pathTransform[i] != path.transform)
                {
                    _nodes.Add(_pathTransform[i]);
                }
            }

            _testHowManyNodes = _nodes.Count - 1;
            _torquePower = _rigidBodyRef.mass / 4;

            if (_whichSideOfRoad == WhichSideOfRoad.Left)
            {
                _leftDetector.enabled = false;
            }
            else
            {
                _rightDetector.enabled = false;
            }
        }
    }

    //====================================

    private void FixedUpdate()
    {
        if (_multiPathRef)
        {
            CheckWayPointDistance();
            CheckSlowdown();
            ApplySteer();
            Drive();
            AdjustBraking();
            CheckNextNode();
            Braking();
        }
    }

    private void ApplySteer()
    {
        if (!_pullOver)
        {
            Vector3 relativeVector = transform.InverseTransformPoint(_nodes[_currentNode].position);

            float newSteer = (relativeVector.x / relativeVector.magnitude) * _maxSteeringAngle;

            wheelFL.steerAngle = newSteer;
            wheelFR.steerAngle = newSteer;

            _steeringValue = newSteer;

            if (_startedPullingOver)
            {
                _startedPullingOver = false;
                _isStopped = false;
            }
        }
        else
        {
            if (!_startedPullingOver)
            {
                _startedPullingOver = true;
                wheelFL.steerAngle = 35f;
                wheelFR.steerAngle = 35f;

                StartCoroutine(PullingOver());
            }
        }
    }

    IEnumerator PullingOver()
    {
        yield return new WaitForSeconds(2f);
        wheelFL.steerAngle = 0f;
        wheelFR.steerAngle = 0f;
        yield return new WaitForSeconds(1f);
        _isStopped = true;
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

        //Account for a looping circuit
        if ((_currentNode == 0) && (_multiPathRef._closedCircuit))
        {
            _DistPrevNode = Vector3.Distance(transform.position, _nodes[_nodes.Count - 1].position);
        }
        else if ((_currentNode != 0) && (_multiPathRef._closedCircuit))
        {
            _DistPrevNode = Vector3.Distance(transform.position, _nodes[_currentNode - 1].position);
        }

        if (_distanceToNextWayPoint < _distanceToSlowdown)
        {
            //If the next node wishes to force a speed limit on the car
            MultiPathNodeIdentifier _multiPathModeIDref = _nodes[_currentNode].gameObject.GetComponent<MultiPathNodeIdentifier>();
            if (_multiPathModeIDref._overrideSpeed)
            {
                _origMaxSpeed = _multiPathModeIDref._newOriginalSpeed;
            }
            
            // if the next node is not a traffic robot AND the next node is not a stop sign
            if ((_nodes[_currentNode].gameObject.tag != "TrafficRobot") && (_nodes[_currentNode].gameObject.tag != "StopSign"))
            {
                if (_eBrake02Ref._shouldBrake)
                {                    
                    if (_currentSpeed > 10)
                    {
                        _applyHalfBrakes = true;
                    }
                    else
                    {
                        _applyHalfBrakes = false;
                    }

                    StartCoroutine(GetPastPrevNode());
                }
                else
                {
                    _applyHalfBrakes = false;
                }
            }            
        }
    }

    //====================================

    private void CheckWayPointDistance()
    {
        //Switch to the next waypoint in the list once you reach the waypoint you were heading towards

        float _tempFloat;
        
        if (_nodes[_currentNode].gameObject.GetComponent<MultiPathNodeIdentifier>()._overrideBool)
        {
            _tempFloat = _nodes[_currentNode].gameObject.GetComponent<MultiPathNodeIdentifier>()._overrideFloat;
        }
        else
        {
            _tempFloat = _multiPathRef._minDistBeforeNextWaypoint;
        }

        _distanceToNextWayPoint = Vector3.Distance(transform.position, _nodes[_currentNode].position);

        //if (_distanceToNextWayPoint < _multiPathRef._minDistBeforeNextWaypoint)
        if (_distanceToNextWayPoint < _tempFloat)
        {
            MultiPathNodeIdentifier _multiPathRef = _nodes[_currentNode].gameObject.GetComponent<MultiPathNodeIdentifier>();

            if (_multiPathRef._connectionsCount > 1)
            {
                int _tempRnd, _tmpLow, _tmpHigh;
                _tmpLow = _multiPathRef._nextNodeIndexList[0];
                _tmpHigh = _multiPathRef._nextNodeIndexList[_multiPathRef._nextNodeIndexList.Count - 1];
                               
                _tempRnd = UnityEngine.Random.Range(_tmpLow, (_tmpHigh + 1));
                
                _currentNode = _tempRnd;
            }
            else if (_multiPathRef._connectionsCount == 1)
            {
                _currentNode = _multiPathRef._nextNodeIndexList[0];
            }
            else
            {
                _currentNode = 0;
            }
        }
    }

    //====================================

    private void Braking()
    {
        //If within 'hard brake' distance, increase to braking power to quickly come to a stop
        if (_distanceToNextWayPoint < (_distanceToBreak))
        {
            _maxBreakTorque = _rigidBodyRef.mass * 2;
        }
        else
        {
            _maxBreakTorque = _eBrake02Ref._force;
        }

        //'Hard brakes'
        if ((_isBreaking) || (_isStopped))
        {
            wheelBL.brakeTorque = _maxBreakTorque;
            wheelBR.brakeTorque = _maxBreakTorque;
            SetMotorTorque(0f);
        }

        //'Soft brakes'
        else
        {               
            if(_applyHalfBrakes)
            {
                wheelBL.brakeTorque = _maxBreakTorque;
                wheelBR.brakeTorque = _maxBreakTorque;
            }
            else
            {
                wheelBL.brakeTorque = 0;
                wheelBR.brakeTorque = 0;
            }

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

    private void AdjustBraking()
    {
        //If the car has too much momentum, increase the 'hard brake' distance
        if (_eBrake02Ref._inirtiaFloat > 1650f)
        {
            _distanceToBreak = _eBrake02Ref._origDistToBrake * 1.5f;
        }
        else
        {
            _distanceToBreak = _eBrake02Ref._origDistToBrake;
        }
    }

    private void CheckNextNode()
    {
        //Test to determine what the nature of the next waypoint node is

        //Next waypoint node is a Traffic light
        if (_nodes[_currentNode].gameObject.tag == "TrafficRobot")
        {
            _stopSign = false;

            TrafficSignAttachment _trafficSignAttachmentRef = _nodes[_currentNode].gameObject.GetComponent<TrafficSignAttachment>();
            TrafficRobot_v02 _trafficRobotRefV02 = _trafficSignAttachmentRef._attachment.GetComponent<TrafficRobot_v02>();

            //Logic to tell car how to react to traffic light colour emissions.            
            switch (_trafficRobotRefV02._trafficColorEm)
            {
                    //Red
                    case TrafficRobot_v02.TrafficColorEm.RedEm:
                        
                        if (((_eBrake02Ref._shouldBrake) && (!_isBreaking)) || ((!_isBreaking) && (_distanceToNextWayPoint < _distanceToBreak)))
                        {
                            _isBreaking = true;
                            StartCoroutine(CloseEnoughToTrafficLight());
                        }
                        
                        else
                        {
                            StartCoroutine(CloseEnoughToTrafficLight());
                        }
                        break;

                    //Green
                    case TrafficRobot_v02.TrafficColorEm.GreenEm:
                        _isBreaking = false;
                        _applyHalfBrakes = false;
                        break;

                    //Orange
                    case TrafficRobot_v02.TrafficColorEm.OrangeEm:
                        
                        if ((((_eBrake02Ref._shouldBrake) && (!_isBreaking)) && (_distanceToNextWayPoint > (_distanceToBreak * 0.5f))))
                        {
                            _isBreaking = true;
                            StartCoroutine(CloseEnoughToTrafficLight());
                        }
                       
                        else if (((!_isBreaking) && (_distanceToNextWayPoint < _distanceToBreak) && (_distanceToNextWayPoint > (_distanceToBreak * 0.5f))))
                        {
                            _isBreaking = true;
                            StartCoroutine(CloseEnoughToTrafficLight());
                        }
                        
                        else if (_distanceToNextWayPoint < (_distanceToBreak * 0.5f))
                        {
                            _applyHalfBrakes = false;
                            _isBreaking = false;
                        }
                        else
                        {                            
                            StartCoroutine(CloseEnoughToTrafficLight());
                        }
                        break;

                    //Red and orange (British)
                    case TrafficRobot_v02.TrafficColorEm.RedAndOrange:
                        if (((_eBrake02Ref._shouldBrake) && (!_isBreaking)) || ((!_isBreaking) && (_distanceToNextWayPoint < _distanceToBreak)))
                        {
                            _isBreaking = true;

                            //Coroutine to prevent car from mistaking the old waypoint for the one after it
                            StartCoroutine(CloseEnoughToTrafficLight());
                        }

                        else
                        {
                            //Coroutine to prevent car from mistaking the old waypoint for the one after it
                            StartCoroutine(CloseEnoughToTrafficLight());
                        }
                        break;
                default:
                        break;
                }
            
        }

        //Next waypoint node is a Stop sign 
        else if (_nodes[_currentNode].gameObject.tag == "StopSign")
        {
            
            if (((_eBrake02Ref._shouldBrake) && (!_stopSign)) || ((!_stopSign) && (_distanceToNextWayPoint < _distanceToSlowdown)))
            {
                _stopSign = true;

                //Coroutine to prevent car from mistaking the old waypoint for the one after it
                StartCoroutine(GetPastPrevNode());               
            }
            
        }

        //Generic 'Untagged' waypoint node
        else
        {
            _stopSign = false;
        }
    }

    //====================================

    IEnumerator CloseEnoughToTrafficLight()
    {
        //Traffic Light
        yield return new WaitForSeconds(0.1f);
        if (_distanceToNextWayPoint >= _distanceToBreak)
        {
            _isBreaking = false;
        }

        if(_distanceToNextWayPoint < _distanceToSlowdown)
        {
            if (_currentSpeed > 10)
            {
                _applyHalfBrakes = true;
            }
            else
            {
                _applyHalfBrakes = false;
            }
        }

    }

    //====================================

    IEnumerator GetPastPrevNode()
    {     
        //Stop sign
        if (_nodes[_currentNode].gameObject.tag == "StopSign")
        {
            yield return new WaitForSeconds(0.1f);

            TrafficSignAttachment _trafficSignAttachmentRef = _nodes[_currentNode].gameObject.GetComponent<TrafficSignAttachment>();
            StopSign _stopSignRef = _trafficSignAttachmentRef._attachment.GetComponent<StopSign>();

            // if car is within slow down range AND is outside braking range
            if ((_distanceToNextWayPoint < _distanceToSlowdown) && (_distanceToNextWayPoint > _distanceToBreak))
            {
                //print("427");
                if (_currentSpeed > 10)
                {
                    _applyHalfBrakes = true;
                }
                else
                {
                    _applyHalfBrakes = false;
                }
                //Keep calling this Coroutine untill within braking distance
                StartCoroutine(GetPastPrevNode());
            }
            // if car has too much momentum OR is within braking distance
            else if ((_eBrake02Ref._shouldBrake) || (_distanceToNextWayPoint < _distanceToBreak))
            {
                if (_nodes[_currentNode].gameObject.tag == "StopSign")
                {
                    StartCoroutine(StopForPeriod(_stopSignRef._secondsToWait));
                }
            }
            // car is outside slow down range
            else
            {
                _stopSign = false;
            }
        }

        //Normal waypoint node
        else if (_nodes[_currentNode].gameObject.tag == "Untagged")
        {
            yield return new WaitForSeconds(0.1f);

            if (_eBrake02Ref._shouldBrake)
            {
                if (_currentSpeed > 10)
                {
                    _applyHalfBrakes = true;
                }
                else
                {
                    _applyHalfBrakes = false;
                }
            }
            else
            {
                _applyHalfBrakes = false;
            }
        }
    }

    //====================================

    IEnumerator StopForPeriod(int seconds)
    {
        //Stop at the stop sign for an amount of time, before continuing.
        //Basic stop sign logic
        _isBreaking = true;
        yield return new WaitForSeconds(seconds);
        _isBreaking = false;
        _applyHalfBrakes = false;
    }
}

