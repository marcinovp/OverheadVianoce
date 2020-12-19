using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] Animator cameraAnimator;
    [SerializeField] List<Pismeno> pismena;

    [SerializeField] GameObject skipButton;
    [SerializeField] GameObject restartButton;

    private const string cameraAnimatorParam = "naPozdrav";

    void Start()
    {
        skipButton.SetActive(true);
        restartButton.SetActive(false);

        foreach (Pismeno pismeno in pismena)
        {
            pismeno.OnPismenoDropped += OnPismenoDropped;
        }
    }

    public void PreskocitHru()
    {
        for (int i = pismena.Count - 1; i >= 0; i--)
        {
            pismena[i].Drop();
        }
    }

    public void HratZnovu()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    private void OnPismenoDropped(Pismeno pismeno)
    {
        pismena.Remove(pismeno);

        if (pismena.Count == 0)
        {
            GoToPozdrav();

            skipButton.gameObject.SetActive(false);
            restartButton.gameObject.SetActive(true);
        }
    }

    private void GoToPozdrav()
    {
        cameraAnimator.SetBool(cameraAnimatorParam, true);
    }
}
