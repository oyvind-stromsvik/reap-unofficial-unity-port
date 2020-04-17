using UnityEngine;
using UnityEditor;

public class SpritePostProcessor : AssetPostprocessor {

    /// <summary>
    /// Allows you to assign defaults to the texture importer so you don't have
    /// to set the same values manually for every sprite you import.
    /// </summary>
    void OnPostprocessTexture(Texture2D texture) {
        // I found this line of code through Google. I don't know how it works,
        // but without it you're not able to change texture importer values
        // manually anymore. With this line the defaults below only apply to
        // newly imported textures.
        Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture2D));
        if (asset) {
            return;
        }

        // Set the defaults we want all the sprites in our project to have.
        TextureImporter textureImporter = assetImporter as TextureImporter;
        textureImporter.spritePixelsPerUnit = 1;
        textureImporter.filterMode = FilterMode.Point;
        textureImporter.mipmapEnabled = false;
        textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
    }
}
