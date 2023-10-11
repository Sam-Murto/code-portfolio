using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHome : BaseGhostBehavior
{
    private float amplitude = 0.25f;

    //No collisions for this behavior
    private void OnEnable()
    {
        ghost = GetComponent<Ghost>();
        ghost.movement.SetCheckCollisions(false);
        ghost.movement.SetDirection(Vector2.zero, true);
        ghost.movement.SetSpeed(speed);
        ghost.material.color = ghost.defaultColor;
        StartCoroutine(Oscillate());
        Invoke(nameof(Exit), duration);

        ghost.isHome = true;
    }

    //Turn collisions back on and set direction
    private void OnDisable()
    {
        CancelInvoke();
        StopAllCoroutines();
        ghost.movement.SetCheckCollisions(true);
        float randomNum = Random.Range(0, 1);
        ghost.movement.SetDirection((randomNum < 0.5f) ? Vector3.left : Vector3.right);

        ghost.isHome = false;
    }

    //Just here to use with invoke
    private void Exit()
    {
       StopAllCoroutines();
       StartCoroutine(ExitCoroutine());
    }

    private IEnumerator Oscillate()
    {
        Vector3 restPosition = ghost.transform.position;
        float t = 0.0f;
        while (true)
        {
            if (!GameManager.isPaused)
            {
                Vector3 position = transform.position;
                position.y = amplitude * Mathf.Sin(t * Mathf.PI * 4) + restPosition.y;
                ghost.transform.position = position;
                t += Time.deltaTime;
            }
            yield return null;
        }

    }

    private IEnumerator ExitCoroutine()
    {
        Transform homeCenter = FindObjectOfType<HomeCenter>().transform;
        Transform homeExit = FindObjectOfType<HomeEntrance>().transform;

        float duration = 0.5f;
        float normalize_t = 1.0f / duration;

        // Go to the center of home
        for(float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            Vector3 newPosition = Vector2.Lerp(ghost.restPosition, homeCenter.position, t * normalize_t);
            newPosition.z = transform.position.z;
            transform.position = newPosition;

            yield return null;
        }
        
        //Exit home
        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            Vector3 newPosition = Vector2.Lerp(homeCenter.position, homeExit.position, t * normalize_t);
            newPosition.z = transform.position.z;
            transform.position = newPosition;

            yield return null;
        }


        if (ghost.isVulnerable)
            ghost.TransitionToBehavior(ghost.flee);
        else
            ghost.TransitionToBehavior(ghost.scatter);

    }

}
