using UnityEngine;
using System.Collections;
//Aggro
public class Pursue : MonoBehaviour
{
    private const float pathUpdateMoveThreshold = .1f;
    private const float minPathUpdateTime = .1f;

    [SerializeField] private Transform head;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask sightLayer;
    private RaycastHit hit;
    [SerializeField] private float detectRadius;
    [SerializeField] private float speed;
    private float turnSpeed;
    private float turnDst;
    private float stoppingDst;
    private float checkRate;
    private float nextCheck;

    private EnemyMaster enemyMaster;
    private Path path;
    private Wander wander;

    private void Start()
    {
        speed = 8.0f;
        turnSpeed = 2.0f;
        turnDst = 3f;
        stoppingDst = 1f;
        detectRadius = 80.0f;
        checkRate = 5f;

        head = GameObject.FindGameObjectWithTag("Head").transform;
        enemyMaster = GetComponent<EnemyMaster>();
    }

    private void Update()
    {
        CarryOutDetection();
    }

    private void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    private void CarryOutDetection()
    {
        if (Time.time > nextCheck)
        {
            nextCheck = Time.time + checkRate;

            Collider[] colliders = Physics.OverlapSphere(head.position, detectRadius, playerLayer);

            if (colliders.Length > 0)
            {
                foreach (Collider potentialTargetCollider in colliders)
                {
                    if (potentialTargetCollider.CompareTag("Player"))
                    {
                        if (CanPotentialTargetBeenSeen(potentialTargetCollider.transform))
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private bool CanPotentialTargetBeenSeen(Transform potentialTarget)
    {
        if (Physics.Linecast(head.position, potentialTarget.position, out hit, sightLayer))
        {
            Debug.DrawLine(head.position, potentialTarget.position, Color.green);

            if (hit.transform == potentialTarget)
            {
                enemyMaster.myTarget = potentialTarget;
                StartCoroutine(UpdatePath());
                enemyMaster.isOnRoute = true;
                enemyMaster.playerIsTarget = true;
                return true;
            }
            else
            {
                enemyMaster.isOnRoute = false;
                enemyMaster.myTarget = null;
                enemyMaster.playerIsTarget = false;
                return false;
            }
        }
        else
        {
            enemyMaster.isOnRoute = true;
            enemyMaster.playerIsTarget = false;
            enemyMaster.myTarget = null;
            return false;
        }
    }

    private IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }

        PathRequestManager.RequestPath(new PathRequest(transform.position, enemyMaster.myTarget.position, OnPathFound));
        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = enemyMaster.myTarget.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);

            if (enemyMaster.myTarget == null)
                break;

            if ((enemyMaster.myTarget.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, enemyMaster.myTarget.position, OnPathFound));
                targetPosOld = enemyMaster.myTarget.position;
            }
        }
    }

    private IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        float speedPercent = 1f;
        transform.LookAt(path.lookPoints[0]);

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex || !enemyMaster.playerIsTarget)
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

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
    }
}
