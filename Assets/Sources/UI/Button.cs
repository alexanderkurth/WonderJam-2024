using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace UI
{
    public class Button : MonoBehaviour
    {
        [SerializeField]
        private game.GameManager.State m_CurrentState = game.GameManager.State.Invalid;

        public void OnClick()
        {
            game.GameManager.Instance.ChangeState(m_CurrentState);
        }
    }
}