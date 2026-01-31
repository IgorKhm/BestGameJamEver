using UnityEngine;

[CreateAssetMenu(menuName = "Masks/Level Config")]
public class LevelConfig : ScriptableObject
{
    [Header("Rule")]
    [Range(1, 10)] public int relevantFeatureCount = 2;
    [Range(0, 10)] public int numberOfCorrectFeaturesToPass = 2;

    [Header("Club")]
    public int maxCapacity = 20;

    [Header("NPC Flow")]
    public float npcDoorInterval = 0.2f;   // seconds between NPC reaching door
    public float npcMoveMinSpeed = 1.0f;
    public float npcMoveMaxSpeed = 1.8f;

    [Header("Player Timing")]
    public float forwardWalkTime = 6.0f;
    public float walkBackTime = 4.0f;
    
    [Header("Level 2+ Rejections")]
    [Range(0f, 1f)] public float rejectNpcChance = 0.0f;   // Level1 = 0, Level2 e.g. 0.3
    public float leftDespawnOffset = 5f;                   // how far left of spawn to delete rejected NPCs
    public float rejectedSpeedMultiplier = 1.0f; 
}