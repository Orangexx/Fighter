/****************************************************************************
 * 2019.5 DESKTOP-ERI3UES
 ****************************************************************************/

namespace Fighter
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class AudioSettingPanel
	{
		public const string NAME = "AudioSettingPanel";

		[SerializeField] public Slider MusicSlider;
		[SerializeField] public Slider AudioSlider;
		[SerializeField] public Button ButtonReturn;

		protected override void ClearUIComponents()
		{
			MusicSlider = null;
			AudioSlider = null;
			ButtonReturn = null;
		}

		private AudioSettingPanelData mPrivateData = null;

		public AudioSettingPanelData mData
		{
			get { return mPrivateData ?? (mPrivateData = new AudioSettingPanelData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
