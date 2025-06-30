using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Text_Scriptable")]
public class TextData : ScriptableObject
{
    [SerializeField] private string message;
    public string Message => message;
    public List<Story> stories = new List<Story>();

    [System.Serializable]
    public class Story
    {
        public string introTextData;
    }
}
