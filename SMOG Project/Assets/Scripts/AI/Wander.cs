using UnityEngine;
using System.Collections;
//Wander
public class Wander : MonoBehaviour
{
    [SerializeField] private GameObject[] wanderingTargets;
    private Transform[] wanderingTargetTransforms;
    private float speed;
    private float turnSpeed;
    private float turnDst;
    private float stoppingDst;
    private float randomRangeMin;
    private float randomRangeMax;
    private float checkRate;
    private float nextCheck;
    private int lastWpIndex;

    private EnemyMaster enemyMaster;
    private Path path;

    private void Start()
    {
        speed = 2.0f;
        turnSpeed = 2.0f;
        turnDst = 1f;
        stoppingDst = 1f;
        randomRangeMin = 14f;
        randomRangeMax = 18f;
        checkRate = Random.Range(randomRangeMin, randomRangeMax);

        enemyMaster = GetComponent<EnemyMaster>();
        wanderingTargets = GameObject.FindGameObjectsWithTag("WanderingTargets");
        wanderingTargetTransforms = System.Array.ConvertAll<GameObject, Transform>(wanderingTargets, x => x.transform);
    }

    private void Update()
    {
        if (Time.time > nextCheck)
        {
            nextCheck = Time.time + checkRate;
            CheckToWander();
        }
    }

    private void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        Debug.Log("Waypoints in the array:" + waypoints.Length);
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private void CheckToWander()
    {
        if (!enemyMaster.playerIsTarget)
        {
            GetRandomWpIndex(0, 4);
            enemyMaster.myTarget = wanderingTargetTransforms[lastWpIndex];
            StartCoroutine(UpdatePath());
            enemyMaster.isOnRoute = true;
        }
    }

    private int GetRandomWpIndex(int min, int max)
    {
        int rndIndex = Random.Range(min, max);

        while (rndIndex == lastWpIndex)
            rndIndex = Random.Range(min, max);

        lastWpIndex = rndIndex;
        Debug.Log("Random WPindex:" + rndIndex);
        return rndIndex;
    }

    private IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }
        PathRequestManager.RequestPath(new PathRequest(transform.position, enemyMaster.myTarget.position, OnPathFound));
    }

    private IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        float speedPercent = 1f;
        transform.LookAt(path.lookPoints[0]);

        while (followingPath && !enemyMaster.playerIsTarget)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex && enemyMaster.playerIsTarget)
                {
                    followingPath = false;
                    break;
                }
                else
                {
                    pathIndex++;
                }
            }

            if (followingPath)
            {
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);

                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                        enemyMaster.isOnRoute = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}