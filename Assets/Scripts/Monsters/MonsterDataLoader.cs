using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MonsterDataLoader : MonoBehaviour
{
    private const string MonsterDataURL =
        "https://docs.google.com/spreadsheets/d/e/2PACX-1vTz_J-VfZTNnDFXDLaFPcWUejct6t5Jjw9rRNuvDd3AgsMcmP5QnCRKYkxlq5wRqYe-YRPmaz9buMsO/pub?gid=0&single=true&output=csv";
        
    public List<MonsterSO> MonsterData = new List<MonsterSO>();
    
    void Start()
    {
        StartCoroutine(LoadMonsterData());
    }
    
    private IEnumerator LoadMonsterData()
    {
        UnityWebRequest request = UnityWebRequest.Get(MonsterDataURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            ParseMonsterData(request.downloadHandler.text);
        }
    }

    private void ParseMonsterData(string csvData)
    {
        string[] rows = csvData.Split('\n');
        for (int i = 1; i < rows.Length; i++) // Skip header row
        {
            if (string.IsNullOrWhiteSpace(rows[i])) continue;
            string[] cols = rows[i].Split(',');

            // Example of creating MonsterSO dynamically
            MonsterSO monster = ScriptableObject.CreateInstance<MonsterSO>();
            monster.name = cols[1].Trim(); // Name
            monster.prefab = Resources.Load<GameObject>(cols[2].Trim()); // Prefab name
            monster.fatigue = float.Parse(cols[3]); // Fatigue
            monster.fearInflicted = float.Parse(cols[4]); // Fear Inflicted
            monster.cooldown = float.Parse(cols[5]); // Cooldown
            monster.humanScaringRange = float.Parse(cols[6]); // Scaring Range
            monster.requiredCoins = float.Parse(cols[7]); // Required Coins

            MonsterData.Add(monster);
        }
    }
}