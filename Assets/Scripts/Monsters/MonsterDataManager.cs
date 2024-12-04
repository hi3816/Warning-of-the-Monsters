using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MonsterDataManager : MonoBehaviour
{
    public static MonsterDataManager Instance { get; private set; }
    
   [MenuItem("Tools/Update MonsterSO from JSON")]
   
   private void Awake()
   {
       if (Instance != null && Instance != this)
       {
           Destroy(gameObject);
           return;
       }
       Instance = this;
   }
   
    public static void UpdateMonsterSOFromGoogleSheets()
    {
        List<Monster_Data.Monster_Data> monsterDataList = Monster_Data.Monster_Data.GetList();

        string savePath = "Assets/SOs/Monster/";

        foreach (Monster_Data.Monster_Data monsterData in monsterDataList)
        {
            string soPath = $"{savePath}MonsterSO_{monsterData.id}.asset";
            MonsterSO monsterSO = AssetDatabase.LoadAssetAtPath<MonsterSO>(soPath);

            if (monsterSO == null)
            {
                monsterSO = ScriptableObject.CreateInstance<MonsterSO>();
                AssetDatabase.CreateAsset(monsterSO, soPath);
            }

            monsterSO.id = monsterData.id;
            monsterSO.poolTag = monsterData.name;
            monsterSO.fatigue = monsterData.fatigue;
            monsterSO.fearInflicted = monsterData.fearInflicted;
            monsterSO.cooldown = monsterData.cooldown;
            monsterSO.humanScaringRange = monsterData.humanScaringRange;
            monsterSO.requiredCoins = monsterData.requiredCoins;

            EditorUtility.SetDirty(monsterSO);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    
    public List<MonsterSO> LoadMonstersFromAssets()
    {
        string folderPath = "Assets/SOs/Monster/";
        string[] assetGUIDs = AssetDatabase.FindAssets("t:MonsterSO", new[] { folderPath });

        List<MonsterSO> monsters = new List<MonsterSO>();
        foreach (string guid in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            MonsterSO monsterSO = AssetDatabase.LoadAssetAtPath<MonsterSO>(assetPath);
            if (monsterSO != null)
            {
                monsters.Add(monsterSO);
            }
        }
        return monsters;
        
    }
}