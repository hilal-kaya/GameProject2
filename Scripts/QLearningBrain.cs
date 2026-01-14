using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class QLearningBrain : MonoBehaviour
{
    public float learningRate = 0.1f;
    public float discount = 0.95f;
    public float exploration = 0.2f;
    public string saveFileName = "RLBrain.json";

    private List<float> currentInputs = new();   
    private List<ActionDefinition> actions = new();

    private Dictionary<string, float[]> Q = new();

    private string savePath;
    

    [Serializable]
    public class ActionDefinition
    {
        public string actionName;
        public Action<object[]> method;
        public int parameterCount;

        public ActionDefinition(string name, Action<object[]> func, int paramCount)
        {
            actionName = name;
            method = func;
            parameterCount = paramCount;
        }
    }

    [Serializable]
    private class SaveModel
    {
        public int inputCount;
        public int actionCount;
        public Dictionary<string, float[]> qTable;
    }

    void Awake()
    {
        savePath = Path.Combine(Application.dataPath, saveFileName);
        LoadOrCreateModel();
    }

    public void SetInputs(List<float> inputs)
    {
        currentInputs = inputs;
    }

    public void RegisterAction(string name, Action<object[]> method, int parameterCount)
    {
        actions.Add(new ActionDefinition(name, method, parameterCount));
    }

    public int DecideAction()
    {
        string state = EncodeState(currentInputs);
        EnsureStateExists(state);

        if (UnityEngine.Random.value < exploration)
        {
            return UnityEngine.Random.Range(0, actions.Count);
        }

        float[] qRow = Q[state];

        int bestIndex = 0;
        float bestVal = qRow[0];

        for (int i = 1; i < qRow.Length; i++)
        {
            if (qRow[i] > bestVal)
            {
                bestVal = qRow[i];
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    public void ExecuteAction(int actionIndex, params object[] parameters)
    {
        actions[actionIndex].method.Invoke(parameters);
    }

    public void Reward(float value)
    {
        ApplyReward(value);
    }

    public void Punish(float value)
    {
        ApplyReward(-Mathf.Abs(value));
    }

    private void ApplyReward(float reward)
    {
        string oldState = EncodeState(currentInputs);
        EnsureStateExists(oldState);

        int action = DecideAction(); 

        float[] qRow = Q[oldState];

        string newState = EncodeState(currentInputs);
        EnsureStateExists(newState);

        float maxNext = Max(Q[newState]);

        qRow[action] = qRow[action] + learningRate * (reward + discount * maxNext - qRow[action]);

        SaveModelToFile();
    }

    private float Max(float[] arr)
    {
        float m = arr[0];
        for (int i = 1; i < arr.Length; i++)
            if (arr[i] > m) m = arr[i];
        return m;
    }

    private void EnsureStateExists(string state)
    {
        if (!Q.ContainsKey(state))
        {
            Q[state] = new float[actions.Count];
        }
    }

    private string EncodeState(List<float> inputs)
    {
        return string.Join("_", inputs);
    }

    private void LoadOrCreateModel()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("RLBrain: Creating new model file.");
            CreateNewModel();
            return;
        }

        string json = File.ReadAllText(savePath);
        var model = JsonUtility.FromJson<SaveModel>(json);

        // Compare structure
        if (model.inputCount != currentInputs.Count ||
            model.actionCount != actions.Count)
        {
            Debug.LogWarning("RLBrain: Structure changed. Creating new model.");
            CreateNewModel();
            return;
        }

        Q = model.qTable;
        Debug.Log("RLBrain: Model loaded.");
    }

    private void CreateNewModel()
    {
        Q = new Dictionary<string, float[]>();
        SaveModelToFile();
    }

    private void SaveModelToFile()
    {
        var model = new SaveModel
        {
            inputCount = currentInputs.Count,
            actionCount = actions.Count,
            qTable = Q
        };

        string json = JsonUtility.ToJson(model, true);
        File.WriteAllText(savePath, json);
    }

    public bool isBrainLoaded = false;
    
    public void LoadModelFromJson(string jsonContent)
    {
        try
        {
            // JSON içeriğini senin tanımladığın SaveModel yapısına çeviriyoruz
            SaveModel model = JsonUtility.FromJson<SaveModel>(jsonContent);
            
            if (model != null && model.qTable != null)
            {
                this.Q = model.qTable;
                this.exploration = 0.1f; // Zeka yüklendiği için rasyonel moda geç
                Debug.Log("QLearningBrain: Yapay zeka ağırlıkları başarıyla yüklendi.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("QLearningBrain: JSON yükleme hatası: " + e.Message);
        }
    }
}