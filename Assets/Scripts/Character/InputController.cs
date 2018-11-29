using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    KeyCode UP;
    KeyCode DOWN;
    KeyCode LEFT;
    KeyCode RIGHT;
    KeyCode JUMP;
    KeyCode ATTACK;
    KeyCode SHANGTI;
    KeyCode XIATI;

    private Sqlite setSqlite;
    public static InputController Instance;

    public bool up;
    public bool down;
    public bool left;
    public bool right;
    public bool upDown;
    public bool downDown;
    public bool leftDown;
    public bool rightDown;
    public bool attack;
    public bool jump;
    public bool ShangTi;
    public bool XiaTI;

    public int SPEED;
    private List<InputSetting> mInpuSetting;

    private static InputController GetInstance()
    {
        if (Instance == null)
            Instance = new InputController();
        return Instance;
    }

    void Start()
    {
        setSqlite = new Sqlite(Application.dataPath + "/SQLites/InputSetting.db");
        SPEED = 5;
        mInpuSetting = setSqlite.SelectTable<InputSetting>();
        UP = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[0].Key);
        DOWN = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[1].Key);
        LEFT = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[2].Key);
        RIGHT = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[3].Key);
        JUMP = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[4].Key);
        ATTACK = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[5].Key);
        SHANGTI = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[6].Key);
        XIATI = (KeyCode)System.Enum.Parse(typeof(KeyCode), mInpuSetting[7].Key);
        setSqlite.Close();
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
        attack = Input.GetKeyDown(JUMP);
        jump = Input.GetKeyDown(ATTACK);
        ShangTi = Input.GetKey(XIATI);
        XiaTI = Input.GetKey(SHANGTI);
    }

    public bool IsMoving()
    {
        return (up || left || down || right);
    }
}
