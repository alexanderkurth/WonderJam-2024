using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplinationScene : MonoBehaviour
{
    [SerializeField]
    public float m_ReadingTime = 5.0f;
    [SerializeField]
    public GameObject m_TextPanel;
    [SerializeField]
    public GameObject m_MainCamera;
    [SerializeField]
    public float m_ZoomDuration = 5.0f;
    [SerializeField]
    private float m_LearpMin = 300.0f;
    [SerializeField]
    private float m_LearpMax = 200.0f;
    private float m_TimeElapsed;
    private bool m_ZoomActif = false;

    // Start is called before the first frame update
    void Start()
    {
        m_ZoomActif = true;
    }

    private void Update()
    {
        if(m_ZoomActif)
        {
            float lerpValue = 0.0f;
            if (m_TimeElapsed < m_ZoomDuration)
            {
                lerpValue = Mathf.Lerp(m_LearpMin, m_LearpMax, m_TimeElapsed / m_ZoomDuration);
                m_TimeElapsed += Time.deltaTime;
                m_MainCamera.GetComponent<Camera>().orthographicSize = lerpValue;
            }
            else
            {
                m_ZoomActif = false;
                StartCoroutine(DisplayTextPanel());
            }
        }
    }

    IEnumerator DisplayTextPanel()
    {
        m_TextPanel.SetActive(true);
        
        yield return new WaitForSeconds(m_ReadingTime);
        
        LoadSceneByName("Menu");
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
