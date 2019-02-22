using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using EasyRoads3Dv3;

public class APITestsEditor : ScriptableObject {

	[MenuItem( "EasyRoads3D/Set Sidewalk Height" )]
	public static void  Test() 
	{
		ERModularBase scr = FindObjectOfType(typeof(ERModularBase)) as ERModularBase;
		scr.maxCurbHeight = 10;
		Debug.Log("Done!");
	}              


	[MenuItem( "EasyRoads3D/Rotation angles" )]
	public static void  Rotationangles () 
	{
		GameObject go = Selection.activeGameObject;
		ERModularRoad scr = go.GetComponent<ERModularRoad>();
	//	Debug.Log(scr.rotationAngles.Count);
	}

	[MenuItem( "GameObject/3D Object/EasyRoads3D/Display Road Side Objects" )]
	public static void  DisplayRoadSideObjects () 
	{
		ERModularBase scr = FindObjectOfType(typeof(ERModularBase)) as ERModularBase;
		if(scr != null){
			scr.showRoadSideObjects = true;
		}
	}

	[MenuItem( "EasyRoads3D/Get Side Objects" )]
	public static void  GetSideObjects () 
	{
		ERModularBase scr = FindObjectOfType(typeof(ERModularBase)) as ERModularBase;
		for(int i = 0; i < scr.QOQDQOOQDDQOOQ.Count; i++){

			Debug.Log(scr.QOQDQOOQDDQOOQ[i].id + " " + scr.QOQDQOOQDDQOOQ[i].name );
		}
	}

	[MenuItem( "EasyRoads3D/Clean Up Side Objects" )]
	public static void  CleanUpSideObjects () 
	{
		GameObject sourceLog = Resources.Load("ERSideObjectsLog") as GameObject;
		if(sourceLog != null){
			GameObject sourceLogTmp = Instantiate(sourceLog) as GameObject;
			ERSideObjectLog scr = sourceLogTmp.GetComponent<ERSideObjectLog>();
			for(int i = 0; i < scr.QOQDQOOQDDQOOQ.Count; i++){
				if(scr.QOQDQOOQDDQOOQ[i].id == -1){
					scr.QOQDQOOQDDQOOQ.RemoveAt(i);
					i--;
				}
			}

			string path = AssetDatabase.GetAssetOrScenePath(sourceLog);
			path = path.Replace("ERSideObjectsLog.prefab", "");
			//	Debug.Log(path);
			UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(path + "ERProjectLog.prefab");
			PrefabUtility.ReplacePrefab(sourceLogTmp, prefab);
			AssetDatabase.ImportAsset(path + "ERProjectLog.prefab");
			DestroyImmediate(sourceLogTmp);
			//	AssetDatabase.Refresh();
			
			//	sourceLog = Resources.Load("ERProjectLog") as GameObject;
			//	ERSideObjectLog scr = sourceLog.GetComponent<ERSideObjectLog>();
			//	Debug.Log(scr.sideObjects.Count);
		}else{
			Debug.Log("The source prefab with all the road type presets and side object presets cannot be loaded");
		}

	}




	/*
	[MenuItem( "EasyRoads3D/Restore Road Network" )]
	public static void  RestoreNetwork () 
	{
		ERRoadNetwork roadNetwork = new ERRoadNetwork();
		roadNetwork.RestoreRoadNetwork();
	}

	[MenuItem( "EasyRoads3D/Create Road" )]
	public static void  CreateRoad () 
	{
		ERRoadNetwork roadNetwork = new ERRoadNetwork();
		// roadNetwork.BuildRoadNetwork(splatmaps: bool, trees: bool, detailobjects: bool);

	//	ERRoadType erType = roadNetwork.GetRoadTypeByName("Tree line");
		ERRoadType erType = roadNetwork.GetRoadTypeByName("Default Road");
		Vector3[] markers = new Vector3[4];
		markers[0] = new Vector3(1000, 0, 1000);
		markers[1] = new Vector3(1050, 0, 1000);
		markers[2] = new Vector3(1050, 0, 1050);
		markers[3] = new Vector3(1100, 0, 1050);

		List<Vector3> m = new List<Vector3>();
		for(int i = 0; i < 100; i++){
			m.Add(new Vector3(500, 1, 50 + (i * 2.1f )));
		}
		markers = m.ToArray();

		ERRoad road = roadNetwork.CreateRoad("Tree Line API test", erType, markers);
		road.roadScript.faceDistance = 1;
		road.roadScript.UpdateMesh(false, false);
	}
	*/
}
