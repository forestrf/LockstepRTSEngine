﻿//=======================================================================
// Copyright (c) 2015 John Pan
// Distributed under the MIT License.
// (See accompanying file LICENSE or copy at
// http://opensource.org/licenses/MIT)
//=======================================================================
using RTSLockstep.Data;
using Newtonsoft.Json;
using UnityEngine;

namespace RTSLockstep
{
    public abstract class Ability : MonoBehaviour//CerealBehaviour
    {
        private bool isCasting;

        private LSAgent _agent;

        public LSAgent Agent
        {
            get
            {
#if UNITY_EDITOR
                if (_agent == null)
                    return GetComponent<LSAgent>();
#endif
                return _agent;
            }
        }

        public string MyAbilityCode { get; private set; }

        public AbilityDataItem Data { get; private set; }

        public int ID { get; private set; }

        public Transform CachedTransform { get { return Agent.CachedTransform; } }

        public GameObject CachedGameObject { get { return Agent.CachedGameObject; } }

        public int VariableContainerTicket { get; private set; }

        private LSVariableContainer _variableContainer;

        public LSVariableContainer VariableContainer { get { return _variableContainer; } }

        public bool IsCasting
        {
            get
            {
                return isCasting;
            }
            protected set
            {
                if (value != isCasting)
                {
                    if (value == true)
                    {
                        Agent.CheckCasting = false;
                    }
                    else
                    {
                        Agent.CheckCasting = true;
                    }
                    isCasting = value;
                }
            }
        }
        protected bool loadedSavedValues = false;

        internal void Setup(LSAgent agent, int id)
        {
            System.Type mainType = this.GetType();
            if (mainType.IsSubclassOf(typeof(ActiveAbility)))
            {
                while (mainType.BaseType != typeof(ActiveAbility) &&
                       mainType.GetCustomAttributes(typeof(CustomActiveAbilityAttribute), false).Length == 0)
                {
                    mainType = mainType.BaseType;
                }
                Data = AbilityDataItem.FindInterfacer(mainType);
                if (Data == null)
                {
                    throw new System.ArgumentException("The Ability of type " + mainType + " has not been registered in database");
                }
                this.MyAbilityCode = Data.Name;
            }
            else
            {
                this.MyAbilityCode = mainType.Name;
            }
            _agent = agent;
            ID = id;
            TemplateSetup();
            OnSetup();
            this.VariableContainerTicket = LSVariableManager.Register(this);
            this._variableContainer = LSVariableManager.GetContainer(VariableContainerTicket);
        }

        internal void LateSetup()
        {
            this.OnLateSetup();
        }
        /// <summary>
        /// Override for communicating with other abilities in the setup phase
        /// </summary>
        protected virtual void OnLateSetup() { }

        protected virtual void TemplateSetup()
        {

        }

        protected virtual void OnSetup()
        {

        }

        internal void Initialize()
        {
            VariableContainer.Reset();
            IsCasting = false;
            IsFirstFrame = true;

            OnInitialize();
        }

        protected virtual void OnInitialize()
        {
        }

        bool IsFirstFrame = true;
        private void FirstFrame()
        {
            OnFirstFrame();
        }
        protected virtual void OnFirstFrame()
        {

        }

        internal void Simulate()
        {
            if (IsFirstFrame)
            {
                FirstFrame();
            }
            TemplateSimulate();

            OnSimulate();
            if (isCasting)
            {
                OnCast();
            }
        }

        protected virtual void TemplateSimulate()
        {

        }

        protected virtual void OnSimulate()
        {
        }

        internal void LateSimulate()
        {
            OnLateSimulate();
        }

        protected virtual void OnLateSimulate()
        {

        }

        protected virtual void OnCast()
        {
        }

        internal void Visualize()
        {
            OnVisualize();
        }

        protected virtual void OnVisualize()
        {
        }

        public void LateVisualize()
        {
            this.OnLateVisualize();
        }

        protected virtual void OnLateVisualize()
        {

        }

        protected virtual void OnGUI()
        {
        }

        public void BeginCast()
        {
            OnBeginCast();
        }

        protected virtual void OnBeginCast()
        {
        }

        public void StopCast()
        {
            OnStopCast();
        }

        protected virtual void OnStopCast()
        {
        }

        public void Deactivate()
        {
            IsCasting = false;
            OnDeactivate();
        }

        protected virtual void OnDeactivate()
        {
        }

        public void SaveDetails(JsonWriter writer)
        {
            OnSaveDetails(writer);
        }

        protected virtual void OnSaveDetails(JsonWriter writer) { }

        public void LoadDetails(JsonTextReader reader)
        {
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string propertyName = (string)reader.Value;
                        reader.Read();
                        HandleLoadedProperty(reader, propertyName, reader.Value);
                    }
                }
                else if (reader.TokenType == JsonToken.EndObject)
                {
                    loadedSavedValues = true;
                    return;
                }
            }
            //loaded position invalidates the selection bounds so they must be recalculated
            loadedSavedValues = true;
        }

        protected virtual void HandleLoadedProperty(JsonTextReader reader, string propertyName, object readValue)
        { }
    }
}