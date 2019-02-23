using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedOverwrite : MonoBehaviour
{
    public float _newSpeed;
    
    public float _oldspeed;

    public enum TypeOfOverride
    {
        Temporary,
        Permenant
    };

    public TypeOfOverride _typeOfOverride;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Car")
        {
            CarEngine _carEngineRef = other.gameObject.GetComponentInParent<CarEngine>();

            _oldspeed = _carEngineRef._origMaxSpeed;

            StartCoroutine(AdjustOriginalSpeed(_carEngineRef));
        }
    }

    IEnumerator AdjustOriginalSpeed(CarEngine carengineRef)
    {
        yield return new WaitForSeconds(0.1f);
        carengineRef._origMaxSpeed = _newSpeed;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_typeOfOverride == TypeOfOverride.Temporary)
        {
            if (other.gameObject.tag == "Car")
            {
                CarEngine _carEngineRef = other.gameObject.GetComponentInParent<CarEngine>();

                _carEngineRef._origMaxSpeed = _oldspeed;
            }
        }
    }
}
