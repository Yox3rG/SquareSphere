using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverSquareDeath : MonoBehaviour
{
    public GameObject particle_squareDeath;

    private void Start()
    {
        SubjectSquare.OnDeath += SubjectSquare_OnDeath;
    }

    private void OnDestroy()
    {
        SubjectSquare.OnDeath -= SubjectSquare_OnDeath;
    }

    private void SubjectSquare_OnDeath(Vector2 position, byte colorIndex)
    {
        GameObject g = Instantiate(particle_squareDeath, position, Quaternion.identity);
        ParticleSystem.MainModule particleMain = g.GetComponent<ParticleSystem>().main;
        Color c = Palette.GetColor(colorIndex);
        particleMain.startColor = c;

        Destroy(g, 2f);
    }
}