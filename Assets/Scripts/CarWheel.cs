using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour {

    public WheelCollider _targetWheel;

    private Vector3 _wheelPosition = new Vector3();
    private Quaternion _wheelRotation = new Quaternion();

	void Update ()
    {
        _targetWheel.GetWorldPose(out _wheelPosition,out _wheelRotation);
        transform.position = _wheelPosition;
        transform.rotation = _wheelRotation;
	}
}
