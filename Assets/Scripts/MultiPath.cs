using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPath : MonoBehaviour
{
    [Header("Input")]
    public Color _lineColor;
    public float _minDistBeforeNextWaypoint = 1f;
    public bool _closedCircuit;
    public bool _visualize = true;

    public enum WhichSideOfRoad
    {
        Left,
        Right
    };

    public WhichSideOfRoad _whichSideOfRoad;

    [Space]

    [Header("References")]
    MultiPathNodeIdentifier _nodeIdRefOld, _nodeIdRefNew;

    private List<Transform> _nodes = new List<Transform>();

    //====================================

    private void OnDrawGizmos()
    {
        if (_visualize)
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
                _nodeIdRefOld = _nodes[i].gameObject.GetComponent<MultiPathNodeIdentifier>();

                //_nodeIdRefOld._nextNodeIndex = 0;
                _nodeIdRefOld._nextNodeIndexList.Clear();

                _nodeIdRefOld._connectionsCount = 0;

                Vector3 currentNode;
                Vector3 previousNode;

                previousNode = _nodes[i].position;
                //_nodeIdRefOld._nextNodeIndex = i + 1;

                //Draw a line between nodes that are on the main road, branches off to different roads or on the same branching road
                for (int y = 1; y < _nodes.Count; y++)
                {
                    currentNode = _nodes[y].position;

                    _nodeIdRefNew = _nodes[y].gameObject.GetComponent<MultiPathNodeIdentifier>();

                    int _oldNodeID, _newNodeID;

                    _oldNodeID = _nodeIdRefOld._identifier;
                    _newNodeID = _nodeIdRefNew._identifier;

                    //Is the next node on the list the next node the previous one should connect to?
                    if (_newNodeID - _oldNodeID == 1)
                    {
                        //Are the nodes on the same main road or the same branching road?
                        if (((_nodeIdRefNew._lane == 0) || (_nodeIdRefOld._lane == _nodeIdRefNew._lane)))
                        {
                            if (_nodeIdRefOld._connectionsCount < 99)
                            {
                                _nodeIdRefOld._connectionsCount++;
                            }
                            Gizmos.DrawLine(previousNode, currentNode);

                            if (_nodeIdRefNew._overrideBool)
                            {
                                Gizmos.DrawWireSphere(currentNode, _nodeIdRefNew._overrideFloat / 2);
                            }
                            else
                            {
                                Gizmos.DrawWireSphere(currentNode, _minDistBeforeNextWaypoint / 2);
                            }

                            _nodeIdRefOld._nextNodeIndexList.Add(y);
                        }
                        //Is the new node connected to the main road?
                        else if (_nodeIdRefOld._lane == 0 && _nodeIdRefNew._nodeType == MultiPathNodeIdentifier.NodeType.Connector)
                        {
                            if (_nodeIdRefOld._connectionsCount < 99)
                            {
                                _nodeIdRefOld._connectionsCount++;
                            }
                            Gizmos.DrawLine(previousNode, currentNode);

                            if (_nodeIdRefNew._overrideBool)
                            {
                                Gizmos.DrawWireSphere(currentNode, _nodeIdRefNew._overrideFloat / 2);
                            }
                            else
                            {
                                Gizmos.DrawWireSphere(currentNode, _minDistBeforeNextWaypoint / 2);
                            }

                            _nodeIdRefOld._nextNodeIndexList.Add(y);
                        }
                        //If the path is looped
                        if ((_nodes.Count > 1) && (_closedCircuit))
                        {
                            int _tempInt = _nodes.Count - 1;
                            Gizmos.DrawLine(_nodes[0].position, _nodes[_tempInt].position);
                        }
                    }

                    currentNode = _nodes[0].position;
                    MultiPathNodeIdentifier _tempNodeRef = _nodes[0].gameObject.GetComponent<MultiPathNodeIdentifier>();

                    if (_tempNodeRef._overrideBool)
                    {
                        Gizmos.DrawWireSphere(currentNode, _tempNodeRef._overrideFloat / 2);
                    }
                    else
                    {
                        Gizmos.DrawWireSphere(currentNode, _minDistBeforeNextWaypoint / 2);
                    }

                }
            }
        }
    }
}
