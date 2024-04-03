using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// G�re les actions de la balle
/// </summary>
public class BallController : MonoBehaviour
{
    #region Param�tres

    /// <summary>
    /// Instance du script
    /// </summary>
    public static BallController instance;

    #region Contr�le du jeu

    /// <summary>
    /// Permet de dessiner la ligne de tire du joueur
    /// </summary>
    [SerializeField]
    private LineRenderer ligneDeTire;

    /// <summary>
    /// Zone autour de la balle, pour une utilisation plus claire
    /// </summary>
    [SerializeField]
    private GameObject zoneDeTire;

    /// <summary>
    /// Permet de sp�cifier sur quelle couche on calcule la position du clic de la souris
    /// </summary>
    [SerializeField]
    private LayerMask CoucheDeTire;

    private Rigidbody rigidBody;

    #endregion

    #region Force

    [SerializeField]
    private float forceMaximal;
    [SerializeField]
    private float coefficientModificationForce;

    private float force;

    #endregion

    #region Direction

    private Vector3 positionDepart;
    private Vector3 positionArrivee;
    private Vector3 direction;

    #endregion

    #region Etat de la balle

    private ParticleSystem streak;
    private bool peutTirer = false;
    private bool estStatique = true;

    #endregion

    #endregion

    /// <summary>
    /// Initialise le singleton instance et r�cup�re le composant Rigidbody attach� au GameObject.
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        rigidBody = GetComponent<Rigidbody>();
        streak = FindAnyObjectByType<ParticleSystem>();
    }

    /// <summary>
    /// Configure la cam�ra pour suivre la balle
    /// </summary>
    private void Start()
    {
        CameraFollow.instance.InstancieCible(gameObject);
    }

    /// <summary>
    /// V�rifie si la balle est immobile et d�clenche une action si c'est le cas
    /// </summary>
    private void Update()
    {
        streak.startSpeed = rigidBody.velocity.x;

        if (rigidBody.velocity == Vector3.zero && !estStatique)
        {
            // Met les variables de la balle dans un �tat statique
            estStatique = true;
            rigidBody.velocity = Vector3.zero;
            // Permet de tirer
            zoneDeTire.SetActive(true);
            // Permet de d�compter un coup
            LevelManager.instance.CoupTire();

        }
            
    }

    /// <summary>
    /// Applique la force au Rigidbody de la balle lorsque le tir est effectu�, d�clenche des actions et des sons associ�s
    /// </summary>
    private void FixedUpdate()
    {
        if (peutTirer)
        {
            // Modifie les boul�ens d'�tats de la balle
            peutTirer = false;
            estStatique = false;
            // D�sactive la zone de tire
            zoneDeTire.SetActive(false);

            // Met un coup dans la balle
            direction = positionDepart - positionArrivee;
            rigidBody.AddForce(direction * force, ForceMode.Impulse);
            force = 0;

            // Initialise les positions de d�part et d'arriv�e � zero
            positionDepart = Vector3.zero;
            positionArrivee = Vector3.zero;

            // Joue un son de tire
            SoundManager.singleton.PlaySfx("throw");
        }
    }

    #region Collisions

    /// <summary>
    /// G�re les collisions avec d'autres objets, d�clenchant des actions sp�cifiques lorsque la balle entre en collision avec un objet "Hole" ou "Destroyer".
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // Si la balle est dans le troue
        if(other.name == "Hole")
        {
            // Termine le niveau
            LevelManager.instance.NiveauTermine();
        }

        // Si la balle atteint les limites de la carte
        if (other.name == "Terrain")
            // Echoue le niveau
            LevelManager.instance.NiveauPerdue();
    }

    #endregion

    #region M�thode de souris

    /// <summary>
    /// D�clench� lors du clic de souris, enregistre la position de d�part pour le tir
    /// </summary>
    public void SourisCliquee()
    {
        // On ne tire pas si la balle roule
        if (!estStatique)
            return;
        
        // Dessine une ligne
        ligneDeTire.gameObject.SetActive(true);

        // Enregistre la position de d�part et dessine la ligne jusqu'� la souris
        positionDepart = PositionClique();
        ligneDeTire.SetPosition(0, ligneDeTire.transform.localPosition);
    }

    /// <summary>
    /// Calcul de la force du tir en fonction de la distance entre la position de d�part et la position actuelle de la souris
    /// </summary>
    public void SourisEnfoncee()
    {
        // Enregistre la position d'arrivee
        positionArrivee = PositionClique();
        positionArrivee.y = ligneDeTire.transform.position.y;

        // Enregistre la force du joueur
        force = Mathf.Clamp(Vector3.Distance(positionArrivee, positionDepart) * coefficientModificationForce, 0, forceMaximal);

        // Dessine la ligne jusqu'� la position d'arriv�e
        ligneDeTire.SetPosition(1, transform.InverseTransformPoint(positionArrivee));
    }

    /// <summary>
    /// D�clench� lorsque le bouton de la souris est rel�ch�, indiquant que le tir est pr�t � �tre effectu�
    /// </summary>
    public void SourisRelachee()
    {
        // Modifie la variable d'�tat de la balle
        peutTirer = true;

        // Supprime la ligne
        ligneDeTire.gameObject.SetActive(false);
    }

    #endregion

    #region Helper

    /// <summary>
    /// Renvoie la position dans le monde o� le rayon de la souris interagit avec les objets de la sc�ne
    /// </summary>
    Vector3 PositionClique()
    {
        // Cr�er une position initialis� � la position � z�ro
        Vector3 position = Vector3.zero;
        // Cr�er un rayon partant de la souris
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // D�tecte une intersection entre le rayon partant de la souris et la couhe de tire.
        if (Physics.Raycast(ray, out RaycastHit intersection, Mathf.Infinity, CoucheDeTire))
        {
            position = intersection.point;
        }

        // Retourne la position du clic
        return position;
    }

    #endregion
}
