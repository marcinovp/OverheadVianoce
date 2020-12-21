using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Animator cameraAnimator;
    [SerializeField] private DroneController droneController;
    [SerializeField] private AudioSource cutAudio;
    [SerializeField] private List<Pismeno> pismena;

    [Header("GUI")]
    [SerializeField] private GameObject skipButton;
    [SerializeField] private GameObject restartButton;
    
    [SerializeField] private string overheadUrl = "https://overhead4d.com/";

    [Header("Mute")]
    [SerializeField] private Toggle muteToggle;
    [SerializeField] private AudioSource podmaz;

    private const string cameraAnimatorParam = "naPozdrav";
    private const string mutePrefParam = "isMuted";

    void Start()
    {
        skipButton.SetActive(true);
        restartButton.SetActive(false);
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);

        bool wasStarted = RuntimeStorage.GetValueBoolean("wasStarted", false);
        if (!wasStarted)
        {
            DontDestroyOnLoad(podmaz.gameObject);
            RuntimeStorage.SetValue("wasStarted", true);
        }
        else
            Destroy(podmaz.gameObject);

        foreach (Pismeno pismeno in pismena)
        {
            pismeno.OnPismenoDropped += OnPismenoDropped;
        }

        bool isMuted = RuntimeStorage.GetValueBoolean(mutePrefParam, true);
        OnMuteValueChanged(isMuted);
        muteToggle.isOn = isMuted;
    }

    public void PreskocitHru()
    {
        droneController.AutoCutOff();
        droneController.BlockInput(true);
        skipButton.gameObject.SetActive(false);
        //for (int i = pismena.Count - 1; i >= 0; i--)
        //{
        //    pismena[i].Drop();
        //}
    }

    public void HratZnovu()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void GoToOverhead()
    {
        Application.OpenURL(overheadUrl);
    }

    private void OnPismenoDropped(Pismeno pismeno)
    {
        pismena.Remove(pismeno);
        cutAudio.Play();

        if (pismena.Count == 0)
        {
            GoToPozdrav();

            droneController.BlockInput(true);
            skipButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(true);
        }
    }

    private void GoToPozdrav()
    {
        cameraAnimator.SetBool(cameraAnimatorParam, true);
    }

    public void OnMuteValueChanged(bool value)
    {
        RuntimeStorage.SetValue(mutePrefParam, value);
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (var audio in audioSources)
        {
            audio.mute = value;
        }
    }
}
