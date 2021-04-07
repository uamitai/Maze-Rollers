using UnityEngine;

public class PlayerColours : MonoBehaviour
{
    [SerializeField] private Shader shader;
    [SerializeField] private Texture specularGloss;

    private Renderer rend;

    private const int textureWidth = 33;
    private const int textureHeight = 33;

    //create a new material for player and set textures
    public void RenderMaterial(Color colour1, Color colour2)
    {
        rend = GetComponent<Renderer>();

        rend.material = new Material(shader);
        rend.material.mainTexture = CreateTexture(colour1, colour2);
        rend.material.SetTexture("_SpecGlossMap", specularGloss);
        rend.material.color = Color.gray;
    }

    //creates albedo texture using two colors
    Texture CreateTexture(Color colour1, Color colour2)
    {
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        SetPixels(texture, 0, colour1);
        SetPixels(texture, textureWidth / 3, Color.black);
        SetPixels(texture, 2 * textureWidth / 3, colour2);

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
