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
    [SerializeField] GameObject CameraObject;
    [SerializeField] float CamerafollowSpeed = 2f;
    [SerializeField] float targetcamfeed;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI Points;
    [SerializeField] Transform Startingpoint;
    [SerializeField] int StartingDepth = 500;
    [SerializeField] int m_by = 100;
    [SerializeField] Transform Endpoint;
    [SerializeField] TextMeshProUGUI Distance;
    [SerializeField] int Score = 0;

    [Header("Audio")]
    public AudioMixer audioMixer;
    private bool isMuted = false;

    private void Start()
    {
        if (CameraObject == null && Camera.main != null)
            CameraObject = Camera.main.gameObject;
        targetcamfeed = -25;
    }
    public void SetTargetZ(float newZ)
    {
        targetcamfeed = newZ;
    }

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


        Vector3 camPos = CameraObject.transform.position;
        camPos.z = Mathf.Lerp(camPos.z, targetcamfeed, Time.deltaTime * CamerafollowSpeed);
        CameraObject.transform.position = camPos;

        float rawDistance = Startingpoint.transform.position.y - Endpoint.transform.position.y;
        
        // Round to the nearest 10 meters (instead of 1)
        int distanceInTens = Mathf.RoundToInt(rawDistance / 10f) * m_by;

        // Add a starting offset to avoid "0 m" — for example, start at 100 m
        int offset = StartingDepth;
        int depthValue = distanceInTens + offset;

        // Format as 3 digits (or 4 if it can go beyond 999m)
        string formattedDistance = depthValue.ToString("D3");

        // Apply to UI
        Distance.text = formattedDistance + " m";
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

    public void setspeedcam(float speed)
    {
        CamerafollowSpeed = speed;
    }

    public void InteractionOn()
    {
        Interactive.Interacting = true;
    }

    public void InteractionOff()
    {
        Interactive.Interacting = false;
    }
}


