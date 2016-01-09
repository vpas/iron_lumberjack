using UnityEngine;
using System.Collections;
using System;

namespace PowerUp
{
    public class Haste : IPowerUp
    {
        float duration = -1;
        public float PowerUpDuration
        {
            get
            {
                if (duration < 0)
                    return Parameters.DefaultPowerUpDuration;
                return duration;
            }
        }

        float _powerUpStartTime;
        public float PowerUpStartTime
        {
            get
            {
                return _powerUpStartTime;
            }

            set
            {
                _powerUpStartTime = value;
            }
        }

        public PowerUpType PowerUpType
        {
            get
            {
                return PowerUpType.Haste;
            }
        }


        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
