/****************************************************************************
 * 2019.3 DESKTOP-CFTQOJO
 ****************************************************************************/

namespace Fighter
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class Panel_Start
	{
		public const string NAME = "Panel_Start";

		[SerializeField] public Text Text_Title;
		[SerializeField] public Image Panel_StartBtns;
		[SerializeField] public Button BtnStart;
		[SerializeField] public Button BtnLoad;
		[SerializeField] public Button BtnIntpuSet;
        [SerializeField] public Button BtnAudioSet;

		protected override void ClearUIComponents()
		{
			Text_Title = null;
			Panel_StartBtns = null;
			BtnStart = null;
			BtnLoad = null;
			BtnIntpuSet = null;
            BtnAudioSet = null;
		}

		private Panel_StartData mPrivateData = null;

		public Panel_StartData mData
		{
			get { return mPrivateData ?? (mPrivateData = new Panel_StartData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
