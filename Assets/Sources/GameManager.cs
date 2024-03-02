using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
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
                PlayerInput playerInput = PlayerInput.Instantiate(mPlayerPrefab, playerIndex, "Gamepad",playerIndex, gamepad);
                
                playerIndex++;
                mPlayersInputs.Add(playerIndex, playerInput);
            }

            foreach (KeyValuePair<int, PlayerInput> keyValuePair in mPlayersInputs)
            {
                int teamId = Mathf.CeilToInt((float)keyValuePair.Key / PLAYER_PER_TEAM);
                mCameraManager.PairPlayerToTeam(teamId, keyValuePair.Value);
            }

            UnityBadSystemOverride();
        }

        private void UnityBadSystemOverride()
        {
            bool found = false;
            KeyValuePair<int, PlayerInput> selectedKey = default;
            foreach (KeyValuePair<int, PlayerInput> keyValuePair in mPlayersInputs)
            {
                if (keyValuePair.Value.devices.Count > 1)
                {
                    found = true;
                    selectedKey = keyValuePair;
                    break;
                }
            }

            if (!found)
            {
                return;
            }
            
            Gamepad gamepad = Gamepad.all[selectedKey.Key - 1];
            PlayerInput playerInput = PlayerInput.Instantiate(mPlayerPrefab, selectedKey.Key, "Gamepad",selectedKey.Key, gamepad);

            int teamId = Mathf.CeilToInt((float)selectedKey.Key / PLAYER_PER_TEAM);
            mCameraManager.UnpairPlayerToTeam(teamId, selectedKey.Value);
            Destroy(mPlayersInputs[selectedKey.Key].gameObject);
            mPlayersInputs[selectedKey.Key] = playerInput;
                    
            mCameraManager.PairPlayerToTeam(teamId, playerInput);
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

                    m_LoadingOperation = SceneManager.LoadSceneAsync((Int32)m_CurrentState, LoadSceneMode.Single);

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