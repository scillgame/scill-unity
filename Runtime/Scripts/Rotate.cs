using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCILL
{
    public class Rotate : MonoBehaviour
    {
        public float anglePerSeconds = 5;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(Vector3.up, anglePerSeconds * Time.deltaTime);
        }
    }
}