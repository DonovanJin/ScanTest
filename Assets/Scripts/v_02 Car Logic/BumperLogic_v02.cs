using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumperLogic_v02 : MonoBehaviour
{
    [Header("Input")]
    public float _minDistanceToCar;

    [Space]

    [Header("ReadOnly")]
    public bool _behindCar;
    RaycastHit _RayHit;
    public float _distanceToCar, _origMinDistToCar;

    [Space]

    [Header("References")]
    public EmergencyBreakLogic_v02 _eBrake02Ref;
    public CarEngine _carEngineRef;
    public CarEngineMultiPath_v02 _carEngineMulti02Ref;

    //====================================

    void Start()
    {
        //fetch references
        _carEngineRef = gameObject.GetComponentInParent<CarEngine>();
        _carEngineMulti02Ref = gameObject.GetComponentInParent<CarEngineMultiPath_v02>();
        _eBrake02Ref = gameObject.GetComponentInParent<EmergencyBreakLogic_v02>();

        _origMinDistToCar = _minDistanceToCar;
    }

    //====================================

    void Update()
    {
        AdjustMinDist();
        TestDistance();
    }

    //If the vehicle accumulates too much momentum when behind another car, increase the distance to brake
    private void AdjustMinDist()
    {
        if (_eBrake02Ref._inirtiaFloat > 1000f)
        {
            _minDistanceToCar = _origMinDistToCar * 2f;
        }
        else
        {
            _minDistanceToCar = _origMinDistToCar;
        }
    }

    //====================================

    private void TestDistance()
    {
        //Measure the distance between this car and the next car, then apply breaks if too close

        //Behind a car
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out _RayHit, Mathf.Infinity))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * _RayHit.distance, Color.yellow);

            //Car gets behind another car
            if (_RayHit.collider.gameObject.tag == "Car")
            {
                _behindCar = true;
                _distanceToCar = _RayHit.distance;
                ApplyBreaks(_distanceToCar);

                if (_carEngineRef)
                {
                    _carEngineRef.SetMotorTorque(0f);
                }
                else
                {
                    _carEngineMulti02Ref.SetMotorTorque(0f);
                }
            }            
            else
            {
                //If this car was behind another car recently
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
                        _carEngineMulti02Ref._isStopped = false;
                    }
                }
            }
        }

        //Not behind another car
        else
        {
            //If this car was behind another car recently
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
                    _carEngineMulti02Ref._isStopped = false;
                }
            }
        }
    }

    //====================================

    private void ApplyBreaks(float dist)
    {
        //Apply brakes when too close to another car
        if (dist < _minDistanceToCar)
        {

            if (_carEngineRef)
            {
                _carEngineRef._isStopped = true;
                _carEngineRef.SetMotorTorque(0f);
            }
            else
            {
                _carEngineMulti02Ref._isStopped = true;
                _carEngineMulti02Ref.SetMotorTorque(0f);
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
                _carEngineMulti02Ref._isStopped = false;
                _carEngineMulti02Ref.SetMotorTorque(_carEngineMulti02Ref._torquePower);
            }
        }
    }
}
