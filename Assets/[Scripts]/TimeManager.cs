using UnityEngine;

public class TimeManager
{
    private static float _timeScale = Time.timeScale;
    public static float timeScale { get { return _timeScale; } set { SetTimeScale(value); } }
    private static bool _paused;
    public static bool paused { get { return _paused; } set { SetPaused(value); } }
    private static float _gameStartTime;
    public static float gameTime => Time.time - _gameStartTime;

    public static void StartNewLevel()
    {
        _gameStartTime = Time.time;
    }

    private static void SetTimeScale(float value)
    {
        _timeScale = value;
        if (!paused)
            Time.timeScale = _timeScale;
    }

    private static void SetPaused(bool value)
    {
        _paused = value;
        Time.timeScale = _paused ? 0.0f : _timeScale;
    }
}
