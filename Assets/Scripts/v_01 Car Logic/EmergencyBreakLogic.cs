using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergencyBreakLogic : MonoBehaviour
{
    [Header("References")]
    public float _inirtiaFloat;
    public Rigidbody _rigidBody;
    public CarEngineMultiPath _carEngineMultiPathRef;
    public float _origDistToBrake;   

    [Header("Version02")]
    public float _force;
    public bool _shouldBrake = false;

    private void Start()
    {
        _carEngineMultiPathRef = this.gameObject.GetComponent<CarEngineMultiPath>();

        _origDistToBrake = _carEngineMultiPathRef._distanceToBreak;        
    }

    void Update ()
    {
        CalculateInertia();
    }

    //Calculates an arbitrary float value to determine if car should apply double brake power
    private void CalculateInertia()
    {
        _inirtiaFloat = (_rigidBody.mass * _carEngineMultiPathRef._currentSpeed) * 0.1f;
        //f = 0.5 * m * v0^2 / (x0 - xs)
        _force = (float)(0.5f * _rigidBody.mass * _carEngineMultiPathRef._currentSpeed / _carEngineMultiPathRef._distanceToNextWayPoint);

        if (_force > _carEngineMultiPathRef._maxBreakTorque)
        {
            _shouldBrake = true;
        }
        else
        {
            _shouldBrake = false;
        }
    }
}
