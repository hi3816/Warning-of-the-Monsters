using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Human_Data;

public class HumanDataLoader : MonoBehaviour
{
    [MenuItem("Tools/Update HumanSO from JSON")]
    public static void UpdateHumanSOFromJSON()
    {
        // JSON 파일 리스트로 로드
        List<Human_Data.HumanData> humanDataList = Human_Data.HumanData.GetList();
        
        string savePath = "Assets/Resources/SO/Human/";  // SO 저장할 경로 설정

        foreach (HumanData humanData in humanDataList)
        {
            // 경로에 SO 있는지 확인하고 없으면 새로 생성
            string soPath = $"{savePath}HumanSO_{humanData.id}.asset";
            HumanSO humanSO = AssetDatabase.LoadAssetAtPath<HumanSO>(soPath);
            if (humanSO == null)
            {
                humanSO = ScriptableObject.CreateInstance<HumanSO>();
                AssetDatabase.CreateAsset(humanSO, soPath);
            }

            // 새로운 값으로 갱신
            humanSO.id = humanData.id;
            humanSO.maxFear = humanData.maxFear;
            humanSO.minFatigueInflicted = humanData.minFatigueInflicted;
            humanSO.maxFatigueInflicted = humanData.maxFatigueInflicted;
            humanSO.cooldown = humanData.cooldown;
            humanSO.speed = humanData.speed;
            humanSO.lifeInflicted = humanData.lifeInflicted;
            humanSO.coin = humanData.coin;

            EditorUtility.SetDirty(humanSO);    // 에디터에 반영
        }

        // 에셋에 변경 내용 적용
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.LogAssertion("HumanSO assets updated successfully!");
    }
}