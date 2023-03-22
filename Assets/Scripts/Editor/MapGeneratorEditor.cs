using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Lithosphere))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
         Lithosphere mapGen = (Lithosphere)target;

        //Draw the default inspector
        if (DrawDefaultInspector())
        {
            if (mapGen.autoUpdate)
            {
                mapGen.GenerateSurface();
            }
        };
        if (GUILayout.Button("Generate"))
        {
            mapGen.GenerateSurface();
        }
    }
}
