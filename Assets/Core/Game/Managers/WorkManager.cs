﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RTSLockstep
{
    public static class WorkManager
    {
        //public static GameObject FindHitObject(Vector3 origin)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(origin);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        return hit.collider.gameObject;

        //    }
        //    return null;
        //}

        //public static Vector3 FindHitPoint(Vector3 origin)
        //{
        //    Ray ray = Camera.main.ScreenPointToRay(origin);
        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        return hit.point;
        //    }
        //    return ResourceManager.InvalidPosition;
        //}

        //not needed
        public static Rect CalculateSelectionBox(Bounds selectionBounds, Rect playingArea)
        {
            //shorthand for the coordinates of the centre of the selection bounds
            float cx = selectionBounds.center.x;
            float cy = selectionBounds.center.y;
            float cz = selectionBounds.center.z;
            //shorthand for the coordinates of the extents of the selection bounds
            float ex = selectionBounds.extents.x;
            float ey = selectionBounds.extents.y;
            float ez = selectionBounds.extents.z;

            //Determine the screen coordinates for the corners of the selection bounds
            List<Vector3> corners = new List<Vector3>();
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy + ey, cz - ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx + ex, cy - ey, cz - ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz + ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy + ey, cz - ez)));
            corners.Add(Camera.main.WorldToScreenPoint(new Vector3(cx - ex, cy - ey, cz - ez)));

            //Determine the bounds on screen for the selection bounds
            Bounds screenBounds = new Bounds(corners[0], Vector3.zero);
            for (int i = 1; i < corners.Count; i++)
            {
                screenBounds.Encapsulate(corners[i]);
            }

            //Screen coordinates start in the bottom left corner, rather than the top left corner
            //this correction is needed to make sure the selection box is drawn in the correct place
            float selectBoxTop = playingArea.height - (screenBounds.center.y + screenBounds.extents.y);
            float selectBoxLeft = screenBounds.center.x - screenBounds.extents.x;
            float selectBoxWidth = 2 * screenBounds.extents.x;
            float selectBoxHeight = 2 * screenBounds.extents.y;

            return new Rect(selectBoxLeft, selectBoxTop, selectBoxWidth, selectBoxHeight);
        }

        public static ResourceType GetResourceType(string type)
        {
            switch (type)
            {
                case "Gold":
                    return ResourceType.Gold;
                case "Army":
                    return ResourceType.Army;
                case "Ore":
                    return ResourceType.Ore;
                case "Crystal":
                    return ResourceType.Crystal;
                case "Stone":
                    return ResourceType.Stone;
                case "Wood":
                    return ResourceType.Wood;
                case "Food":
                    return ResourceType.Food;
                default:
                    return ResourceType.Unknown;
            }
        }

        public static FlagState GetFlagState(string type)
        {
            switch (type)
            {
                case "SetFlag":
                    return FlagState.SetFlag;
                case "SettingFlag":
                    return FlagState.SettingFlag;
                case "FlagSet":
                    return FlagState.FlagSet;
                default:
                    return FlagState.SetFlag;
            }
        }

        public static List<RTSAgent> FindNearbyObjects(Vector3 position, float range)
        {
            Collider[] hitColliders = Physics.OverlapSphere(position, range);
            HashSet<int> nearbyObjectIds = new HashSet<int>();
            List<RTSAgent> nearbyObjects = new List<RTSAgent>();
            for (int i = 0; i < hitColliders.Length; i++)
            {
                Transform parent = hitColliders[i].transform.parent;
                if (parent)
                {
                    RTSAgent parentObject = parent.GetComponent<RTSAgent>();
                    if (parentObject && !nearbyObjectIds.Contains(parentObject.GlobalID))
                    {
                        nearbyObjectIds.Add(parentObject.GlobalID);
                        nearbyObjects.Add(parentObject);
                    }
                }
            }
            return nearbyObjects;
        }

        public static RTSAgent FindNearestWorldObjectInListToPosition(List<RTSAgent> objects, Vector3 position)
        {
            if (objects == null || objects.Count == 0)
            {
                return null;
            }

            RTSAgent nearestObject = objects[0];
            float sqrDistanceToNearestObject = Vector3.SqrMagnitude(position - nearestObject.transform.position);
            for (int i = 1; i < objects.Count; i++)
            {
                float sqrDistanceToObject = Vector3.SqrMagnitude(position - objects[i].transform.position);
                if (sqrDistanceToObject < sqrDistanceToNearestObject)
                {
                    sqrDistanceToNearestObject = sqrDistanceToObject;
                    nearestObject = objects[i];
                }
            }

            return nearestObject;
        }
    }
}
