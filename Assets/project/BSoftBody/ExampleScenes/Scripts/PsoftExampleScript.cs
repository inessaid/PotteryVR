using UnityEngine;
using System.Collections;

public class PsoftExampleScript : MonoBehaviour {

	BSoftBody[] psoftBodies;

	// Use this for initialization
	void Start () {
		//find all psoftbodies in the scene and assign them to the psoftBodies array variable
		psoftBodies = Object.FindObjectsOfType<BSoftBody>();
	}
	
	// Update is called once per frame
	void Update(){

		//if key R is pressed, reset all psoftbody meshes
		if(Input.GetKeyDown(KeyCode.R)){
			foreach(BSoftBody ps in psoftBodies)
				ps.ResetMesh();
		}

		//if key E is held, lerp all vertices in the mesh of each psoftbody towards the original mesh 
		if(Input.GetKey(KeyCode.E)){
			foreach(BSoftBody ps in psoftBodies)
				ps.LerpTowardsOriginal(Time.deltaTime * 8f);
		}

		//if key T is held, perform random impacts on the mesh of each psoftbody
		if(Input.GetKey(KeyCode.T)){
			foreach(BSoftBody ps in psoftBodies)
				ps.RandomImpact();
		}

		//if key Y is Pressed, Shoot a ray from the mouse and if it hits a psoft body, deform it at the hit point
		if(Input.GetKeyDown(KeyCode.Y)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit)){
				BSoftBody hitBody = hit.transform.GetComponent<BSoftBody>();
				if(hitBody)
					hitBody.DeformAtPoint(hit.point, hit.normal, 35f, 0.7f);
			}
		}

	}



}
