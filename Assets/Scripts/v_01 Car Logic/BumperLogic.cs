using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperLogic : MonoBehaviour
{
    [Header("ReadOnly")]
    public bool _behindCar;    
    RaycastHit _RayHit;
    public float _distanceToCar, _minDistanceToCar, _origDistToCar;

    [Header("References")]
    public EmergencyBreakLogic _eBrakeRef;
    public CarEngine _carEngineRef;
    public CarEngineMultiPath _carEngineMultiRef;

    //====================================

    void Start ()
    {
        _carEngineRef = gameObject.GetComponentInParent<CarEngine>();
        _carEngineMultiRef = gameObject.GetComponentInParent<CarEngineMultiPath>();
        _eBrakeRef = gameObject.GetComponentInParent<EmergencyBreakLogic>();

        _origDistToCar = _minDistanceToCar;
    }

    //====================================

    void Update ()
    {
        AdjustMinDist();
        TestDistance();
    }

    private void AdjustMinDist()
    {
        if (_eBrakeRef._inirtiaFloat > 1000f)
        {
            _minDistanceToCar = _origDistToCar * 2f;
        }
        else
        {
            _minDistanceToCar = _origDistToCar;
        }
    }

    //====================================

    private void TestDistance()
    {
        //Measure the distance between this bumper and the next car's mesh and apply breaks if too close
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _RayHit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _RayHit.distance, Color.yellow);
            if (_RayHit.collider.gameObject.tag == "Car")
            {
                _behindCar = true;
                _distanceToCar = Vector3.Distance(this.transform.position, _RayHit.collider.transform.position);
                ApplyBreaks(_distanceToCar);

                if (_carEngineRef)
                {
                    _carEngineRef.SetMotorTorque(0f);
                }
                else 
                {
                    _carEngineMultiRef.SetMotorTorque(0f);
                }
            }
            else
            {
                if(_behindCar)
                {
                    _behindCar = false;
                    _distanceToCar = 999f;

                    if (_carEngineRef)
                    {
                        _carEngineRef._isStopped = false;
                    }
                    else
                    {
                        _carEngineMultiRef._isStopped = false;
                    }
                }
            }
        }
        else
        {
            if (_behindCar)
            {
                _behindCar = false;
                _distanceToCar = 999f;

                if (_carEngineRef)
                {
                    _carEngineRef._isStopped = false;
                }
                else
                {
                    _carEngineMultiRef._isStopped = false;
                }
            }
        }
    }

    //====================================

    private void ApplyBreaks(float dist)
    {
        //Logic for using breaks
        if (dist < _minDistanceToCar)
        {

            if (_carEngineRef)
            {
                _carEngineRef._isStopped = true;
                _carEngineRef.SetMotorTorque(0f);
            }
            else
            {
                _carEngineMultiRef._isStopped = true;
                _carEngineMultiRef.SetMotorTorque(0f);
            }
        }
        else 
        {

            if (_carEngineRef)
            {
                _carEngineRef._isStopped = false;
                _carEngineRef.SetMotorTorque(_carEngineRef._torquePower);
            }
            else
            {
                _carEngineMultiRef._isStopped = false;
                _carEngineMultiRef.SetMotorTorque(_carEngineMultiRef._torquePower);
            }
        }  
    }
}
