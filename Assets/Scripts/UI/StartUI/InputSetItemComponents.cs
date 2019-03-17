/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/

namespace QFramework.Fighter
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class InputSetItem
	{
		public const string NAME = "InputSetItem";

		[SerializeField] private Text mKeyName;
		[SerializeField] private Button mButton;
		[SerializeField] private Text mText;

		protected override void ClearUIComponents()
		{
			mKeyName = null;
			mButton = null;
			mText = null;
		}

		private InputSetItemData mPrivateData = null;

		public InputSetItemData mData
		{
			get { return mPrivateData ?? (mPrivateData = new InputSetItemData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
