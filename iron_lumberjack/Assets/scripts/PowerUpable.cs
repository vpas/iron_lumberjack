using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IPowerUpable {
    void ApplyPowerup(PowerUpType powerUp);
    bool IsAffectedByPowerUp(PowerUpType powerUp);
    float GetPowerUpRemainingTime(PowerUpType powerUp);
}
