using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace game
{
    public class GameManager : AbstractSingleton<GameManager>
    {
        //This enum matches the scene index in the build settings. Modifying the build setting will reqiure to updat this enum
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
                        SceneManager.LoadScene(m_CurrentState.ToString());

                        break;
                    }

                case State.Gameplay:
                    {
                        m_CurrentState = State.Gameplay;
                        SceneManager.LoadScene(m_CurrentState.ToString());

                        break;
                    }

                case State.InGameMenu:
                    {
                        //load scene in game menu
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
    }
}