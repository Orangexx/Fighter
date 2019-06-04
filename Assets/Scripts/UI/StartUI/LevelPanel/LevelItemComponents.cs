/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace Fighter
{
	public partial class LevelItem
	{
		[SerializeField] public Text Text;
        [SerializeField] public Image Image;
        [SerializeField] public Button Button;

		public void Clear()
		{
			Text = null;
		}

		public override string ComponentName
		{
			get { return "LevelItem";}
		}
	}
}
