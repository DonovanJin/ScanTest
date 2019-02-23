using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficRobot : MonoBehaviour
{
    [Header("Input")]
    public int _redWaitTime;
    public int _greenWaitTime;
    public int _orangeWaitTime;
    public int _delay;
    public Material _green, _orange, _red;
    public bool _automated, _switched, _reflectOtherRobotColor = false;
    public TrafficRobot _counterPart;
        
    [Header("ReadOnly")]
    public bool _interConnected;
    

    Renderer _rend;

    //====================================

    public enum TrafficColor
    {
        Green,
        Orange,
        Red
    };    

    public TrafficColor _trafficColor;

    //====================================

    void Start ()
    {       
       GetStarted();        
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
            if (_trafficColor == TrafficColor.Green)
            {
                yield return new WaitForSeconds(_greenWaitTime);
                _trafficColor = TrafficColor.Orange;
            }
            else if (_trafficColor == TrafficColor.Red)
            {
                yield return new WaitForSeconds(_redWaitTime);
                _trafficColor = TrafficColor.Green;
            }
            else
            {
                yield return new WaitForSeconds(_orangeWaitTime);
                _trafficColor = TrafficColor.Red;
            }

            _switched = false;
        }        
    }

    IEnumerator SwitchColoursInterConnected()
    {
        if (_automated)
        {
            if (_trafficColor == TrafficColor.Green)
            {
                yield return new WaitForSeconds(_greenWaitTime);
                _trafficColor = TrafficColor.Orange;
            }
            else if (_trafficColor == TrafficColor.Orange)
            {
                yield return new WaitForSeconds(_orangeWaitTime);
                _trafficColor = TrafficColor.Red;
                yield return new WaitForSeconds(_orangeWaitTime);
                _counterPart._trafficColor = TrafficRobot.TrafficColor.Green;
            }

            _switched = false;
        }
    }

    //====================================

    void Update ()
    {
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
        else
        {
            if (_counterPart)
            {
                _trafficColor = _counterPart._trafficColor;
            }
        }

        switch (_trafficColor)
        {
            case TrafficColor.Green:
                _rend.sharedMaterial = _green;
                break;
            case TrafficColor.Orange:
                _rend.sharedMaterial = _orange;
                break;
            case TrafficColor.Red:
                _rend.sharedMaterial = _red;
                break;
            default:
                break;
        }
        //else if (_counterPart)
        //{
        //    //_rend.sharedMaterial = _counterPart._rend.sharedMaterial;
        //    _trafficColor = _counterPart._trafficColor;

        //    switch (_trafficColor)
        //    {
        //        case TrafficColor.Green:
        //            _rend.sharedMaterial = _green;
        //            break;
        //        case TrafficColor.Orange:
        //            _rend.sharedMaterial = _orange;
        //            break;
        //        case TrafficColor.Red:
        //            _rend.sharedMaterial = _red;
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }
}
