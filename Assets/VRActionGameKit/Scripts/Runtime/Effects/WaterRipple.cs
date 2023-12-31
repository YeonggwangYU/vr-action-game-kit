﻿using UnityEngine;

namespace _3DGamekitLite.Scripts.Runtime.Effects
{
    [ExecuteInEditMode]
    public class WaterRipple : MonoBehaviour
    {

        Material mat;
        public Transform offsetPos;

        // Use this for initialization
        void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            mat.SetVector("_UVOffset", new Vector4(offsetPos.position.x, 0, offsetPos.position.z, 1));
        }
    } 
}
