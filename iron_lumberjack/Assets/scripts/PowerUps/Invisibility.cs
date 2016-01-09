using UnityEngine;
using System.Collections;

namespace PowerUp
{
    public class Invisibility : IPowerUp
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
                return PowerUpType.Invisibility;
            }
        }
    }
}