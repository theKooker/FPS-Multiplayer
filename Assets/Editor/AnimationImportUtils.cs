using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimationImportUtils
{
    [MenuItem("Animation Import/Rename Animation")]
    static void RenameAnimationsWithFileName() {
        foreach(var guid in Selection.assetGUIDs) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetImporter importer = AssetImporter.GetAtPath(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            if (importer is not ModelImporter) {
                Debug.LogError("Asset is not a Model");
                continue;
            }
            ModelImporter modelImporter = (ModelImporter)importer;
            string name = fileName.ToLower().Replace(" ", "_");
            var clipAnimations = modelImporter.clipAnimations.Length == 0 ? modelImporter.defaultClipAnimations : modelImporter.clipAnimations;
            foreach (var animation in clipAnimations)
                animation.name = name;
            modelImporter.clipAnimations = clipAnimations;
            modelImporter.SaveAndReimport();
        }
    }

    [MenuItem("Animation Import/Set To Loop")]
    static void SetAnimationToLoop() {// TODO refactor duplicated code
        foreach(var guid in Selection.assetGUIDs) {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            AssetImporter importer = AssetImporter.GetAtPath(path);
            if (importer is not ModelImporter) {
                Debug.LogError("Asset is not a Model");
                continue;
            }
            ModelImporter modelImporter = (ModelImporter)importer;
            var clipAnimations = modelImporter.clipAnimations.Length == 0 ? modelImporter.defaultClipAnimations : modelImporter.clipAnimations;
            foreach (var animation in clipAnimations)
                animation.loopTime = true;
            modelImporter.clipAnimations = clipAnimations;
            modelImporter.SaveAndReimport();
        }
    }
}
