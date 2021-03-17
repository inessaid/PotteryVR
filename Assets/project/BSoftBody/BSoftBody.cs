using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[AddComponentMenu("Physics/B-Soft Body")]
[RequireComponent(typeof(MeshFilter))]
public class BSoftBody : MonoBehaviour {

    [Header("SoftBody Settings:")]
    [Tooltip("Minimum velocity of the collision in order to trigger the B-Soft calculation.")]
    public float minSoftVelocity = 4f;
    [Tooltip("Scale of the collision impact. A larger scale means a larger dent.")]
    public float collisionImpactScale = 1f;
    [Tooltip("Multiplier for the force of the collision. A larger multiplier results in deeper dents.")]
    public float forceMultiplier = 1f;

    [Header("Normals Settings:")]
    [Tooltip("Whether or not to recalculate the normals after the deformation. (Recommended)")]
    public bool recalculateNormals = true;
    [Tooltip("Whether or not to use a better method for recalculating the normals after the deformation. Recommended if the mesh is small."
        + "This is very slow however and is better left off for larger meshes and one of the first things you should turn off if its running slow.")]
    public bool betterRecalculateNormals = true;
    [Tooltip("When using the better normal recalculation, this is the smoothing angle used in calculation.")]
    public float smoothingAngle = 60f;

    [Header("Collision Settings:")]
    [Tooltip("This will automatically deform the mesh when something collides with it." +
        " (This will only work if the object is not a child of something else that steals its OnCollisionEnter event." +
        " If it is you'll need to relay the collision object to this scripts DeformWithCollision() method.)")]
    public bool deformOnCollision = true;
    [Tooltip("Using this will cause the B-Soft body to use a mesh collider that will be updated every time the mesh is deformed." +
        "Please Note that this is quite a slow feature on larger meshes due to the collision mesh needing to be rebuilt every time." +
        "If you can get away with not using this its probably for the best.")]
    public bool dynamicMeshCollision = true;
    [Tooltip("Recalculates the meshes bounds after every deformation."
        + "You can turn this off to save a small bit of performance on large meshes if youre ok with the bounds not being rebuilt."
        + "This affects mesh culling but so long as the deformation doesnt grow the mesh significantly then it shouldnt matter much.")]
    public bool recalculateBounds = true;
    [Tooltip("Take the mass of the colliding object into account when calculating deformation.")]
    public bool useColliderMass = true;
    [Tooltip("Take the scale of the colliding object into account when calculating deformation.")]
    public bool useColliderScale = true;
    [Tooltip("Use the collisions normal for deformation direction instead of the collisions relative velocity. This drastically changes impact force modeling.")]
    public bool useCollisionNormal = false;
    [Tooltip("Use the OnCollisionStay method for triggering deformations, best used with a minSoftVelocity of 0. This can be useful in some situations though is not recommended.")]
    public bool useCollisionStay = false;
    [Tooltip("Make sure the collider used for deformation is this objects collider and not some other one." +
        " This is useful for when you need to call the DeformWithCollision() method from elsewhere with collision info that could contain collisions from other things.")]
    public bool checkColliderIsThisObject = false;

    MeshFilter mF;
    MeshCollider mC;
    Mesh originalMesh;

    // Use this for initialization
    void Awake() {
        if (GetComponent<SkinnedMeshRenderer>()) {
            Debug.LogError("B-SoftBody: Skinned meshes are not supported");
            return;
        }

        mF = gameObject.GetComponent<MeshFilter>();
        originalMesh = mF.sharedMesh;
        mF.sharedMesh = Instantiate(mF.sharedMesh) as Mesh;

        CheckSwapColliders();
    }

    void CheckSwapColliders() {
        if (dynamicMeshCollision) {
            if (!gameObject.GetComponent<Rigidbody>()) {
                swapColliders();
            } else {
#if UNITY_5
                if ((mF.sharedMesh.triangles.Length / 3) > 256) {
                    Debug.LogError("B-SoftBody: Dynamic collsion is not supported on Rigidbody meshes with more than 256 polygons in Unity 5! " +
                        "This mesh has " + (mF.sharedMesh.triangles.Length / 3) + " polygons.");
                    dynamicMeshCollision = false;
                } else {
                    swapColliders();
                    mC.convex = true;
                }
#else
				swapColliders();
#endif
            }
        }
    }

    void swapColliders() {
        foreach (Collider cld in GetComponents<Collider>()) {
            Destroy(cld);
        }
        mC = gameObject.AddComponent<MeshCollider>();
        mC.sharedMesh = mF.sharedMesh;
    }

    void OnCollisionEnter(Collision col) {
        DeformWithCollision(col);
    }

    void OnCollisionStay(Collision col) {
        if (useCollisionStay)
            DeformWithCollision(col);
    }

    /// <summary>
    /// Call this with collision info to perform deformation calculation with that collision.
    /// </summary>
    /// <param name="col">The collision data to use for this deformation</param>
    public void DeformWithCollision(Collision col) {
        float collisionMagnitude;
        //return if not strong enough impact
        if ((collisionMagnitude = col.relativeVelocity.magnitude) < minSoftVelocity)
            return;

        //get scale thing
        float scale = 1f;
        if (useColliderScale && col.rigidbody && col.collider) {
            scale = col.collider.bounds.size.magnitude * _scaleMult;
        }

        //get mass thing
        float tmass = 1;
        if (useColliderMass && col.rigidbody) {
            tmass = col.rigidbody.mass;
        }

        //calculate some garbage to use
        Vector3 impactVel = col.relativeVelocity * _impactScaleMult;
        float tFM = forceMultiplier * tmass;
        float tIS = collisionImpactScale * scale;

        //run for all contact points
        foreach (ContactPoint contct in col.contacts) {
            if (checkColliderIsThisObject)
                if (contct.thisCollider.transform != transform)
                    continue;

            Vector3 tNormal = useCollisionNormal ? contct.normal : impactVel;
            DeformAtPoint(contct.point, tNormal, collisionMagnitude,
                tIS, true, tFM);
        }
        //END
    }

    void FinalizeNewMesh(Vector3[] softVerts) {
        mF.sharedMesh.vertices = softVerts;
        if (recalculateBounds) mF.sharedMesh.RecalculateBounds();

        if (recalculateNormals)
            if (betterRecalculateNormals)
                mF.sharedMesh.BetterRecalculateNormals(60);
            else
                mF.sharedMesh.RecalculateNormals();

        CheckDynamic();
    }

    void CheckDynamic() {
        if (dynamicMeshCollision) {
            if (!mC) CheckSwapColliders();
            mC.sharedMesh = mF.sharedMesh;
        }
    }


    //Useful public methods

    /// <summary>
    /// Will simply reset the mesh to the orginal mesh state.
    /// </summary>
    public void ResetMesh() {
        //if(!mF)
        //	return;
        Destroy(mF.sharedMesh);
        mF.sharedMesh = Instantiate(originalMesh) as Mesh;

        CheckDynamic();
    }

    /// <summary>
    /// Lerp all vertices in the mesh of the BSoftBody towards the original mesh vertices.
    /// </summary>
    /// <param name="t">The lerp value. 0 being no movement, 1 being fully move to the original vertices.</param>
    public void LerpTowardsOriginal(float t) {
        Vector3[] softVerts = mF.sharedMesh.vertices;
        for (int i = 0; i < softVerts.Length; i++) {
            softVerts[i] = Vector3.Lerp(softVerts[i], originalMesh.vertices[i], t);
        }
        mF.sharedMesh.vertices = softVerts;
        mF.sharedMesh.RecalculateBounds();

        if (recalculateNormals)
            if (betterRecalculateNormals)
                mF.sharedMesh.BetterRecalculateNormals(60);
            else
                mF.sharedMesh.RecalculateNormals();
    }

    /// <summary>
    /// Performs a deformation located at a random vertex of the mesh.
    /// </summary>
    /// <param name="impactForce"></param>
    /// <param name="impactScale"></param>
    /// <param name="inverseImpacts"></param>
    public void RandomImpact(float impactForce = 15f, float impactScale = 1f, bool inverseImpacts = false) {
        int nVert = Random.Range(0, mF.sharedMesh.vertexCount);
        Vector3 worldPoint = transform.TransformPoint(mF.sharedMesh.vertices[nVert]);
        Vector3 impactNormal = mF.sharedMesh.normals[nVert];

        DeformAtPoint(worldPoint, impactNormal, impactForce, impactScale);
        //END
    }


    //some arbitrary variables that make stuff look better i guess
    const float _scaleMult = 0.7f;
    const float _impactScaleMult = 0.025f;
    const float _forceMult = 0.35f;
    const float _distAdd = 0.25f;

    /// <summary>
    /// Will perform an impact at a specified world point, great for use with raycasts.
    /// </summary>
    /// <param name="worldPoint">Point in world space to deform the mesh.</param>
    /// <param name="impactNormal">Direction of the force of the impact to be applied with the deformation.</param>
    /// <param name="impactForce">The force of the impact that will deform the mesh.</param>
    /// <param name="impactScale">Scale of the impact.</param>
    /// <param name="inverseImpacts">Whether or not to invert the impactNormal.</param>
    /// <param name="custForceMult">A custom force multiplier, if left 0 it uses the default.</param>
    /// <param name="updateMesh">Whether or not to update the mesh after the deformation. This is useful for saving a lot of 
    /// performance when you might want to deform the mesh multiple times before updating.</param>
    public void DeformAtPoint(Vector3 worldPoint, Vector3 impactNormal, float impactForce = 15f, float impactScale = 1f, bool inverseImpacts = false, float custForceMult = 0f, bool updateMesh = true) {

        ////some code for timing. uncomment this and at the bottom if you want to time the deformation
        //var sw = new System.Diagnostics.Stopwatch();
        //sw.Reset();
        //sw.Start();

        float tIS = impactScale * _impactScaleMult / transform.localScale.magnitude;
        float tFM = custForceMult != 0f ? custForceMult * _forceMult : forceMultiplier * _forceMult;

        float maxDist = impactForce * tIS;
        if (maxDist > impactScale + _distAdd)
            maxDist = impactScale + _distAdd;
        //sqrMaxDist for sqr distance checks which are much faster
        maxDist *= maxDist;

        Vector3 localImpactNormal = transform.InverseTransformDirection((inverseImpacts ? impactNormal : -impactNormal) * tFM);
        Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

        Vector3[] softVerts = mF.sharedMesh.vertices;
        for (int i = 0; i < softVerts.Length; i++) {
            Vector3 localVert = softVerts[i];
            float tmpDist = Vector3.SqrMagnitude(localPoint - localVert);
            //float tmpDist = Vector3.Distance(localPoint, worldVert);
            if (tmpDist < maxDist) {
                localVert += localImpactNormal * (maxDist - tmpDist);
                softVerts[i] = localVert;
            }
        }

        if (updateMesh) FinalizeNewMesh(softVerts);

        ////end timing
        //sw.Stop();
        //Debug.Log("Deform Took: " + sw.ElapsedMilliseconds + " : " + sw.ElapsedTicks);


        //END
    }



    //END CLASS
}
