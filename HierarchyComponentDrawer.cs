using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class HierarchyComponentDrawer {
    static HierarchyComponentDrawer()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
    }

    const int WIDTH = 16;
    const int HEIGHT = 16;

    static void OnGUI(int instanceId, Rect selectionRect)
    {
        var obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
        if(obj == null)
        {
            return;
        }

        var pos = selectionRect;
        pos.x = pos.xMax - WIDTH;
        pos.width = WIDTH;
        pos.height = HEIGHT;

        var components = obj.GetComponents<Component>().Where(c => c != null).Where(c => !(c is Transform)).Reverse();

        var current = Event.current;
        foreach (var c in components)
        {
            var thumbnail = AssetPreview.GetMiniThumbnail(c);
            if(thumbnail == null)
            {
                continue;
            }

            if(current.type == EventType.MouseDown && pos.Contains(current.mousePosition))
            {
                Debug.LogFormat("Select! {0} - {1}", c.name, c.GetType());
                c.SetEnable(!c.IsEnable());

                
            }

            var color = GUI.color;
            GUI.color = c.IsEnable() ? Color.white : Color.gray;
            GUI.DrawTexture(pos, thumbnail, ScaleMode.ScaleToFit);
            GUI.color = color;
            pos.x -= pos.width;

        }
    }

    public static bool IsEnable(this Component self)
    {
        if(self == null)
        {
            return true;
        }

        var type = self.GetType();
        var property = type.GetProperty("enabled", typeof(bool));

        if(property == null)
        {
            return true;
        }

        return (bool) property.GetValue(self, null);
    }

    public static void SetEnable(this Component self, bool enable)
    {
        if(self == null)
        {
            return;
        }

        var type = self.GetType();
        var property = type.GetProperty("enable", typeof(bool));

        if(property == null)
        {
            return;
        }

        property.SetValue(self, enable);
    }
}
