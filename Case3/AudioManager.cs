using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource sfxSource;

    // 記憶體中由程式產生的音效片段
    private AudioClip clipButtonClick;
    private AudioClip clipScreenClick;
    private AudioClip clipDialogue;
    private AudioClip clipSystemPrompt;
    private AudioClip clipCorrect;
    private AudioClip clipWrong;
    private AudioClip clipAlarm;
    private AudioClip clipSuccess;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        // 🌟 自動用數學波形合成全套音效（免匯入任何 MP3/WAV 檔案）
        GenerateAllProceduralSFX();
    }

    void Start()
    {
        AutoBindAllButtonSFX();
    }

    /// <summary>
    /// 自動為場景中所有按鈕綁定音效
    /// </summary>
    public void AutoBindAllButtonSFX()
    {
        Button[] allButtons = Resources.FindObjectsOfTypeAll<Button>();
        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(PlayButtonClick);
            btn.onClick.AddListener(PlayButtonClick);
        }
    }

    // ==================== 🔊 播放介面 ====================

    public void PlayButtonClick() => PlaySFX(clipButtonClick, 0.4f);
    public void PlayScreenClick() => PlaySFX(clipScreenClick, 0.3f);
    public void PlayDialogue() => PlaySFX(clipDialogue, 0.3f);
    public void PlaySystemPrompt() => PlaySFX(clipSystemPrompt, 0.5f);
    public void PlayCorrect() => PlaySFX(clipCorrect, 0.6f);
    public void PlayWrong() => PlaySFX(clipWrong, 0.6f);
    public void PlayAlarm() => PlaySFX(clipAlarm, 0.6f);
    public void PlaySuccess() => PlaySFX(clipSuccess, 0.7f);

    private void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    // ==================== 🧮 程式波形生成器 (Procedural Audio) ====================

    private void GenerateAllProceduralSFX()
    {
        clipButtonClick = CreateTone(1200f, 0.04f, 1f, WaveType.Sine, true);
        clipScreenClick = CreateTone(600f, 0.03f, 0.8f, WaveType.Sine, true);
        clipDialogue = CreateTone(800f, 0.05f, 0.6f, WaveType.Triangle, true);
        clipSystemPrompt = CreateArpeggio(new float[] { 523.25f, 659.25f, 783.99f }, 0.06f, WaveType.Sine); // C5 - E5 - G5
        clipCorrect = CreateArpeggio(new float[] { 523.25f, 659.25f, 1046.50f }, 0.1f, WaveType.Sine);     // 叮咚！
        clipWrong = CreateTone(150f, 0.2f, 1f, WaveType.Square, false);                                      // 蜂鳴嗶嗶！
        clipAlarm = CreateAlarmBeep();                                                                        // 鬧鐘嗶嗶嗶！
        clipSuccess = CreateFanfare();                                                                       // 關卡勝利慶祝音！
    }

    private enum WaveType { Sine, Square, Triangle, Noise }

    private AudioClip CreateTone(float frequency, float duration, float decay, WaveType waveType, bool pitchDrop = false)
    {
        int sampleRate = 44100;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float currentFreq = pitchDrop ? Mathf.Lerp(frequency, frequency * 0.5f, t / duration) : frequency;
            float wave = GetWaveSample(currentFreq, t, waveType);
            float envelope = Mathf.Pow(1f - (t / duration), decay); // 音量遞減包絡線

            samples[i] = wave * envelope;
        }

        AudioClip clip = AudioClip.Create("ProceduralTone", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateArpeggio(float[] frequencies, float noteDuration, WaveType waveType)
    {
        int sampleRate = 44100;
        int totalSamples = (int)(sampleRate * noteDuration * frequencies.Length);
        float[] samples = new float[totalSamples];

        for (int note = 0; note < frequencies.Length; note++)
        {
            int startSample = (int)(note * noteDuration * sampleRate);
            int noteSamples = (int)(noteDuration * sampleRate);

            for (int i = 0; i < noteSamples; i++)
            {
                float t = (float)i / sampleRate;
                float wave = GetWaveSample(frequencies[note], t, waveType);
                float envelope = 1f - (t / noteDuration);

                samples[startSample + i] = wave * envelope;
            }
        }

        AudioClip clip = AudioClip.Create("ProceduralArpeggio", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateAlarmBeep()
    {
        int sampleRate = 44100;
        float duration = 0.5f;
        int totalSamples = (int)(sampleRate * duration);
        float[] samples = new float[totalSamples];

        for (int i = 0; i < totalSamples; i++)
        {
            float t = (float)i / sampleRate;
            bool isBeepPeriod = (t % 0.15f) < 0.08f; // 每 0.15 秒嗶一聲
            float wave = isBeepPeriod ? GetWaveSample(1500f, t, WaveType.Square) : 0f;
            samples[i] = wave * 0.5f;
        }

        AudioClip clip = AudioClip.Create("ProceduralAlarm", totalSamples, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    private AudioClip CreateFanfare()
    {
        float[] melody = new float[] { 523.25f, 659.25f, 783.99f, 1046.50f }; // C - E - G - High C
        return CreateArpeggio(melody, 0.12f, WaveType.Triangle);
    }

    private float GetWaveSample(float frequency, float time, WaveType waveType)
    {
        float phase = time * frequency * 2f * Mathf.PI;
        switch (waveType)
        {
            case WaveType.Square:
                return Mathf.Sign(Mathf.Sin(phase));
            case WaveType.Triangle:
                return Mathf.PingPong(time * frequency * 4f, 2f) - 1f;
            case WaveType.Noise:
                return Random.Range(-1f, 1f);
            case WaveType.Sine:
            default:
                return Mathf.Sin(phase);
        }
    }
}