﻿/*
 * Copyright (C) 2017-2020. Autumn Beauchesne. All rights reserved.
 * Author:  Autumn Beauchesne
 * Date:    29 July 2020
 * 
 * File:    CanvasSpaceTransformation.cs
 * Purpose: Helper struct for transforming from world space to canvas space.
 */

using System;
using UnityEngine;

namespace BeauUtil
{
    /// <summary>
    /// Utility for converting between world and canvas space.
    /// </summary>
    [Serializable]
    public struct CanvasSpaceTransformation
    {
        [Header("World Space")]
        public Camera WorldCamera;
        public TransformOffset WorldOffset;

        [Header("Canvas Space")]
        public RectTransform CanvasSpace;
        public Camera CanvasCamera;

        public void Reset()
        {
            this = default(CanvasSpaceTransformation);
        }

        public bool TryLoadWorld(Camera inWorldCamera)
        {
            WorldCamera = inWorldCamera;
            return WorldCamera != null;
        }

        public bool TryLoadCanvas(RectTransform inCanvas, bool inbIncludeInactiveCameras)
        {
            CanvasSpace = inCanvas;
            return CanvasSpace && CanvasSpace.TryGetCamera(inbIncludeInactiveCameras, out CanvasCamera);
        }

        public bool LoadCanvas(RectTransform inCanvas, Camera inCanvasCamera)
        {
            CanvasSpace = inCanvas;
            CanvasCamera = inCanvasCamera;
            return inCanvas != null;
        }

        #region Convert To World Space
    
        public bool TryConvertToWorldSpace(Transform inTransform, out Vector3 outWorld)
        {
            return TryConvertToWorldSpace(inTransform, WorldCamera, WorldOffset, out outWorld);
        }

        public bool TryConvertToWorldSpace(Transform inTransform, TransformOffset inOffset, out Vector3 outWorld)
        {
            return TryConvertToWorldSpace(inTransform, WorldCamera, WorldOffset, out outWorld);
        }

        public bool TryConvertToWorldSpace(Transform inTransform, Camera inWorldCamera, out Vector3 outWorld)
        {
            return TryConvertToWorldSpace(inTransform, WorldCamera, WorldOffset, out outWorld);
        }

        public bool TryConvertToWorldSpace(Transform inTransform, Camera inWorldCamera, TransformOffset inOffset, out Vector3 outWorld)
        {
            if (inWorldCamera == CanvasCamera)
            {
                outWorld = inOffset.EvaluateWorld(inTransform);
                return true;
            }
            
            Vector3 screenSpace = TransformHelper.ScreenPosition(inTransform, inWorldCamera, inOffset);
            return RectTransformUtility.ScreenPointToWorldPointInRectangle(CanvasSpace, screenSpace, CanvasCamera, out outWorld);
        }

        #endregion // Convert To World Space

        #region Convert to Local Space

        public bool TryConvertToLocalSpace(Transform inTransform, out Vector3 outLocal)
        {
            return TryConvertToLocalSpace(inTransform, WorldCamera, WorldOffset, out outLocal);
        }

        public bool TryConvertToLocalSpace(Transform inTransform, TransformOffset inOffset, out Vector3 outLocal)
        {
            return TryConvertToLocalSpace(inTransform, WorldCamera, inOffset, out outLocal);
        }

        public bool TryConvertToLocalSpace(Transform inTransform, Camera inWorldCamera, out Vector3 outLocal)
        {
            return TryConvertToLocalSpace(inTransform, inWorldCamera, WorldOffset, out outLocal);
        }

        public bool TryConvertToLocalSpace(Transform inTransform, Camera inWorldCamera, TransformOffset inOffset, out Vector3 outLocal)
        {
            bool bAvailable = TryConvertToWorldSpace(inTransform, inWorldCamera, inOffset, out outLocal);
            if (bAvailable && CanvasSpace.parent)
            {
                outLocal = CanvasSpace.parent.InverseTransformPoint(outLocal);
            }
            return bAvailable;
        }

        #endregion // Convert to Local Space
    }
}