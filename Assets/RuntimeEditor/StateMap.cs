using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RuntimeEditor
{
    public enum TriggerAction
    {
        isGrounded = 0,
        LeftOrRight = 1,
        Attack = 2,
        Skill1 = 3,
        Skill2 = 4,
        Skill3 = 5,
        Jump = 6,
        DunFu = 7
    }

    [CreateAssetMenu(menuName = "Create StateMap")]
    public class StateMap : SerializedScriptableObject
    {
        public string StateName;
        public Dictionary<string, List<StateTriggers>> Dic_name_triggers = new Dictionary<string, List<StateTriggers>>();
    }

    public class StateTriggers
    {
        public TriggerAction trigger;
        public string[] param;

        public string ParamToString()
        {
            string str = "";
            if (param != null)
                for (int i = 0; i < param.Length; i++)
                {
                    str += param[i];
                    if (i != param.Length - 1)
                    {
                        str += ",";
                    }
                }
            return str;
        }
    }
}
