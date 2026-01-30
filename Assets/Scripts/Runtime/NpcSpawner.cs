using System.Collections.Generic;
using UnityEngine;

public class NpcSpawner : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject npcPrefab;
    public Transform spawnPoint;

    private float _t;

    private void Update()
    {
        if (!gameManager || !gameManager.IsRunning) return;

        _t += Time.deltaTime;
        if (_t >= gameManager.CurrentLevel.npcDoorInterval)
        {
            _t = 0f;
            SpawnNpcLevel1();
        }
    }

    private void SpawnNpcLevel1()
    {
        // Level 1: everyone accepted; relevant features always identical to rule.
        var mask = gameManager.CreateNpcMask(accepted: true);

        var go = Instantiate(npcPrefab, spawnPoint.position, Quaternion.identity);
        var npc = go.GetComponent<NpcController>();

        string dbg = gameManager.BuildMaskDebugString(mask);
        npc.Init(gameManager.CurrentLevel.npcMoveSpeed, mask, true, dbg);
        }
}