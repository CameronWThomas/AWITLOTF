//using UnityEngine;
//using UnityEditor;

//namespace AWITLOTF.Assets.Code.Scripts.Npc
//{
//    [CustomEditor(typeof(NpcManager))]
//    public class NpcManagerEditor : Editor
//    {
//        public override void OnInspectorGUI()
//        {
//            // Draw the default inspector
//            DrawDefaultInspector();

//            // Add some space
//            EditorGUILayout.Space(10);

//            // Get reference to the NpcManager
//            NpcManager npcManager = (NpcManager)target;

//            // Create a button to advance the queue
//            if (GUILayout.Button("Advance Queue", GUILayout.Height(30)))
//            {
//                npcManager.AdvanceQueue();
//            }

//            // Display current index info
//            EditorGUILayout.HelpBox($"Current Pedestrian Index: {npcManager.currentPedestrianIndex}", MessageType.Info);
//        }
//    }
//}
