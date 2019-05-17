/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class SettingPanel
	{
		public const string NAME = "SettingPanel";

		[SerializeField] public Button Button_Resume;
		[SerializeField] public Button Button_MainMenu;
		[SerializeField] public Button Button_InputSet;

		protected override void ClearUIComponents()
		{
			Button_Resume = null;
			Button_MainMenu = null;
			Button_InputSet = null;
		}

		private SettingPanelData mPrivateData = null;

		public SettingPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new SettingPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
