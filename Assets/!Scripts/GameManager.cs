using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    public bool GameState = true;
    [SerializeField] GameObject PauseMenu;


    [Tooltip("Mute volume level in decibels")]
    public float muteVolume = -80f;
    [Tooltip("Unmuted volume level in decibels (0 = full volume)")]
    public float normalVolume = 0f;

    [SerializeField] Image AudioIconImage;
    [SerializeField] Sprite MuteIcon, UnmuteIcon;


    [Header("UI")]
    [SerializeField] TextMeshProUGUI Points;
    [SerializeField] int Score = 0;

    [Header("Audio")]
    public AudioMixer audioMixer;
    private bool isMuted = false;
    public void ChangeGameState()
    {
        GameState = !GameState;
    }
    public void ToggleMute()
    {
        isMuted = !isMuted;

        float targetVolume = isMuted ? muteVolume : normalVolume;

        audioMixer.SetFloat("MasterVol", targetVolume);

        Debug.Log("Muted: " + isMuted);
    }

    private void Update()
    {
        if (GameState)
        {
            Time.timeScale = 1f;
            PauseMenu.SetActive(false);
        }
        else
        {
            PauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }

        Points.text = Score.ToString();

        if (isMuted)
        {
            AudioIconImage.sprite = MuteIcon;
        }
        else
        {
            AudioIconImage.sprite = UnmuteIcon;
        }
    }

    public void Addscore(int Amount)
    {
        Score += Amount;
    }
}
