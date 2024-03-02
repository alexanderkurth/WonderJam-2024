using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    InProgress = 1,
    InShop = 2,
    Ending = 3
}

public class GameMode : Singleton<GameMode>
{
    public enum GameOverCondition
    {
        OutOfScreen,
        Madness, 
        NotEnoughMoney
    }

    [SerializeField] 
    private Canvas _canvas;
    [SerializeField] 
    private GameObject _gameOverScreen;
    [SerializeField] 
    private GameObject _winScreen;
    [SerializeField] 
    private int _ennemyCount = 10;
    [SerializeField]
    private float _TimeBeforeStart = 3.0f;

    [SerializeField]
    private GameState _mGameState = GameState.InProgress;
    public GameState GameState { get => _mGameState; }
	
    [SerializeField]
    private GameObject _chevalier;

    public int dayCount { get => GlobalDataHolder.Instance.CurrentDay; }
    public bool isGameOver = false;

    [SerializeField]
    private Vector2 _mTimeBetweenPlayerMessages = new Vector2(10f, 20f);
    private float _mTimeSinceLastMessageDisplayed = 0f;

    private void Start()
    {
        Time.timeScale = 1f;
        DailyTax.Instance.DisplayTax();

        StartChevalierAndEnemies();
        SpawnEnnemies();
        Inventory.Instance.ChangeCurrencyValue(0);
        _mGameState = GameState.InProgress;
        HUDCanvas.Instance.SetDayText(dayCount);

    }

    private void Update()
    {
        if (_mGameState == GameState.InProgress)
        {
            if (Time.timeSinceLevelLoad - _mTimeSinceLastMessageDisplayed >= Random.Range(_mTimeBetweenPlayerMessages.x, _mTimeBetweenPlayerMessages.y))
            {
                DisplayMessage(MessageEnum.RandomSentenceInGame);
            }
        }
    }

    private void StartChevalierAndEnemies()
    {
        _chevalier.GetComponent<ChevalierMove>().SetStartTimer(_TimeBeforeStart);
        _chevalier.GetComponentInChildren<EnemySpawner>().SetStartTimer(_TimeBeforeStart);

        DisplayMessage(MessageEnum.DelayStartGame, 0.5f, true);
    }

    public void OnEnemyKilled()
    {
        if (Random.Range(0, 2) > 0)
        {
            DisplayMessage(MessageEnum.FinishingAnEnemy, 0f, true);
        }
    }

    public void DisplayMessage(MessageEnum messageEnum, float delayStartTime = 0f, bool forceMessage = false)
    {
        if (Time.timeSinceLevelLoad - _mTimeSinceLastMessageDisplayed >= Random.Range(_mTimeBetweenPlayerMessages.x, _mTimeBetweenPlayerMessages.y) || forceMessage)
        {
            if(HUDCanvas.Instance.DisplayMessage(messageEnum, delayStartTime))
            {
                _mTimeSinceLastMessageDisplayed = Time.timeSinceLevelLoad;
            }
        }
    }

    public void DayEnd()
    {
        DailyTax.Instance.DeductTax();
        GlobalDataHolder.Instance.IncreaseDayCount();

        if (_mGameState != GameState.Ending)
        {
            Shop.Instance.InitializeShopOfTheDay();

            _mGameState = GameState.InShop;
            Camera.main.enabled = false;
            ShopSceneManager.Instance.StartShoppingLoop();
        }
    }

    public void StartNewDay()
    {
        IncreaseDifficulty(dayCount);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    public void WinGame()
    {
        Time.timeScale = 0f;
        
        HUDCanvas.Instance.SendVictoryMessage();
        Transform lastChild = _canvas.transform.GetChild(_canvas.transform.childCount - 1);
        
        GameObject winScreen = Instantiate(_winScreen, _canvas.transform);
        Button firstButton = winScreen.GetComponentInChildren<Button>();
        EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        
        lastChild.SetAsLastSibling();
        _mGameState = GameState.Ending;

        // Destroy Inventory
        Destroy(Inventory.Instance.gameObject);
        Destroy(GlobalDataHolder.Instance.gameObject);
    }
    
    public void GameOver(GameOverCondition gameOverCondition)
    {
        Time.timeScale = 0f;

        HUDCanvas.Instance.SendGameOverMessage(gameOverCondition);
        Transform lastChild = _canvas.transform.GetChild(_canvas.transform.childCount - 1);
        GameObject gameOver = Instantiate(_gameOverScreen, _canvas.transform);
        gameOver.GetComponent<GameOver>().UpdateGameOverText(gameOverCondition);
        
        // Select "Go to main menu" button
        Button firstButton = gameOver.GetComponentInChildren<Button>();
        EventSystem.current.SetSelectedGameObject(firstButton.gameObject);

        lastChild.SetAsLastSibling();
        isGameOver = true;
        _mGameState = GameState.Ending;
        
        // Stop All Audio
        AudioListener[] allAudioListeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in allAudioListeners)
        {
            listener.enabled = false;
        }
        
        // Destroy Inventory
        Destroy(Inventory.Instance.gameObject);
        Destroy(GlobalDataHolder.Instance.gameObject);
    }

    public void SpawnEnnemies()
    {
        EnemySpawner.Instance.StartSpawn(_ennemyCount);
    }

    public void IncreaseDifficulty(int level)
    {
        ChevalierMove chevalier = _chevalier.GetComponent<ChevalierMove>();
        if(chevalier.m_Speed + level >= 10.0f)
        {
            chevalier.m_Speed = 10.0f;
        }
        else
        {
            chevalier.m_Speed += level;
        }

        // TODO : Impact on the Madness
    }
}
