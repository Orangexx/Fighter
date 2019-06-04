using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using UnityEditor;
using QFramework.Fighter;

namespace Fighter
{
    public class GameRoot :MonoBehaviour
    {

        ResLoader mResLoader = ResLoader.Allocate();

        private void Awake()
        {
            ResMgr.Init();
            GlobalManager.Instance.Init();
            UIMgr.OpenPanel<Panel_Start>();
        }
    }
}

