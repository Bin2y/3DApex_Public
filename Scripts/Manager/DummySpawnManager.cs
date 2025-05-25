using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DummySpawnManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("Dummy Prefab")]
    public GameObject dummyPrefab;
    public Transform dummyRoot;

    [Tooltip("플레이어")]
    public Transform player;

    [Tooltip("웨이브 대기 시간")]
    public float timeBetweenWaves = 30f;

    [Tooltip("웨이브 당 생성할 Dummy 수")]
    public int dummiesPerWave = 2;

    [Tooltip("웨이브 내에서 Dummy 간 생성 간격")]
    public float spawnInterval = 0.5f;

    [Header("Spawn Radius")]
    [Tooltip("플레이어로부터 최소 거리")]
    public float minSpawnRadius = 200f;

    [Tooltip("플레이어로부터 최대 거리")]
    public float maxSpawnRadius = 400f;

    private Coroutine waveRoutine;

    private void OnEnable()
    {
        //스폰 시작
        waveRoutine = StartCoroutine(SpawnWaves());
    }

    private void OnDisable()
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);
    }

    private IEnumerator SpawnWaves()
    {
        while (true)
        {
            yield return SpawnSingleWave();
            yield return new WaitForSeconds(timeBetweenWaves);
        }
    }

    private IEnumerator SpawnSingleWave()
    {
        for (int i = 0; i < dummiesPerWave; i++)
        {
            Vector3 spawnPos = GetRandomPointAroundPlayer();
            GameObject go = Instantiate(dummyPrefab, spawnPos, Quaternion.identity);

            var agent = go.GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                //navmeshWarp을 이용해서 올려준다
                agent.Warp(spawnPos);
            }
            go.transform.SetParent(dummyRoot);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomPointAroundPlayer()
    {
        NavMeshHit hit;
        //여러 번 시도
        for (int i = 0; i < 10; i++)
        {
            float dist = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 dir = Random.onUnitSphere;
            dir.y = 0;
            Vector3 candidate = player.position + dir.normalized * dist;

            if (NavMesh.SamplePosition(candidate, out hit, maxSpawnRadius, NavMesh.AllAreas))
                return hit.position;
        }
        //실패시에 플레이어 최소반경에 스폰
        return player.position + player.forward * minSpawnRadius;
    }
}

