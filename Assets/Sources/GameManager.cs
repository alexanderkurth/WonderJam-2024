using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace game
{
    public class GameManager : Singleton<GameManager>
    {
        private const int PLAYER_PER_TEAM = 2;
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

        [SerializeField] private State m_CurrentState = State.Invalid;

        [SerializeField] private Dictionary<State, string> m_StateToScene;

        [SerializeField] private GameObject mPlayerPrefab;
        [SerializeField] private CameraManager mCameraManager;

        private Dictionary<int, PlayerInput> mPlayersInputs = new Dictionary<int, PlayerInput>();

        private void Start()
        {
            CreateControllersAndCharacters();
        }

        public void CreateControllersAndCharacters()
        {
            int playerIndex = 0;
            foreach (Gamepad gamepad in Gamepad.all)
            {
                PlayerInput playerInput = PlayerInput.Instantiate(mPlayerPrefab, playerIndex, "Gamepad",playerIndex, Gamepad.all[playerIndex].device);
                playerIndex++;
                mPlayersInputs.Add(playerIndex, playerInput);
            }

            foreach (KeyValuePair<int, PlayerInput> keyValuePair in mPlayersInputs)
            {
                int teamId = Mathf.CeilToInt((float)keyValuePair.Key / PLAYER_PER_TEAM);
                Debug.Log(teamId);
                mCameraManager.PairPlayerToTeam(teamId, keyValuePair.Value);
            }

            mCameraManager.Initialize();
        }
        
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