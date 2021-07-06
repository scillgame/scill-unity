using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SCILL
{
    public class SCILLNotification : MonoBehaviour
    {
        public Image image;

        public Text message;

        private float _startTime;

        // Start is called before the first frame update
        void Start()
        {
            _startTime = Time.time;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - _startTime > 5)
            {
                //Destroy(this.gameObject);
            }
        }
    }
}