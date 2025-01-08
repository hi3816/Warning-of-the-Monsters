using GoogleSheet.Type;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hamster.ZG.Type
{
    [Type(typeof(List<float>), new string[] { "list<single>", "List<single>", "list<float>", "List<float>" })]
    public class SingleListType : IType
    {
        public object DefaultValue => new List<float>();

        public object Read(string value)
        {
            try
            {
                var list = new List<float>();
                if (value == "[]") return list;

                var datas = ReadUtil.GetBracketValueToArray(value);
                if (datas != null)
                {
                    foreach (var data in datas)
                    {
                        list.Add(float.Parse(data));
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Debug.LogError($"SingleListType.Read Error: {ex.Message}");
                return new List<float>();
            }
        }

        public string Write(object value)
        {
            var list = value as List<float>;
            return WriteUtil.SetValueToBracketArray(list);
        }
    }
}