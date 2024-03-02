using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace game
{
    public enum TeamID : UInt32
    {
        Team1 = 0,
        Team2 = 1,

        Count = 2,
        Invalid = 3,
    };

    public enum PlayerID : UInt32
    {
        Player1 = 0,
        Player2 = 1,
        Player3 = 2,
        Player4 = 3,

        Count = 4,
        Invalid = 5,
    };

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

        [SerializeField]
        private AnimalDatas m_AnimalDatas;

        [SerializeField]
        private GameObject m_LoadingScreen;

        UnityEngine.AsyncOperation m_LoadingOperation;

        [Header("Sounds")]
        [SerializeField]
        private AK.Wwise.Event PlayButtonEvent = null;
        [SerializeField]
        private AK.Wwise.Event GenericButtonEvent = null;
        [SerializeField]
        private AK.Wwise.Event QuitButtonEvent = null;

        public void ChangeState(State state)
        {
            m_CurrentState = state;
            switch (state)
            {
                case State.MainMenu:
                    {
                        m_CurrentState = State.MainMenu;

                        m_LoadingOperation = SceneManager.LoadSceneAsync((Int32)m_CurrentState, LoadSceneMode.Single);

                        break;
                    }

                case State.Gameplay:
                    {
                        m_CurrentState = State.Gameplay;
                        m_LoadingOperation = SceneManager.LoadSceneAsync((Int32)m_CurrentState, LoadSceneMode.Single);

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
        
        public AnimalDatas GetAnimalDatas()
        {
            return m_AnimalDatas;
        }

        public void Update()
        {
            if(m_LoadingOperation != null)
            {
                float value = Mathf.Clamp01(m_LoadingOperation.progress / 0.9f);
                Debug.Log(value);
                if(m_LoadingOperation.isDone)
                {
                    m_LoadingOperation = null;
                }
            }
        }

        public void ChangeToGameplay()
        {
            ChangeState(State.Gameplay);
            PlayButtonEvent.Post(gameObject);
        }

        public void ChangeToMainMenu()
        {
            GenericButtonEvent.Post(gameObject);
            ChangeState(State.MainMenu);
        }

        public void QuitGame()
        {
            QuitButtonEvent.Post(gameObject);
            Application.Quit();
        }

        public void PlayGenericSound()
        {
            GenericButtonEvent.Post(gameObject);
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