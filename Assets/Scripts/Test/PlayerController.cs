using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public Camera camera;

    private NavMeshAgent m_Agent;

    private Vector3 m_DeltaPos;
    
	void Start ()
	{
	    m_Agent = gameObject.GetComponent<NavMeshAgent>();

	    m_DeltaPos = camera.transform.position - transform.position;
	}
	
	void Update () {
	    if (Input.GetMouseButtonDown(0))
	    {
	        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
	        RaycastHit hit;
	        if (Physics.Raycast(ray, out hit))
	        {
	            m_Agent.SetDestination(hit.point);
	        }
	    }

	    Vector3 campos = transform.position + m_DeltaPos;
	    camera.transform.position = Vector3.Lerp(camera.transform.position, campos, Time.deltaTime*10);
	}
}
