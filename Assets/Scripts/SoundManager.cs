using UnityEngine;
using System;

/// <summary>
/// Gère les sons du jeux.
/// </summary>
public class SoundManager : MonoBehaviour
{
    #region Paramètres

    public static SoundManager singleton;

    #region Sons

    [SerializeField]
    private SoundData[] musicSound;

    [SerializeField]
    private SoundData[] sfxSound;

    #endregion

    [SerializeField]
    private AudioSource musicSource;
    [SerializeField]
    private AudioSource sfxSource;

    #endregion

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;

            // Considère SoundManager comme un singleton.
            DontDestroyOnLoad(gameObject); // Permet a la musique de ne pas recommencer à chaque chargement.
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        PlayMusic("theme");
    }

    #region Music and SFX

    private void PlayMusic(string nom)
    {
        SoundData s = Array.Find(musicSound, x => x.nom == nom);

        if (s != null)
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
        else
        {
            Debug.Log("Son introuvable");
        }
    }

    public void PlaySfx(string nom)
    {
        SoundData s = Array.Find(sfxSound, x => x.nom == nom);

        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.Log("Son introuvable");
        }
    }

    #endregion

    #region Sliders

    public void VolumeMusic(float volume)
    {
        musicSource.volume = volume;
    }

    public void VolumeSfx(float volume)
    {
        sfxSource.volume = volume;
    }

    #endregion
}
