using UnityEngine;

[CreateAssetMenu(menuName = "Masks/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Rule")]
    [Range(1, 10)] public int relevantFeatureCount = 2;

    [Header("Club")]
    public int maxCapacity = 20;

    [Header("NPC Flow")]
    public float npcDoorInterval = 0.2f;   // seconds between NPC reaching door
    public float npcMoveMinSpeed = 1.0f;
    public float npcMoveMaxSpeed = 1.8f;

    [Header("Player Timing")]
    public float forwardWalkTime = 6.0f;
    public float walkBackTime = 4.0f;
}