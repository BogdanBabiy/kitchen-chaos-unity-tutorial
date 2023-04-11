using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KitchenGameManager : MonoBehaviour
{
    public event EventHandler OnGameStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnpaused;
    
    public static KitchenGameManager Instance { get; private set; }
    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    [SerializeField] private float gamePlayingTimerMax = 10f;
    
    private GameState state;
    private float countdownToStartTimer = 3f;
    private float gamePlayingTimer;
    private bool isGamePaused;
   

    
    private void Awake()
    {
        Instance = this;
        state = GameState.WaitingToStart;
        gamePlayingTimer = gamePlayingTimerMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameInput.Instance.onPauseAction += GameInput_onPauseAction;
        GameInput.Instance.onInteractAction += GameInput_onInteractAction;
    }

    private void GameInput_onInteractAction(object sender, EventArgs e)
    {
        if (state == GameState.WaitingToStart)
        {
            state = GameState.CountdownToStart;
            OnGameStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_onPauseAction(object sender, EventArgs e)
    {
        TogglePauseGame();
    }
    
    public void TogglePauseGame()
    {
        isGamePaused = !isGamePaused;
        
        Time.timeScale = isGamePaused ? 0f: 1f;
        
        if(isGamePaused) OnGamePaused?.Invoke(this,EventArgs.Empty);
        else OnGameUnpaused?.Invoke(this,EventArgs.Empty);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case GameState.WaitingToStart:
                break;
            case GameState.CountdownToStart:
                countdownToStartTimer -= Time.deltaTime;
                if (countdownToStartTimer < 0f)
                {
                    state = GameState.GamePlaying;
                    gamePlayingTimer = gamePlayingTimerMax;
                    OnGameStateChanged?.Invoke(this,EventArgs.Empty);
                }
                break; 
            case GameState.GamePlaying:
                gamePlayingTimer -= Time.deltaTime;
                if (gamePlayingTimer < 0f) 
                {
                    state = GameState.GameOver;
                    OnGameStateChanged?.Invoke(this,EventArgs.Empty);
                }
                break; 
            case GameState.GameOver:
                break;
        }
    }

    public bool IsGameWaitingToStart()
    {
        return state == GameState.WaitingToStart;
    }
    
    public bool IsGamePlaying()
    {
        return state == GameState.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state == GameState.CountdownToStart;
    }
    
    public bool IsGameOver()
    {
        return state == GameState.GameOver;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer;
    }

    public float GetGamePlayingTimerNormalised()
    {
        return gamePlayingTimer / gamePlayingTimerMax;
    }
}
