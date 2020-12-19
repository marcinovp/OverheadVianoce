using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] Animator cameraAnimator;
    [SerializeField] List<Pismeno> pismena;

    private const string cameraAnimatorParam = "naPozdrav";

    void Start()
    {
        foreach (Pismeno pismeno in pismena)
        {
            pismeno.OnPismenoDropped += OnPismenoDropped;
        }
    }

    private void OnPismenoDropped(Pismeno pismeno)
    {
        pismena.Remove(pismeno);

        if (pismena.Count == 0)
        {
            GoToPozdrav();
        }
    }

    private void GoToPozdrav()
    {
        cameraAnimator.SetBool(cameraAnimatorParam, true);
    }
}
