using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialOnCollision : MonoBehaviour {

    public Material[] material;
    Renderer rend;


	// Use this for initialization
	void Start () {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = material[0];
		
	}

   void OnCollisionEnter(Collision col)
    {
       if (col.gameObject.tag == "MtlChangeObject")
        {
            rend.sharedMaterial = material[1];
        }
        else
        {
            rend.sharedMaterial = material[2];
        }
    }

}
