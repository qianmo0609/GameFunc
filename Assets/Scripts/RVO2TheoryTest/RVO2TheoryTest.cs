using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RVO2TheoryTest : MonoBehaviour
{
    [Header("Settings")]
    public GameObject agentPrefab;
    public int agentCount = 10;
    public float spawnRadius = 10f;
    public Transform goal;

    private List<RVOAgent> agents = new List<RVOAgent>();

    void Start()
    {
        SpawnAgents();
    }

    void SpawnAgents()
    {
        for (int i = 0; i < agentCount; i++)
        {
            Vector3 pos = Random.insideUnitSphere * spawnRadius;
            pos.y = 0;
            GameObject agent = Instantiate(agentPrefab, pos, Quaternion.identity);
            RVOAgent rvo = agent.GetComponent<RVOAgent>();
            agents.Add(rvo);
        }
    }

    public Vector3 GetGoalPosition()
    {
        return goal.position;
    }
}
