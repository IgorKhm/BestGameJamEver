using UnityEngine;

public class StreetLanes : MonoBehaviour
{
    public Transform PlayerStartingPoint;

    public Transform CrowdTopSpawnPoint;
    private float crowdTopSpawnPointY;
    public Transform CrowdBottomSpawnPoint;
    private float crowdBottomSpawnPointY;
    private float crowdSpawnPointX;
    
    public Transform CrowdReturnLane;

    private void Awake()
    {
        crowdTopSpawnPointY = CrowdTopSpawnPoint.position.y;
        crowdBottomSpawnPointY = CrowdBottomSpawnPoint.position.y;
        crowdSpawnPointX = CrowdBottomSpawnPoint.position.x;
    }

    public Vector3 GetRandomNpcSpawnLocation()
    {
        return new Vector3(
            crowdSpawnPointX,
            Random.Range(crowdTopSpawnPointY, crowdBottomSpawnPointY),
            0);
    }
}
