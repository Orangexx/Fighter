/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace Fighter
{
	public partial class LevelItem : UIElement
	{
		private void Awake()
		{
		}

        public void SetItem(Sprite sprite,int level)
        {
            Image.sprite = sprite;
            Text.text = string.Format("第{0}关",level.ToString());
            Button.AddCallback(() => { GlobalManager.Instance.InitMainScene(level); });
            gameObject.SetActive(true);
        }

		protected override void OnBeforeDestroy()
		{
		}
	}
}