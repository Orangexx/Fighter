using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QFramework;

[QFramework.QMonoSingletonPath("[Manager]/MapManager")]
public class LevelManager: MonoSingleton<LevelManager>
{
    public int CurLevel { private set; get; }
    private List<LevelConfig> mCurConfigs;
    public void Init(int level)
    {
        CurLevel = level;
        mCurConfigs = ConfigManager.Instance.GetLevelConfigs(level);
    }

    public void MoveToNextRoom()
    {

    }

}
