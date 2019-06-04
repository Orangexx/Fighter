/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

namespace Fighter
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class LevelPanel
	{
		public const string NAME = "LevelPanel";

		[SerializeField] public RectTransform Content;
		[SerializeField] public LevelItem BtnLevel;
        [SerializeField] public Button BtnReturn;

		protected override void ClearUIComponents()
		{
			Content = null;
			BtnLevel = null;
		}

		private LevelPanelData mPrivateData = null;

		public LevelPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new LevelPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
