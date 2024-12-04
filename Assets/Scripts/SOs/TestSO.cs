using UnityEngine;

[CreateAssetMenu(fileName = "TestSO", menuName = "TestSO")]

public class TestSO : ScriptableObject
{
    public int id; // test 아이디
    public string testName; // test이름
    public string testSpriteName; // test스프라이트 이름
}