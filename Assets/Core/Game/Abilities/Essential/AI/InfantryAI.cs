﻿using Newtonsoft.Json;
using RTSLockstep;
using UnityEngine;

namespace RTSLockstep
{
    public class InfantryAI: DeterminismAI
    {
        #region Serialized Values (Further description in properties)
        #endregion

        protected override void OnInitialize()
        {
        }

        protected override void OnVisualize()
        {
          
        }

        public override void CanAttack()
        {
            cachedAttack.CanAttack = true;
        }

        protected override void OnSaveDetails(JsonWriter writer)
        {
            base.SaveDetails(writer);
        }

        protected override void HandleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
        {
            base.HandleLoadedProperty(reader, propertyName, readValue);

        }
    }
}