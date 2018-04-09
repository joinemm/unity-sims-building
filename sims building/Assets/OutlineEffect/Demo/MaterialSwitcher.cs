using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OutlineNamespace
{
    public class MaterialSwitcher : MonoBehaviour
    {
        public Material target;
        public int index;

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.M))
            {
                Material[] materials = GetComponent<Renderer>().materials;
                materials[index] = target;
                GetComponent<Renderer>().materials = materials;
            }
        }
    }
}