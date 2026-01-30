using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    public GameManager gameManager;
    public Transform topSpawnPoint, bottomSpawnPoint;
    public GameObject npcPrefab;
    private BoxCollider2D npcCollider;

    [SerializeField] private float _spawnFrequency;

    private void Awake()
    {
        Random.InitState(System.DateTime.Now.Millisecond);
        npcCollider = npcPrefab.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!gameManager || !gameManager.IsRunning) return;

        _spawnFrequency += Time.deltaTime;
        if (_spawnFrequency >= gameManager.CurrentLevel.npcDoorInterval)
        {
            _spawnFrequency = 0f;
            SpawnNpc();
        }
    }

    private void SpawnNpc()
    {
        bool accepted = Random.value >= gameManager.CurrentLevel.rejectNpcChance;

        var mask = gameManager.CreateNpcMask(accepted);


        var spawnPoint = GetNpcSpawnerLocation();
        var go = Instantiate(npcPrefab, spawnPoint, Quaternion.identity);
        var npc = go.GetComponent<NpcController>();
        float despawnX = spawnPoint.x - gameManager.CurrentLevel.leftDespawnOffset;
        float npcRandomMoveSpeed = Random.Range(gameManager.CurrentLevel.npcMoveMinSpeed, gameManager.CurrentLevel.npcMoveMaxSpeed);

        string dbg = ""; // optional: show relevant features for debugging
        // string dbg = gameManager.BuildMaskDebugString(mask);

        npc.Init(
            npcRandomMoveSpeed,
            mask,
            accepted,
            dbg,
            despawnX,
            gameManager.CurrentLevel.rejectedSpeedMultiplier
        );
    }

    private Vector3 GetNpcSpawnerLocation()
    {
        float spawnVerticalValue = Random.Range(topSpawnPoint.position.y, bottomSpawnPoint.position.y);
        spawnVerticalValue -= (spawnVerticalValue % (npcCollider.size.y / 2) + Random.Range(npcCollider.size.y * -0.1f, npcCollider.size.y * 0.1f));
        float spawnHorizontalValue = Random.Range(topSpawnPoint.position.x, bottomSpawnPoint.position.x);
        Vector3 randomSpawnLocation = new Vector3(spawnHorizontalValue, spawnVerticalValue, 0);
        return randomSpawnLocation;
    }
}