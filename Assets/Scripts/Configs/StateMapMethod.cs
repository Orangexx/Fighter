
using System.Collections.Generic;

public partial class StateMap : IConfig
{
    private List<string> mNextStates = new List<string>();

    public List<string> GetNextStates()
    {
        if (mNextStates.Count > 0)
            return mNextStates;

        string[] nextStates = NextStateNames.Split(':');
        for (int i = 0; i < nextStates.Length; i++)
            mNextStates.Add(nextStates[i]);
        return mNextStates;
    }

}
