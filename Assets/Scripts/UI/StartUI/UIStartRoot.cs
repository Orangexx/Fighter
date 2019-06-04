using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

namespace Fighter
{
    public class UIStartRoot : MonoBehaviour
    {

        private void Awake()
        {
            ResMgr.Init();
            GlobalManager.Instance.DeleteAllManager();
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.2f);

            UIMgr.OpenPanel<Panel_Start>();
        }
    }
}

