using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QLearningAgent : MonoBehaviour
{
    public float learningRate = 0.1f;
    public float discountFactor = 0.9f;
    public float epsilon = 0.2f; 

    private Dictionary<string, float[]> qTable = new Dictionary<string, float[]>();
    private int actionCount = 4; 

    public string targetTag = "Player"; 
    private Transform enemyTransform;

    void Start()
    {
        FindEnemy();
    }

    void Update()
    {
        if (enemyTransform == null) { FindEnemy(); return; }

        string currentState = GetState();
        int action = SelectAction(currentState);
        ExecuteAction(action);

        string nextState = GetState();
        float reward = CalculateReward();

        UpdateQTable(currentState, action, reward, nextState);
    }

    void FindEnemy()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(targetTag);
        foreach (GameObject p in players)
        {
            if (p != this.gameObject)
            {
                enemyTransform = p.transform;
                break;
            }
        }
    }

    string GetState()
    {
        Vector3 direction = (enemyTransform.position - transform.position).normalized;
        return $"X:{Mathf.RoundToInt(direction.x)}Z:{Mathf.RoundToInt(direction.z)}";
    }

    int SelectAction(string state)
    {
        if (Random.value < epsilon)
        {
            return Random.Range(0, actionCount);
        }

        if (!qTable.ContainsKey(state))
            qTable[state] = new float[actionCount];

        float[] actions = qTable[state];
        int bestAction = 0;
        for (int i = 1; i < actions.Length; i++)
        {
            if (actions[i] > actions[bestAction]) bestAction = i;
        }
        return bestAction;
    }

    void ExecuteAction(int action)
    {
        float step = 2f * Time.deltaTime;
        switch (action)
        {
            case 0: transform.Translate(Vector3.forward * step); 
            break;
            case 1: transform.Translate(Vector3.back * step); 
            break;
            case 2: transform.Translate(Vector3.right * step); 
            break;
            case 3: transform.Translate(Vector3.left * step); 
            break;
        }
    }

    float CalculateReward()
    {
        float distance = Vector3.Distance(transform.position, enemyTransform.position);
        return -distance; 
    }

    void UpdateQTable(string state, int action, float reward, string nextState)
    {
        if (!qTable.ContainsKey(state)) qTable[state] = new float[actionCount];
        if (!qTable.ContainsKey(nextState)) qTable[nextState] = new float[actionCount];

        float maxNextQ = Mathf.Max(qTable[nextState]);
        qTable[state][action] += learningRate * (reward + discountFactor * maxNextQ - qTable[state][action]);
    }
}