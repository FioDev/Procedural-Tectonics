using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceDisplay : MonoBehaviour
{
    //Turn noisemap into a texture, and then apply texture as a heightmap to a plane.
    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture)
    {
        
        //Apply in editor independantly of runtime
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);

    }

    public void DrawMesh(MeshData meshData, Texture2D texture)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture = texture;
    }
}
