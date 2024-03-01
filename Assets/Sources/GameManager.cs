using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace game
{
    public class GameManager : Singleton<GameManager>
    {
        //This enum matches the scene index in the build settings. 
        //Modifying the build setting will reqiure to update this enum
        public enum State : UInt32
        {
            MainMenu = 0,
            Gameplay = 1,
            InGameMenu = 2,

            Count = 3,
            Invalid = 4,
        };

        [SerializeField]
        private State m_CurrentState = State.Invalid;

        [SerializeField]
        private Dictionary<State, string> m_StateToScene;

        public void ChangeState(State state)
        {
            m_CurrentState = state;
            switch (state)
            {
                case State.MainMenu:
                    {
                        m_CurrentState = State.MainMenu;
                        SceneManager.LoadScene((Int32)m_CurrentState, LoadSceneMode.Single);

                        break;
                    }

                case State.Gameplay:
                    {
                        m_CurrentState = State.Gameplay;
                        SceneManager.LoadScene((Int32)m_CurrentState, LoadSceneMode.Single);

                        break;
                    }

                case State.InGameMenu:
                    {
                        //handle in game menu when time has come
                        break;
                    }
                default:
                    {
                        Debug.LogError("unknowm entry :" + state);
                        break;
                    }
            }
        }

        public State GetCurrentState()
        {
            return m_CurrentState;
        }

#if DEBUG
        void OnGUI()
        {
            GUI.Box(new Rect(10, 10, 100, 90), m_CurrentState.ToString());

            if (GUI.Button(new Rect(20, 40, 80, 20), "MainMenu"))
            {
                ChangeState(State.MainMenu);
            }

            if (GUI.Button(new Rect(20, 70, 80, 20), "Gameplay"))
            {
                ChangeState(State.Gameplay);
            }
        }
    }
#endif // DEBUG
    }