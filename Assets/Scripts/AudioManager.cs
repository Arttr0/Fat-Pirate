using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource; // Источник музыки
    public AudioSource sfxSource; // Источник звуковых эффектов
    public Slider musicSlider; // Слайдер для музыки
    public Slider sfxSlider; // Слайдер для звуковых эффектов
    public AudioClip[] audioClips; // Массив звуковых эффектов

    private void Awake()
    {
        // Проверяем, есть ли уже экземпляр AudioManager
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Установка начальных значений громкости
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Обновление громкости
        UpdateMusicVolume();
        UpdateSFXVolume();
        // Подписка на изменения слайдеров
        musicSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });
        sfxSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
    }
    public void UpdateMusicVolume()
    {
        musicSource.volume = musicSlider.value;
        PlayerPrefs.SetFloat("MusicVolume", musicSlider.value);
    }

    public void UpdateSFXVolume()
    {
        sfxSource.volume = sfxSlider.value;
        PlayerPrefs.SetFloat("SFXVolume", sfxSlider.value);
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < audioClips.Length)
        {
            sfxSource.clip = audioClips[index];
            sfxSource.Play();
        }
        else
        {
            Debug.LogWarning("Index out of bounds: " + index);
        }
    }

    public void StopSFX()
    {
        sfxSource.Stop(); // Останавливает звуковой эффект
    }

    public bool IsSFXPlaying(int index)
    {
        if (index >= 0 && index < audioClips.Length)
        {
            return sfxSource.clip == audioClips[index] && sfxSource.isPlaying;
        }
        return false;
    }

}
