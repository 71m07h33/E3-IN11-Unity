using UnityEngine;

/// <summary>
/// G�re la sourie en entr�e.
/// </summary>
public class InputManager : MonoBehaviour
{
    #region Param�tres

    [SerializeField]
    private float limiteDeDistance = 1.5f;

    private float distanceDeTrainee;

    private bool peutTournerLaCamera = false;

    #endregion

    /// <summary>
    /// G�re les entr�es utilisateur pour contr�ler la cam�ra et la balle.
    /// </summary>
    void Update()
    {
        // Si on ne joue pas, on ne tiens pas compte des entr�es, on sort de la fonction.
        if (GameManager.singleton.gameStatus != GameStatus.PLAYING)
            return;

        //Si le bouton gauche de la sourie est enfonc� et la cam�ra ne peut pas tourner
        if(Input.GetMouseButtonDown(0) && !peutTournerLaCamera)
        {
            // On peut tourner la cam�ra :)
            peutTournerLaCamera = true;

            // On stock la distance
            GetDistance();

            // Si la distance est dans le p�rim�tre de la balle (1.5f)
            if (distanceDeTrainee <= limiteDeDistance)
                // On tire
                BallController.instance.SourisCliquee();
        }

        // Si on peut tourner
        if(peutTournerLaCamera)
        {
            // Si le clic gauche est enfonc� actuellement
            if(Input.GetMouseButton(0))
            {
                // Si la distance est dans le p�rim�tre de la balle 
                if (distanceDeTrainee <= limiteDeDistance)
                    BallController.instance.SourisEnfoncee();
                else
                    CameraRotation.instance.TourneLaCamera(Input.GetAxis("Mouse X"));
            }

            if (Input.GetMouseButtonUp(0))
            {
                peutTournerLaCamera = false;

                if (distanceDeTrainee <= limiteDeDistance)
                    BallController.instance.SourisRelachee();
            }
        }
    }

    #region Helpers

    /// <summary>
    /// Retourne la distance entre la souris et la balle.
    /// </summary>
    void GetDistance()
    {
        // Cr�er un rayon qui va de la cam�ra � la souris
        var rayon = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Cr�er un plan qui va de la sourie � la balle
        var planSourisBalle = new Plane(Camera.main.transform.forward, BallController.instance.transform.position);

        // Si le rayon intersecte le plan, rayon vaudra true et distance vaudra la distance
        if(planSourisBalle.Raycast(rayon, out float distance))
        {
            // Point d'intersection entre le rayon et le plan
            var pointIntersection = rayon.GetPoint(distance);

            // L'attribut sera la distance entre la sourie et la balle
            distanceDeTrainee = Vector3.Distance(pointIntersection, BallController.instance.transform.position);
        }
    }

    #endregion
}
