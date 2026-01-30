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
            CreateNpcLevel1();
        }
    }

    private void CreateNpcLevel1()
    {
        // Level 1: everyone accepted; relevant features always identical to rule.
        MaskData mask = gameManager.CreateNpcMask(accepted: true);

        var go = Instantiate(npcPrefab, GetNpcSpawnerLocation(), Quaternion.identity);
        var npc = go.GetComponent<NpcController>();

        string dbg = gameManager.BuildMaskDebugString(mask);
        float npcRandomMoveSpeed = Random.Range(gameManager.CurrentLevel.npcMoveMinSpeed, gameManager.CurrentLevel.npcMoveMaxSpeed); 
        npc.Init(npcRandomMoveSpeed, mask, true, dbg);
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