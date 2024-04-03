using UnityEngine;

#region Game Status

[System.Serializable]
/// <summary>
/// Etat de l'application
/// </summary>
public enum GameStatus
{
    WELCOME,
    LEVELSELECTION, 
    PLAYING,
    FAILED,
    COMPLETE
}

#endregion

/// <summary>
/// G�re le jeux dans son ensemble
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Param�tres

    /// <summary>
    /// Singleton of the script
    /// </summary>
    public static GameManager singleton;

    #region Param�tre du jeu

    [HideInInspector]
    public int currentLevelIndex = 0;
    [HideInInspector]
    public GameStatus gameStatus = GameStatus.LEVELSELECTION;

    #endregion

    #endregion

    /// <summary>
    /// Initialise le singleton ou d�truit l'instance en cours, assurant qu'une seule instance de ce script existe dans la sc�ne
    /// </summary>
    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
            // Ne supprime pas l'objet � la g�n�ration de nouveau niveau
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
}
