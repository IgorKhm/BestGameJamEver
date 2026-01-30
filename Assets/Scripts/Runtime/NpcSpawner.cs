using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    public static NpcSpawner Instance;
    public GameManager gameManager;
    public Transform topSpawnPoint, bottomSpawnPoint;
    public GameObject npcPrefab;
    private BoxCollider2D npcCollider;

    private float _spawnFrequency;

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
        MaskData mask = gameManager.CreateNpcMask(accepted: true);

        GameObject instantiatedNpcPrefab = Instantiate(npcPrefab, GetNpcSpawnerLocation(), Quaternion.identity);
        NpcController npcController = instantiatedNpcPrefab.GetComponent<NpcController>();

        string dbg = gameManager.BuildMaskDebugString(mask, showAll: true);
        float npcRandomMoveSpeed = Random.Range(gameManager.CurrentLevel.npcMoveMinSpeed, gameManager.CurrentLevel.npcMoveMaxSpeed);
        SetOrderInLayerForNpc(instantiatedNpcPrefab);
        npcController.Init(npcRandomMoveSpeed, mask, true, dbg);
    }

    public void SetOrderInLayerForNpc(GameObject npc)
    {
        SpriteRenderer[] sprites = npc.GetComponentsInChildren<SpriteRenderer>();
        float uiOrderInLayer = (npc.transform.position.y - bottomSpawnPoint.position.y) / 
                                (topSpawnPoint.position.y - bottomSpawnPoint.position.y);
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.sortingOrder = (int)((1 - uiOrderInLayer) * 100);
        }

    }

    private Vector3 GetNpcSpawnerLocation()
    {
        float spawnVerticalValue = Random.Range(topSpawnPoint.position.y, bottomSpawnPoint.position.y);
        spawnVerticalValue -= (spawnVerticalValue % (npcCollider.size.y / 2));
        float spawnHorizontalValue = Random.Range(topSpawnPoint.position.x, bottomSpawnPoint.position.x);
        Vector3 randomSpawnLocation = new Vector3(spawnHorizontalValue, spawnVerticalValue, 0);
        return randomSpawnLocation;
    }
}