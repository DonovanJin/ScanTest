using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyBreakLogic_v02 : MonoBehaviour
{
    [Header("ReadOnly Please Ignore")]
    public float _inirtiaFloat;
    public float _origDistToBrake;

    [Space]

    [Header("References")]    
    public Rigidbody _rigidBody;
    public CarEngineMultiPath_v02 _carEngineMultiPath02Ref;

    [Space]

    [Header("Version02")]
    public float _force;
    public float _force02;
    public bool _shouldBrake = false;

    private void Start()
    {
        //fetch referenes
        _rigidBody = this.gameObject.GetComponent<Rigidbody>();
        _carEngineMultiPath02Ref = this.gameObject.GetComponent<CarEngineMultiPath_v02>();

        _origDistToBrake = _carEngineMultiPath02Ref._distanceToBreak;
    }

    void Update()
    {
        CalculateInertia();
    }

    //Calculates an arbitrary float value to represent vehicle's momentum
    private void CalculateInertia()
    {
        _inirtiaFloat = (_rigidBody.mass * _carEngineMultiPath02Ref._currentSpeed) * 0.1f;

        _force02 = (float)((_carEngineMultiPath02Ref._currentSpeed * 0.1) * (_carEngineMultiPath02Ref._currentSpeed * 0.1));
        _force02 *= 3f;        

        _force = (float)(0.5f * _rigidBody.mass * _carEngineMultiPath02Ref._currentSpeed / _carEngineMultiPath02Ref._distanceToNextWayPoint);

        //If the distance required to stop is smaller than the actual distance
        if (_force > (_carEngineMultiPath02Ref._maxBreakTorque * 1f))
        {
            if (_force02 >= _carEngineMultiPath02Ref._distanceToNextWayPoint)
            {
                _shouldBrake = true;
            }
            else
            {
                _shouldBrake = false;
            }
        }
        else
        {
            _shouldBrake = false;
        }
    }
}
