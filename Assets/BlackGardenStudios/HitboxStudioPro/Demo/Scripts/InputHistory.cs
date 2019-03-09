using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BlackGardenStudios.HitboxStudioPro.Demo
{
#pragma warning disable CS0649 
    public partial class InputHistory : MonoBehaviour
    {
        static readonly int INPUT_HISTORY_LENGTH = 16;
        /// <summary>
        /// How long does it take an input in history to become stale
        /// </summary>
        static readonly float INPUT_TIMEOUT_IN_SECONDS = 0.5f;

        [Serializable]
        public class InputEvent : UnityEvent<INPUTFLAG> { }

        [SerializeField]
        private InputEvent m_InputEvents;
        [SerializeField]
        private InputEvent m_OnChange;
        public InputEvent Event { get { return m_InputEvents; } }
        public InputEvent OnChange { get { return m_OnChange; } }
        [HideInInspector]
        public string InputMod = "";

        static readonly float m_Epsilon = 0.25f;

        [Serializable]
        public struct Input
        {
            public INPUTFLAG input;
            public float time;

            public Input(INPUTFLAG i, float t)
            {
                input = i;
                time = t;
            }
        }

        struct Combo
        {
            public INPUTFLAG[] inputs;
        }

        LinkedList<Input> m_RecordedInputs = new LinkedList<Input>();
        Stack<INPUTFLAG> m_InputStack = new Stack<INPUTFLAG>(16);

        float lastFramexAxis, lastFrameyAxis;
        INPUTFLAG m_Analog;
        INPUTFLAG m_Buttons;
        public INPUTFLAG CurrentState { get { return m_Analog | m_Buttons; } }

        public INPUTFLAG GetAnalog(Vector2 current)
        {
            INPUTFLAG analog = 0;
            var abs = new Vector2(Mathf.Abs(current.x), Mathf.Abs(current.y));

            if (abs.y >= m_Epsilon)
                analog |= current.y > 0 ? INPUTFLAG.UP : INPUTFLAG.DOWN;

            if (abs.x >= m_Epsilon)
                analog |= current.x > 0 ? INPUTFLAG.RIGHT : INPUTFLAG.LEFT;

            return analog;
        }

        public INPUTFLAG GetAnalogDelta(Vector2 current, Vector2 last)
        {
            INPUTFLAG analog = 0;

            if (Mathf.Abs(current.y) >= m_Epsilon)
            {
                if (Mathf.Abs(current.y) <= m_Epsilon ||
                    (Mathf.Abs(last.x) <= m_Epsilon && Mathf.Abs(current.x) >= m_Epsilon || (current.x == 0 && last.x != 0)))
                    analog |= current.y >= m_Epsilon ? INPUTFLAG.UP : (current.y <= -m_Epsilon ? INPUTFLAG.DOWN : 0);
            }

            if (Mathf.Abs(current.x) >= m_Epsilon)
            {
                if ((Mathf.Abs(current.y) >= m_Epsilon && Mathf.Abs(last.y) <= m_Epsilon || (current.y == 0 && last.y != 0)) ||
                    Mathf.Abs(last.x) <= m_Epsilon)
                    analog |= current.x >= m_Epsilon ? INPUTFLAG.RIGHT : (current.x <= -m_Epsilon ? INPUTFLAG.LEFT : 0);
            }

            return analog;
        }

        public bool ExistsInHistory(params INPUTFLAG[] args)
        {
            if (args.Length > m_RecordedInputs.Count) return false;

            var last = m_RecordedInputs.Last;
            var time = Time.time;

            m_InputStack.Clear();
            for (int i = 0; i < args.Length; i++)
                m_InputStack.Push(args[i]);

            while ((last.Value.input & m_InputStack.Pop()) != 0 && time - last.Value.time < INPUT_TIMEOUT_IN_SECONDS) 
            {
                if (m_InputStack.Count == 0)
                    return true;
                else
                    last = last.Previous;
            }

            return false;
        }

        void Update()
        {
            while (m_RecordedInputs.Count > INPUT_HISTORY_LENGTH)
                m_RecordedInputs.RemoveFirst();

            INPUTFLAG thisFrame = 0;
            float frameTime = Time.time;
            float xAnalog = UnityEngine.Input.GetAxis("Horizontal" + InputMod), yAnalog = UnityEngine.Input.GetAxis("Vertical" + InputMod);

            INPUTFLAG analog = 0;
            INPUTFLAG buttons = 0;

            if (Mathf.Abs(yAnalog) >= m_Epsilon)
            {
                if (Mathf.Abs(lastFrameyAxis) <= m_Epsilon ||
                    (Mathf.Abs(lastFramexAxis) <= m_Epsilon && Mathf.Abs(xAnalog) >= m_Epsilon || (xAnalog == 0 && lastFramexAxis != 0)))
                    thisFrame |= yAnalog >= m_Epsilon ? INPUTFLAG.UP : (yAnalog <= -m_Epsilon ? INPUTFLAG.DOWN : 0);

                analog |= yAnalog > 0 ? INPUTFLAG.UP : INPUTFLAG.DOWN;
            }

            if (Mathf.Abs(xAnalog) >= m_Epsilon)
            {
                if ((Mathf.Abs(yAnalog) >= m_Epsilon && Mathf.Abs(lastFrameyAxis) <= m_Epsilon || (yAnalog == 0 && lastFrameyAxis != 0)) ||
                    Mathf.Abs(lastFramexAxis) <= m_Epsilon)
                    thisFrame |= xAnalog >= m_Epsilon ? INPUTFLAG.RIGHT : (xAnalog <= -m_Epsilon ? INPUTFLAG.LEFT : 0);

                analog |= xAnalog > 0 ? INPUTFLAG.RIGHT : INPUTFLAG.LEFT;
            }

            lastFramexAxis = xAnalog;
            lastFrameyAxis = yAnalog;

            if (UnityEngine.Input.GetKey("joystick " + InputMod + " button 2"))
                buttons |= INPUTFLAG.LIGHT;
            if (UnityEngine.Input.GetKeyDown("joystick " + InputMod + " button 2"))
                thisFrame |= INPUTFLAG.LIGHT;

            if (UnityEngine.Input.GetKey("joystick " + InputMod + " button 0"))
                buttons |= INPUTFLAG.HEAVY;
            if (UnityEngine.Input.GetKeyDown("joystick " + InputMod + " button 0"))
                thisFrame |= INPUTFLAG.HEAVY;

            /*if (UnityEngine.Input.GetButton("Dodge"))
                buttons |= INPUTFLAG.DODGE;
            if (UnityEngine.Input.GetButtonDown("Dodge"))
                thisFrame |= INPUTFLAG.DODGE;

            if (UnityEngine.Input.GetButton("Jump"))
                buttons |= INPUTFLAG.JUMP;
            if (UnityEngine.Input.GetButtonDown("Jump"))
                thisFrame |= INPUTFLAG.JUMP;

            if (UnityEngine.Input.GetButton("Guard"))
                buttons |= INPUTFLAG.GUARD;
            if (UnityEngine.Input.GetButtonDown("Guard"))
                thisFrame |= INPUTFLAG.GUARD;

            if (UnityEngine.Input.GetButton("Specials"))
                buttons |= INPUTFLAG.SPECIAL;
            if (UnityEngine.Input.GetButtonDown("Specials"))
                thisFrame |= INPUTFLAG.SPECIAL;*/

            if (thisFrame != 0)
            {
                m_RecordedInputs.AddLast(new Input(thisFrame, frameTime));
                m_InputEvents.Invoke(thisFrame);
            }

            if ((buttons | analog) != CurrentState)
            {
                m_OnChange.Invoke((buttons | analog));
            }

            m_Buttons = buttons;
            m_Analog = analog;
        }
    }
}