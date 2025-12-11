using UnityEngine;
using UnityEngine.AI;
public class AiTarget : MonoBehaviour
{
    public Transform Target;
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    public float m_Distance;
    private Vector3 m_startingPoint;
    private bool m_pathCalculate = true;
    private AiStats stats;
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        m_startingPoint = transform.position;
        stats = GetComponent<AiStats>();

        m_Agent.speed = stats.movespeed;
        if (stats.flyHeight > 0) m_Agent.baseOffset = stats.flyHeight;

        if (Target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                Target = playerObj.transform;
            }
        }

        if (m_Agent.isOnNavMesh == false)
        {
            Debug.LogWarning(name + " was not on NavMesh! Trying to fix...");
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position; // Teleport to valid ground
                m_Agent.Warp(hit.position);        // Tell Agent we moved
            }
        }

    }


    void Update()
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, Target.position);
        if (m_Distance > stats.DetectDistance)
        {
            m_Agent.isStopped = false;
            m_Agent.destination = m_startingPoint;
            m_pathCalculate = false;
        } else if (m_Distance < stats.AttackDistance)
        {
            m_Agent.isStopped = true;
        }
        else
        {
            m_Agent.isStopped = false;
            if (!m_Agent.hasPath && m_pathCalculate)
            {
                m_Agent.destination = m_startingPoint;
                m_pathCalculate = false;
            }
            else
            {
                m_Agent.destination = Target.position;
                m_pathCalculate = true;
            }
        }

    }
}
