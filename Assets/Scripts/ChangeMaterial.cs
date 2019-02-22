using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour {

    public Material[] material;
    public Material defaultColour;
    public Material GreenGlow;
    public Material RedGlow;

    Renderer rend;


    // Use this for initialization
    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        rend.sharedMaterial = defaultColour;

    }

    private void Update()
    {
        if(Input.GetMouseButton(0))
        {
            rend.sharedMaterial = GreenGlow;
        }

        if (Input.GetMouseButton(1))
        {
            rend.sharedMaterial = RedGlow;
        }
    }


   // void OnCollisionEnter(Collision col)
   // {
   //     if (col.gameObject.tag == "MtlChangeObject")
   //     {
   //         rend.sharedMaterial = material[1];
   //     }
  //      else
  //      {
  //          rend.sharedMaterial = material[2];
  //      }
  //  }
}
