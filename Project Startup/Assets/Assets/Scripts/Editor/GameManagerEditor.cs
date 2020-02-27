using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    override public void OnInspectorGUI()
    {
        var myScript = target as GameManager;
        bool notEmpty = myScript.inventoryManager != null && myScript.teamManager != null && myScript.gRayCaster != null &&
                        myScript.eventSystem != null && myScript.inventoryButton != null && myScript.inventoryHover != null &&
                        myScript.tileBoard != null && myScript.detailShower != null;

        if(myScript.notSetup && notEmpty)
            myScript.notSetup = EditorGUILayout.Toggle("Things not set up", myScript.notSetup);
 
        using (var group = new EditorGUILayout.FadeGroupScope(Convert.ToSingle(myScript.notSetup)))
        {
            if (group.visible == true)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PrefixLabel("Components");
                myScript.inventoryManager =
                    EditorGUILayout.ObjectField("Inventory Manager", myScript.inventoryManager, typeof(InventoryManager), true) as InventoryManager;
                myScript.teamManager =
                    EditorGUILayout.ObjectField("Team Manager", myScript.teamManager, typeof(TeamManager), true) as TeamManager;
                myScript.gRayCaster =
                    EditorGUILayout.ObjectField("Raycaster", myScript.gRayCaster, typeof(GraphicRaycaster), true) as GraphicRaycaster;
                myScript.eventSystem =
                    EditorGUILayout.ObjectField("Event System", myScript.eventSystem, typeof(EventSystem), true) as EventSystem;
                myScript.inventoryButton =
                    EditorGUILayout.ObjectField("Inventory Button", myScript.inventoryButton, typeof(Transform), true) as Transform;
                myScript.inventoryHover =
                    EditorGUILayout.ObjectField("Inventory Hover", myScript.inventoryHover, typeof(InventoryHover), true) as InventoryHover;
                myScript.tileBoard =
                    EditorGUILayout.ObjectField("Tile Board", myScript.tileBoard, typeof(Transform), true) as Transform;
                myScript.detailShower =
                    EditorGUILayout.ObjectField("Detail Shower", myScript.detailShower, typeof(GameObject), true) as GameObject;
                myScript.startFightMusicBtn =
                    EditorGUILayout.ObjectField("Start Fight Btn", myScript.startFightMusicBtn, typeof(GameObject), true) as GameObject;
                myScript.stopFightMusicBtn =
                    EditorGUILayout.ObjectField("Retry Btn", myScript.stopFightMusicBtn, typeof(GameObject), true) as GameObject;
                EditorGUI.indentLevel--;
            }
        }
    }
}