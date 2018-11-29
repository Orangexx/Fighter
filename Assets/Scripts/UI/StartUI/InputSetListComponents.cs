/****************************************************************************
 * 2018.11 DESKTOP-CFTQOJO
 ****************************************************************************/

namespace QFramework.Example
{
	using UnityEngine;
	using UnityEngine.UI;

	public partial class InputSetList
	{
		public const string NAME = "InputSetList";

        [SerializeField] private RectTransform mItemRoot;
        [SerializeField] private InputSetItem mSetItemTem;
        [SerializeField] private Button mSaveBtn;
        [SerializeField] private Button mCancelBtn;

		protected override void ClearUIComponents()
		{
            mItemRoot = null;
            mSetItemTem = null;
            mSaveBtn = null;
            mCancelBtn = null;
		}

		private InputSetListData mPrivateData = null;

		public InputSetListData mData
		{
			get { return mPrivateData ?? (mPrivateData = new InputSetListData()); }
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
