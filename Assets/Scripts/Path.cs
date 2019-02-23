using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    [Header("Input")]
    public Color _lineColor;    
    public float _minDistBeforeNextWaypoint = 1f;

    private List<Transform> _nodes = new List<Transform>();

    //====================================

    private void OnDrawGizmos() 
    {
        Gizmos.color = _lineColor;

        //Create an empty array of Transforms
        Transform[] _pathTransform = GetComponentsInChildren<Transform>();
        _nodes = new List<Transform>();

        //Populate previous array with the children's transforms, but ignore the perant's
        for (int i = 0; i < _pathTransform.Length; i++)
        {
            if (_pathTransform[i] != transform)
            {
                _nodes.Add(_pathTransform[i]);
            }
        }        

        //Draw a line between each node
        for (int i = 0; i < _nodes.Count; i++)
        {
            Vector3 currentNode = _nodes[i].position;
            Vector3 previousNode = Vector3.zero;

            //Create a looped path:

            //Connect a line between each node
            if (i > 0)
            {
                previousNode = _nodes[i - 1].position;
            }
                        
            //Connect a line between first and last nodes
            else if (i == 0 && _nodes.Count > 1)
            {
                previousNode = _nodes[_nodes.Count - 1].position;
            }

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, _minDistBeforeNextWaypoint/2);            
        }
    }
}
