/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class WinPanel
	{
		public const string NAME = "WinPanel";

		[SerializeField] public Text Text_Win;
		[SerializeField] public Button Button_MainMenu;

		protected override void ClearUIComponents()
		{
			Text_Win = null;
			Button_MainMenu = null;
		}

		private WinPanelData mPrivateData = null;

		public WinPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new WinPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
