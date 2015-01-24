﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	private GameObject grabMigeonLabel;
	private GameObject releaseMigeonLabel;

    Transform carriedMigeon = null;
    Transform migeonItWantsToFuck = null;
    float timeWantToFuck = 0;
 
	// Use this for initialization
	void Start ()
	{
		grabMigeonLabel = GameObject.Find("GrabMigeonLabel");
		releaseMigeonLabel = GameObject.Find("ReleaseMigeonLabel");

        if (grabMigeonLabel != null)
            grabMigeonLabel.SetActive(false);

        if (releaseMigeonLabel != null)
            releaseMigeonLabel.SetActive(false);
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        if(carriedMigeon == null)
        {
            RaycastHit hit;
            bool found = false;
            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                found = true;
            if(!found)
                if(Physics.Raycast(Camera.main.transform.position,  Quaternion.AngleAxis(-10, Vector3.up) * Camera.main.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                    found = true;
            if(!found)
                if(Physics.Raycast(Camera.main.transform.position, Quaternion.AngleAxis(+10, Vector3.up) * Camera.main.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                    found = true;

            if (found)
            {
                if (grabMigeonLabel != null)
                    grabMigeonLabel.SetActive(true);

                if (releaseMigeonLabel != null)
                    releaseMigeonLabel.SetActive(false);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    carriedMigeon = hit.collider.transform;
                    carriedMigeon.GetComponent<MigeonBehavior>().takeControl(true);
                    carriedMigeon.rigidbody.isKinematic = true;
                }
            }
            else
            {
                if (grabMigeonLabel != null)
                    grabMigeonLabel.SetActive(false);

                if (releaseMigeonLabel != null)
                    releaseMigeonLabel.SetActive(false);
            }
        }
        else
		{
            //On porte un migeon
            carriedMigeon.transform.LookAt(carriedMigeon.transform.position + transform.forward, Vector3.up); 
            Vector3 posMigeonInArms = Camera.main.transform.position + Camera.main.transform.forward * 2f;
            carriedMigeon.transform.position = posMigeonInArms;

            //On regarde s'il veut niquer
            if (migeonItWantsToFuck == null)
            {
                RaycastHit hit;
                if (Physics.Raycast(carriedMigeon.transform.position, carriedMigeon.transform.forward, out hit, 3f, LayerMask.GetMask("Migeon")))
                {
                    timeWantToFuck = 0;
                    migeonItWantsToFuck = hit.transform;
                }
            }

            if (migeonItWantsToFuck != null)
            {
                Vector3 directionMigeon = migeonItWantsToFuck.position - carriedMigeon.position;
                timeWantToFuck += Time.deltaTime;

                //Si trop grande distance
                if (directionMigeon.magnitude > 6.0f)
                {
                    migeonItWantsToFuck = null;
                }
                else
                {
                    if (directionMigeon.magnitude > 1)
                        directionMigeon.Normalize();

                    carriedMigeon.transform.position = Vector3.Lerp(posMigeonInArms, posMigeonInArms + directionMigeon, timeWantToFuck);
                    directionMigeon.y = 0;
                    carriedMigeon.transform.LookAt(carriedMigeon.transform.position + directionMigeon, Vector3.up); 
                } 
            }


			if (grabMigeonLabel != null)
				grabMigeonLabel.SetActive(false);
			
			if (releaseMigeonLabel != null)
				releaseMigeonLabel.SetActive(true);
			
			if (Input.GetKeyDown(KeyCode.E))
			{
                carriedMigeon.GetComponent<MigeonBehavior>().takeControl(false);
                carriedMigeon.rigidbody.isKinematic = false;
                carriedMigeon = null;
                migeonItWantsToFuck = null;
                if (grabMigeonLabel != null)
				    grabMigeonLabel.SetActive(false);

			    if (releaseMigeonLabel != null)
				    releaseMigeonLabel.SetActive(false);
			}
		}
	}
}
