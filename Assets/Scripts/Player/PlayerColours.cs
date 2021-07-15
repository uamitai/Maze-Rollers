//script that applies two colours on the player model mesh
//this is done via dynamically creating a material and texture
//sets the texture's pixels as the given colours and places the texture onto the material 


using UnityEngine;

public class PlayerColours : MonoBehaviour
{
    //editor parameters
    [SerializeField] private Shader shader;
    [SerializeField] private Texture specularGloss;

    private Renderer rend;
    
    //texture size constants
    private const int textureWidth = 33;
    private const int textureHeight = 33;

    //create a new material for player and set textures
    public void RenderMaterial(Color colour1, Color colour2)
    {
        //get renderer component
        rend = GetComponent<Renderer>();

        //create new material
        rend.material = new Material(shader);

        //set main texture
        rend.material.mainTexture = CreateTexture(colour1, colour2);
        
        //tidy up
        rend.material.SetTexture("_SpecGlossMap", specularGloss);
        rend.material.color = Color.gray;
    }

    //creates albedo texture using two colors
    Texture CreateTexture(Color colour1, Color colour2)
    {
        //create new texture
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        //colour texture
        SetPixels(texture, 0, colour1);
        SetPixels(texture, textureWidth / 3, Color.black);
        SetPixels(texture, 2 * textureWidth / 3, colour2);

        //apply and return
        texture.Apply();
        return texture;
    }

    //set values of block of pixels
    void SetPixels(Texture2D texture, int x, Color colour)
    {
        for (int h = 0; h < textureHeight; h++)
        {
            for (int w = 0; w < textureWidth / 3; w++)
            {
                texture.SetPixel(x + w, h, colour);
            }
        }
    }
}
