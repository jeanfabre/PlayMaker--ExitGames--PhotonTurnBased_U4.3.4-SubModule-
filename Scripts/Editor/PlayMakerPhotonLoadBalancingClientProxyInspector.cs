// (c) Copyright HutongGames, LLC 2010-2015. All rights reserved.

using UnityEngine;
using System.Collections;
using UnityEditor;

namespace HutongGames.PlayMaker.Photon.TurnBased
{
	[CustomEditor(typeof(PlayMakerPhotonLoadBalancingClientProxy))]
	public class PlayMakerPhotonLoadBalancingClientProxyInspector : UnityEditor.Editor {

		PlayMakerPhotonLoadBalancingClientProxy _target;


		//GUILayoutOption[] GUILayoutOption_null = ;

		public override void OnInspectorGUI()
		{
			//DrawDefaultInspector();


			if(_target==null)
			{
				_target = (PlayMakerPhotonLoadBalancingClientProxy)target;
			}

			//"59b9d56e-198e-4ce3-bcbd-14d2df422b74"

			EditorGUILayout.LabelField("Setup", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
				_target.appId = EditorGUILayout.TextField("appId",_target.appId);
				_target.appVersion = EditorGUILayout.TextField("appVersion",_target.appVersion);
			EditorGUI.indentLevel--;

			if (Application.isPlaying)
			{
				EditorGUILayout.LabelField("Feedback", EditorStyles.boldLabel);
				EditorGUI.indentLevel++;
					EditorGUILayout.LabelField("state",_target.state.ToString());
					EditorGUILayout.LabelField("Status",_target.statusCode.ToString());
				EditorGUI.indentLevel--;
			}
		}
	}
}
