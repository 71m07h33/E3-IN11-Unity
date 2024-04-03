using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// Permet de tourner la cam�ra
/// </summary>
public class CameraRotation : MonoBehaviour
{
    #region Param�tres

    /// <summary>
    /// Instance du script
    /// </summary>
    public static CameraRotation instance;

    private float welcomeRotationSpeed = 0.1f;
    private float inGameRotationSpeed = 2f;

    #region Menu de bienvenue

    [SerializeField]
    private Transform cible; // La cible autour de laquelle la cam�ra tournera

    private float angle = 0.0f; // Angle de rotation actuel

    #endregion

    #endregion

    /// <summary>
    /// Initialise le singleton ou d�truit l'instance en cours
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if(GameManager.singleton.gameStatus == GameStatus.WELCOME)
        {
            // Calculer la position de la cam�ra en fonction de l'angle
            Vector3 offset = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
            transform.position = cible.position + offset;

            // Faire tourner la cam�ra autour de la cible
            transform.LookAt(cible);
            angle += welcomeRotationSpeed * Time.deltaTime;
            if (angle > Mathf.PI * 2)
                angle -= Mathf.PI * 2;
        }
        
    }

    #region Helper

    /// <summary>
    /// Fait tourner la cam�ra autour de son axe vertical en fonction de la valeur xAxisRotation fournie en param�tre
    /// </summary>
    public void TourneLaCamera(float rotationAxeX)
    {
        transform.Rotate(Vector3.down, -rotationAxeX* inGameRotationSpeed);
    }


    #endregion
}
