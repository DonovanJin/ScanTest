using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficRobot_v02 : MonoBehaviour
{
    [Header("Input")]
    public int _redWaitTime;
    public int _greenWaitTime;
    public int _orangeWaitTime;
    public int _redAndOrangeWaitTime;
    public int _delay;
    public Material _trafficLightMat;
    public Texture _greenEm, _orangeEm, _redEm, _redAndOrangeEm;
    public bool _automated = true, _switched, _reflectOtherRobotColor = false, _interConnected = true;
    public TrafficRobot_v02 _counterPart; 


    Renderer _rend;

    //====================================

    public enum TrafficColorEm
    {
        GreenEm,
        OrangeEm,
        RedEm,
        RedAndOrange
    };

    public TrafficColorEm _trafficColorEm;

    //====================================

    void Start()
    {
        GetStarted();
    }

    //====================================

    private void Update()
    {
        TrafficLogic();
    }

    //====================================

    private void GetStarted()
    {

        if (_delay > 0)
        {
            _switched = true;
        }
        else
        {
            _switched = false;
        }

        _rend = GetComponent<Renderer>();
        _rend.enabled = true;

        if (!_reflectOtherRobotColor)
        {
            StartCoroutine(Delay(_delay));
        }
    }

    //====================================

    IEnumerator Delay(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (!_interConnected)
        {
            StartCoroutine(SwitchColoursNotConnected());
        }
        else
        {
            StartCoroutine(SwitchColoursInterConnected());
        }
        _switched = false;
    }

    IEnumerator SwitchColoursNotConnected()
    {
        //Automatically switch between colors
        if (_automated)
        {
            if (_trafficColorEm == TrafficColorEm.GreenEm)
            {
                yield return new WaitForSeconds(_greenWaitTime);
                _trafficColorEm = TrafficColorEm.OrangeEm;
            }
            else if (_trafficColorEm == TrafficColorEm.RedEm)
            {
                yield return new WaitForSeconds(_redWaitTime);
                _trafficColorEm = TrafficColorEm.RedAndOrange;
            }
            else if (_trafficColorEm == TrafficColorEm.OrangeEm)
            {
                yield return new WaitForSeconds(_orangeWaitTime);
                _trafficColorEm = TrafficColorEm.RedEm;
            }
            else if (_trafficColorEm == TrafficColorEm.RedAndOrange)
            {
                yield return new WaitForSeconds(_orangeWaitTime);
                _trafficColorEm = TrafficColorEm.GreenEm;
            }

            _switched = false;
        }
    }

    IEnumerator SwitchColoursInterConnected()
    {
        if (_automated)
        {
            if (_trafficColorEm == TrafficColorEm.RedAndOrange)
            {
                yield return new WaitForSeconds(_redAndOrangeWaitTime);
                _trafficColorEm = TrafficColorEm.GreenEm;
            }
            else if (_trafficColorEm == TrafficColorEm.GreenEm)
            {
                yield return new WaitForSeconds(_greenWaitTime);
                _trafficColorEm = TrafficColorEm.OrangeEm;
            }
            else if (_trafficColorEm == TrafficColorEm.OrangeEm)
            {
                yield return new WaitForSeconds(_orangeWaitTime);
                _trafficColorEm = TrafficColorEm.RedEm;
                yield return new WaitForSeconds(_redAndOrangeWaitTime);
                _counterPart._trafficColorEm = TrafficRobot_v02.TrafficColorEm.RedAndOrange;
            }

            _switched = false;
        }
    }

    //====================================

    void TrafficLogic()
    {
        //If a traffic light is supposed to be the same as another
        if (!_reflectOtherRobotColor)
        {
            if (!_switched)
            {
                _switched = true;
                if (!_interConnected)
                {
                    StartCoroutine(SwitchColoursNotConnected());
                }
                else
                {
                    StartCoroutine(SwitchColoursInterConnected());
                }
            }
        }
                
        //else
        //{
        //    if (_counterPart)
        //    {
        //        _trafficColorEm = _counterPart._trafficColorEm;
        //    }
        //}


        //Change emission map based on traffic light condition
        switch (_trafficColorEm)
        {
            case TrafficColorEm.GreenEm:
                _rend.material.SetTexture("_EmissionMap", _greenEm);
                break;
            case TrafficColorEm.OrangeEm:
                _rend.material.SetTexture("_EmissionMap", _orangeEm);
                break;
            case TrafficColorEm.RedEm:
                _rend.material.SetTexture("_EmissionMap", _redEm);
                break;
            case TrafficColorEm.RedAndOrange:
                _rend.material.SetTexture("_EmissionMap", _redAndOrangeEm);
                break;
            default:
                Debug.LogError("A material needs to be set.");
                break;
        }
    }
}
