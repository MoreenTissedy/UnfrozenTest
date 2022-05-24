using System;
using System.Collections.Generic;
using System.Linq;
using Spine;
using Spine.Unity;
using UnfrozenTest;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(AnimationSet))]
    public class AnimationSetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var skeleton = serializedObject.FindProperty("skeleton");
            EditorGUILayout.PropertyField(skeleton);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("filter"));
            var skeletonData = skeleton.objectReferenceValue as SkeletonDataAsset;
            if (skeletonData is null)
            {
                EditorGUILayout.LabelField("No skeleton data found");
                return;
            }
            string[] animationsRaw = GetAnimationListFromSpine(skeletonData.GetSkeletonData(false));
            string filter = serializedObject.FindProperty("filter").stringValue;
            string[] animations;
            string[] animationsPlusNone;
            animationsPlusNone = new string[animationsRaw.Length + 1];
            animationsPlusNone[0] = "--NONE--";
            animationsRaw.CopyTo(animationsPlusNone, 1);
            if (filter != String.Empty)
            {
                List<string> fillteredAnimations = new List<string>() {"--NONE--"};
                foreach (var animation in animationsRaw)
                {
                    if (animation.IndexOf(serializedObject.FindProperty("filter").stringValue, StringComparison.OrdinalIgnoreCase)>=0)
                        fillteredAnimations.Add(animation);
                }
                animations = fillteredAnimations.ToArray();
            }
            else
            {
                animations = animationsPlusNone;
            }

            var values = serializedObject.FindProperty("animations");
            values.arraySize = Enum.GetNames(typeof(AnimationSetItems)).Length;
            for (int i = 0; i < values.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Enum.GetName(typeof(AnimationSetItems), i));
                int selected = GetAnimationIndex(animationsPlusNone, values.GetArrayElementAtIndex(i).stringValue);
                string value;
                if (selected >= 1)
                {
                    selected = EditorGUILayout.Popup(selected, animationsPlusNone);
                    value = animationsPlusNone[selected];
                }
                else
                {
                    selected = EditorGUILayout.Popup(0, animations);
                    value = animations[selected];
                }
                values.GetArrayElementAtIndex(i).stringValue = value;
                EditorGUILayout.EndHorizontal();
            }
            serializedObject.ApplyModifiedProperties();
        }

        int GetAnimationIndex(string[] strings, string animation)
        {
            for (var i = 0; i < strings.Length; i++)
            {
                if (string.Equals(strings[i], animation))
                    return i;
            }

            return -1;
        }

        string[] GetAnimationListFromSpine(SkeletonData data)
        {
            List<string> animList = data.Animations.Select(anim => anim.Name).ToList();
            animList.Insert(0, String.Empty);
            return animList.ToArray();
        }
    }
}