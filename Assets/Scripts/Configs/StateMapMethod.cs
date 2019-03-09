
using System.Collections.Generic;

public partial class StateMap : IConfig
{

    private List<StateTrigger> mTriggers = new List<StateTrigger>();

    private void _Init()
    {
        if (mTriggers.Count > 0)
            return;

        string[] toNextStates = StateTriggers.Split('|');
        for (int i = 0; i < toNextStates.Length; i++)
        {
            StateTrigger trigger = new StateTrigger(toNextStates[i]);
            mTriggers.Add(trigger);
        }
    }

    public List<StateTrigger> GetStateTriggers()
    {
        _Init();
        return mTriggers;
    }
}

public class StateTrigger
{
    public string NextStateName { private set; get; }
    public string TriggerKey { private set; get; }
    public string TriggerTime { private set; get; }

    public StateTrigger(string triggerString)
    {
        string[] toNextState = triggerString.Split('.');
        if (toNextState.Length != 3)
            return;

        NextStateName = toNextState[0];
        TriggerKey = toNextState[1];
        TriggerTime = toNextState[2];
    }
}
