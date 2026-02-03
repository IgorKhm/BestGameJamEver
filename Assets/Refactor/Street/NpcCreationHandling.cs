using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class NpcCreationHandling : MonoBehaviour
{
    public bool ShouldSpawn = false;
    [SerializeField] private StreetLanes streetLanes;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject npcPrefab;

    [SerializeField] private float spawnFrequency;
    private ObjectPool<GameObject> npcs;

    private void Awake()
    {
        npcs = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(npcPrefab, this.transform),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => Destroy(obj),
            collectionCheck: false,
            defaultCapacity: 10
        );
    }

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        if (npcPrefab != null)
        {
            while (ShouldSpawn)
            {
                GameObject npc = npcs.Get();
                npc.transform.position = streetLanes.GetRandomNpcSpawnLocation();
                npc.transform.rotation = Quaternion.identity;

                yield return new WaitForSeconds(spawnFrequency);
            }
        }
    }

    public void ReleaseNpc(GameObject npc)
    {
        npcs.Release(npc);
    }
}
