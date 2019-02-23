using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCameras : MonoBehaviour
{
    public int _selectedCamIndex = 0;
    public Camera[] _cameras;

    void Start()
    {
        SwitchToCamera();
    }

    void Update()
    {
        ChooseCamera();
    }

    public void ChooseCamera()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedCamIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedCamIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedCamIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedCamIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedCamIndex = 4;
        }

        SwitchToCamera();
    }

    public void SwitchToCamera()
    {
        if ((_selectedCamIndex <= _cameras.Length - 1) && (_selectedCamIndex >= 0))
        {
            if (_cameras.Length > 1)
            {
                for (int i = 0; i < _cameras.Length; i++)
                {
                    //Switch off all camera not selected
                    if (i != _selectedCamIndex)
                    {
                        _cameras[i].enabled = false;
                    }
                    //Switch on selected camera
                    else
                    {
                        _cameras[i].enabled = true;
                    }
                }
            }            
        }
        else
        {
            print("Out of range.");
            _selectedCamIndex = 0;
        }
    }
}
