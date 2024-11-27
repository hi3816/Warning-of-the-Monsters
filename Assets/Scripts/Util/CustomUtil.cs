using UnityEngine;

public static class CustomUtil
{
    // 경로에 있는 오브젝트를 가져오는 메소드
    public static T ResourceLoad<T>(string path) where T : Object
    {
        T instance = Resources.Load<T>(path);
        if (instance == null)
        {
            Debug.Log($"{typeof(T).Name} not found in Resources folder at {path}.");
        }

        return instance;
    }

    // 경로에 존재하는 모든 오브젝트를 가져오는 메소드
    public static T[] ResourceLoadAll<T>(string path) where T : Object
    {
        T[] instance = Resources.LoadAll<T>(path);
        if (instance == null)
        {
            Debug.Log($"{typeof(T).Name} not found in Resources folder at {path}.");
        }

        return instance;
    }
}