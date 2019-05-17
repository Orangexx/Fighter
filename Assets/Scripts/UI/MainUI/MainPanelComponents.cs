/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class MainPanel
	{
		public const string NAME = "MainPanel";

		[SerializeField] public Slider Slider_Hp;
		[SerializeField] public Slider Slider_Exp;
		[SerializeField] public Text Text_Level;
		[SerializeField] public Button Setting;

		protected override void ClearUIComponents()
		{
			Slider_Hp = null;
			Slider_Exp = null;
			Text_Level = null;
			Setting = null;
		}

		private MainPanelData mPrivateData = null;

		public MainPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new MainPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
