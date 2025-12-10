using UnityEngine;
using UnityEngine.AI;
public class AiTarget : MonoBehaviour
{
    public Transform Target;
    public float AttackDistance;
    private NavMeshAgent m_Agent;
    private Animator m_Animator;
    public float m_Distance;
    private Vector3 m_startingPoint;
    private bool m_pathCalculate = true;
    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
        //m_Animator = GetComponent<Animator>();
        m_startingPoint = transform.position;
    }

    void Update()
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, Target.position);
        if (m_Distance < AttackDistance)
        {
            m_Agent.isStopped = true;
        } else
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
