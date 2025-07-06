using UnityEngine;
using System.Collections.Generic;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public int numberToSpawn = 10;
    public List<Transform> spawnPoints;

    void Start()
    {
        for (int i = 0; i < numberToSpawn; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];
            Vector3 spawnPos = point.position + Random.insideUnitSphere * 10f;
            spawnPos.y = point.position.y;

            GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.Euler(0,0,0));
        }
    }
}
