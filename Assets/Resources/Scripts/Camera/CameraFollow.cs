using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public GameObject m_Target;
    public float m_LerpRate = 3.0f;

    private Vector2 m_CurrentPosition;

	// Use this for initialization
	void Start () {
        m_CurrentPosition = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        if (m_Target != null)
        {
            m_CurrentPosition = Vector2.Lerp(m_CurrentPosition, m_Target.transform.position, m_LerpRate);
            Vector3 val = m_CurrentPosition;
            val.z = -10.0f;
            transform.position = val;
        }
	}

    public void SetTarget(GameObject target)
    {
        m_Target = target;
    }

    public void RemoveTarget()
    {
        m_Target = null;
    }

}
