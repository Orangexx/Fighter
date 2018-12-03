using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;

public class InputController : MonoSingleton<InputController>
{
    KeyCode UP;
    KeyCode DOWN;
    KeyCode LEFT;
    KeyCode RIGHT;
    KeyCode JUMP;
    KeyCode ATTACK;
    KeyCode SKILL2;
    KeyCode SKILL3;
    KeyCode SKILL1;

    private Sqlite setSqlite;

    public Bool Bup { private set; get; }
    public Bool Bdown { private set; get; }
    public Bool Bleft { private set; get; }
    public Bool Bright { private set; get; }
    public Bool BupDown { private set; get; }
    public Bool BdownDown { private set; get; }
    public Bool BleftDown { private set; get; }
    public Bool BrightDown { private set; get; }
    public Bool Battack { private set; get; }
    public Bool Bjump { private set; get; }
    public Bool BSkill2 { private set; get; }
    public Bool BSkill3 { private set; get; }
    public Bool isMoving { private set; get; }
    public Bool BSkill1 { private set; get; }
    

    public bool up { private set; get; }
    public bool down { private set; get; }
    public bool left { private set; get; }
    public bool right { private set; get; }
    public bool upDown { private set; get; }
    public bool downDown { private set; get; }
    public bool leftDown { private set; get; }
    public bool rightDown { private set; get; }
    public bool attack { private set; get; }
    public bool jump { private set; get; }
    public bool ShangTi { private set; get; }
    public bool XiaTI { private set; get; }

    private List<InputSetting> mInpuSetting;

    private void Awake()
    {
        setSqlite = new Sqlite(Application.dataPath + "/SQLites/InputSetting.db");
        mInpuSetting = setSqlite.SelectTable<InputSetting>();
        UP = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[0].Key);
        DOWN = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[1].Key);
        LEFT = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[2].Key);
        RIGHT = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[3].Key);
        JUMP = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[4].Key);
        ATTACK = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[5].Key);
        SKILL1 = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[6].Key);
        SKILL2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[7].Key);
        SKILL3 = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[8].Key);
        Debug.Log(mInpuSetting[7].Key);
        setSqlite.Close();


        Bup = new Bool(false);
        Bdown = new Bool(false);
        Bleft = new Bool(false);
        Bright = new Bool(false);
        BdownDown = new Bool(false);
        BupDown = new Bool(false);
        BleftDown = new Bool(false);
        BrightDown = new Bool(false);
        Battack = new Bool(false);
        Bjump = new Bool(false);
        BSkill2 = new Bool(false);
        BSkill3 = new Bool(false);
        BSkill1 = new Bool(false);
        isMoving = new Bool(false);

    }

    void Start()
    {

    }

    void Update()
    {
        up = Input.GetKey(UP);
        down = Input.GetKey(DOWN);
        right = Input.GetKey(RIGHT);
        left = Input.GetKey(LEFT);
        upDown = Input.GetKeyDown(UP);
        downDown = Input.GetKeyDown(DOWN);
        rightDown = Input.GetKeyDown(RIGHT);
        leftDown = Input.GetKeyDown(LEFT);
        attack = Input.GetKeyDown(ATTACK);
        jump = Input.GetKeyDown(JUMP);
        //ShangTi = Input.GetKey(SHANGTI);
        //XiaTI = Input.GetKey(XIATI);

        Bup.BOOL = Input.GetKey(UP);
        Bdown.BOOL = Input.GetKey(DOWN);
        Bright.BOOL = Input.GetKey(RIGHT);
        Bleft.BOOL = Input.GetKey(LEFT);
        BupDown.BOOL = Input.GetKeyDown(UP);
        BdownDown.BOOL = Input.GetKeyDown(DOWN);
        BrightDown.BOOL = Input.GetKeyDown(RIGHT);
        BleftDown.BOOL = Input.GetKeyDown(LEFT);
        Battack.BOOL = Input.GetKeyDown(ATTACK);
        Bjump.BOOL = Input.GetKeyDown(JUMP);
        BSkill1.BOOL = Input.GetKey(SKILL1);
        BSkill2.BOOL = Input.GetKey(SKILL2);
        BSkill3.BOOL = Input.GetKey(SKILL3);
        isMoving.BOOL = (up || left || down || right);

    }

    public bool IsMoving()
    {
        return (up || left || down || right);
    }

    public Bool BIsMoving()
    {
        return new Bool(up || left || down || right);
    }
}

public class Bool
{
    public bool BOOL = false;
    public Bool(bool val)
    {
        BOOL = val;
    }
}
