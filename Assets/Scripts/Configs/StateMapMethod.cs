
using System.Collections.Generic;

public partial class StateMap : IConfig
{

    private List<StateTrigger> mTriggers = new List<StateTrigger>();
    private List<StateMove> mStateMoves = new List<StateMove>();

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


        if (MoveSpeedTriggers == "null")
            return;
        string[] moveInfos = MoveSpeedTriggers.Split('|');
        StateMove moveHo = new StateMove(moveInfos[0], true);
        mStateMoves.Add(moveHo);
        if(moveInfos.Length==2)
        {
            StateMove moveVe = new StateMove(moveInfos[1], false);
            mStateMoves.Add(moveVe);
        }
    }

    public List<StateTrigger> GetStateTriggers()
    {
        _Init();
        return mTriggers;
    }

    public List<StateMove> GetStateMoves()
    {
        _Init();
        if (mStateMoves.Count == 0)
            return null;
        return mStateMoves;
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

public class StateMove
{
    public string MoveType { private set; get; }
    public float MoveSpeed { private set; get; }
    public bool IsMoveHorizontal { private set; get; }

    public StateMove(string moveString,bool isHorz)
    {
        string[] moveInfo = moveString.Split('.');

        if (moveInfo.Length != 2)
            return;

        MoveType = moveInfo[1];
        float a = -1;
        float.TryParse(moveInfo[0],out a);
        MoveSpeed = a;
        IsMoveHorizontal = isHorz;
    }
}
