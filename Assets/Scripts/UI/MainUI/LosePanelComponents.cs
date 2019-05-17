/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class LosePanel
	{
		public const string NAME = "LosePanel";

		[SerializeField] public Text Text_Lose;
		[SerializeField] public Button Button_MainMenu;

		protected override void ClearUIComponents()
		{
			Text_Lose = null;
			Button_MainMenu = null;
		}

		private LosePanelData mPrivateData = null;

		public LosePanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new LosePanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
