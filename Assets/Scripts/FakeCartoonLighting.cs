using UnityEngine;
using System.Collections;
using System;

public class FakeCartoonLighting : MonoBehaviour {

    [Serializable]
    public class LightingObjectProfile {

        public SpriteRenderer Graphic;
        public Transform Center;
        public float AngularRange;

        [NonSerialized] public bool isActive;

    }

    public bool InitialOnStart = true;
    public bool CheckLightPosition = true;
    public bool CheckObjectPosition = true;
    public bool CheckObjectRotation = true;

    [Space]

    public Light LightObject;

    [Space]

    public Transform ObjectCenter;
    public LightingObjectProfile[] LightingObjects;

    private Vector3 previousLightPosition;
    private Vector3 previousObjectPosition;
    private Quaternion previousObjectRotation;
    
    void Start() {
        if (InitialOnStart)
            Initialise();
    }

    void Update() {
        UpdateLighting();
    }

    public bool IsValid() {
        for (int i = 0; i < LightingObjects.Length; i++) {
            LightingObjectProfile profile = LightingObjects[i];

            // The minumim information that must be supplied
            if (profile.Graphic == null || profile.AngularRange <= 0)
                return false;

        }

        return LightingObjects != null && LightingObjects.Length != 0;
    }

    public void Initialise() {
        for (int i = 0; i < LightingObjects.Length; i++) {
            LightingObjectProfile profile = LightingObjects[i];

            if (profile.Center == null)
                profile.Center = profile.Graphic.transform;

            // Set the lighting object graphic just in front of the main object
            Vector3 tmp = profile.Graphic.transform.position;
            tmp.z = ObjectCenter.transform.position.z - 0.1f;
            profile.Graphic.transform.position = tmp;

            // Deactivate to begin with
            profile.isActive = false;
            profile.Graphic.gameObject.SetActive(false);

        }

        // Record an initial position of the light and object, and the rotation of the object
        previousLightPosition = LightObject.transform.position;
        previousObjectPosition = ObjectCenter.transform.position;
        previousObjectRotation = ObjectCenter.transform.rotation;

    }

    bool lightHasMoved = false;
    bool objectHasMoved = false;
    bool objectHasRotated = false;

    public void UpdateLighting() {
        Vector2 ToObject;
        float percentage;

        // Check if the light has moved
        if (CheckLightPosition)
            lightHasMoved = (previousLightPosition != LightObject.transform.position);
        
        // Check if the object has moved
        if (CheckObjectPosition)
            objectHasMoved = (previousObjectPosition != ObjectCenter.transform.position);

        // Check if the object has moved
        if (CheckObjectRotation)
            objectHasRotated = (previousObjectRotation != ObjectCenter.transform.rotation);

        // Update previous position as neccesary or stop updating if nothing has moved
        if (lightHasMoved) {
            previousLightPosition = LightObject.transform.position;
            if (objectHasMoved) {
                previousObjectPosition = ObjectCenter.transform.position;
                if (objectHasRotated) {
                    previousObjectRotation = ObjectCenter.transform.rotation;
                }
            } else {
                if (objectHasRotated) {
                    previousObjectRotation = ObjectCenter.transform.rotation;
                }
            }
        } else {
            if (objectHasMoved) {
                previousObjectPosition = ObjectCenter.transform.position;
                if (objectHasRotated) {
                    previousObjectRotation = ObjectCenter.transform.rotation;
                }
            } else {
                if (objectHasRotated) {
                    previousObjectRotation = ObjectCenter.transform.rotation;
                } else {
                    return;
                }
            }
        }

        // Get an idea of the distance to the light
        Vector2 ToLight = ObjectCenter.position - LightObject.transform.position;
        float sqrDistanceToLight = ToLight.sqrMagnitude;
        
        for (int i = 0; i < LightingObjects.Length; i++) {
            LightingObjectProfile profile = LightingObjects[i];

            // Find percentage within angluar range
            ToObject = ObjectCenter.position - profile.Center.transform.position;
            percentage = (Mathf.Acos(Vector3.Dot(ToObject, ToLight) / Mathf.Sqrt(ToObject.sqrMagnitude * sqrDistanceToLight)) * Mathf.Rad2Deg) / profile.AngularRange;

            if (percentage <= 1) {
                // If percentage indicates that the light is within the angular range

                // Activate if not already
                if (!profile.isActive) { 
                    profile.Graphic.gameObject.SetActive(true);
                    profile.isActive = true;
                }

                // Set transparency of sprite according to percentage
                Color tmp = LightObject.color;
                tmp.a = 1 - percentage;
                profile.Graphic.color = tmp;

            } else if (profile.isActive) {
                // If outside range and active, deactivate

                profile.isActive = false;
                profile.Graphic.gameObject.SetActive(false);
            }

        }
    }

}
