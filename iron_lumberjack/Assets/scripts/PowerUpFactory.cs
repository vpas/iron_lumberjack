using UnityEngine;
using System.Collections;

public static class PowerUpFactory {

    public static IPowerUp CreatePowerUp(PowerUpType powerUpType)
    {
        switch (powerUpType)
        {
            case PowerUpType.Haste:
                return new PowerUp.Haste();
            case PowerUpType.Invisibility:
                return new PowerUp.Invisibility();
            case PowerUpType.TrueSight:
                return new PowerUp.TrueSight();
            case PowerUpType.VisionBonus:
                return new PowerUp.Vision();
        }
        return new PowerUp.Haste();
    }
}
