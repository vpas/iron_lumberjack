using UnityEngine;
using System.Collections;

public enum PowerUpType { Haste, VisionBonus, TrueSight, Invisibility }
public interface IPowerUp {
    PowerUpType PowerUpType { get; }
    float PowerUpDuration { get; }
    float PowerUpStartTime { get; set; }
}
