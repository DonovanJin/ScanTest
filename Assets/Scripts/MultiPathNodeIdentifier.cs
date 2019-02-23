using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPathNodeIdentifier : MonoBehaviour
{
    //No logic, just a script to keep variables

    [Header("Input")]
    //public int _nextNodeIndex;
    public int _identifier;
    public int _connectionsCount;
    public int _lane;
    public int _newOriginalSpeed;
    public bool _overrideSpeed = false;
    public bool _overrideBool = false;
    public float _overrideFloat;
    
    public enum NodeType
    {
        Branch,
        Connector
    };

    public NodeType _nodeType;

    [Space]

    [Header("ReadOnly")]
    public List<int> _nextNodeIndexList = new List<int>();

    private void Start()
    {
        MeshRenderer _meshRend = this.gameObject.GetComponent<MeshRenderer>();
        _meshRend.enabled = false;
    }
}
