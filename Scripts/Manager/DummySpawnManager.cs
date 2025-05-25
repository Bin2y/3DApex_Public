using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DummySpawnManager : MonoBehaviour
{
    [Header("Spawner Settings")]
    [Tooltip("Dummy Prefab")]
    public GameObject dummyPrefab;
    public Transform dummyRoot;

    [Tooltip("�÷��̾�")]
    public Transform player;

    [Tooltip("���̺� ��� �ð�")]
    public float timeBetweenWaves = 30f;

    [Tooltip("���̺� �� ������ Dummy ��")]
    public int dummiesPerWave = 2;

    [Tooltip("���̺� ������ Dummy �� ���� ����")]
    public float spawnInterval = 0.5f;

    [Header("Spawn Radius")]
    [Tooltip("�÷��̾�κ��� �ּ� �Ÿ�")]
    public float minSpawnRadius = 200f;

    [Tooltip("�÷��̾�κ��� �ִ� �Ÿ�")]
    public float maxSpawnRadius = 400f;

    private Coroutine waveRoutine;

    private void OnEnable()
    {
        //���� ����
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
                //navmeshWarp�� �̿��ؼ� �÷��ش�
                agent.Warp(spawnPos);
            }
            go.transform.SetParent(dummyRoot);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private Vector3 GetRandomPointAroundPlayer()
    {
        NavMeshHit hit;
        //���� �� �õ�
        for (int i = 0; i < 10; i++)
        {
            float dist = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector3 dir = Random.onUnitSphere;
            dir.y = 0;
            Vector3 candidate = player.position + dir.normalized * dist;

            if (NavMesh.SamplePosition(candidate, out hit, maxSpawnRadius, NavMesh.AllAreas))
                return hit.position;
        }
        //���нÿ� �÷��̾� �ּҹݰ濡 ����
        return player.position + player.forward * minSpawnRadius;
    }
}

