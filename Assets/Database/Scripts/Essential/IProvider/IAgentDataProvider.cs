﻿using UnityEngine;
using System.Collections; using FastCollections;

namespace RTSLockstep.Data
{
    public interface IAgentDataProvider
    {
        IAgentData[] AgentData {get;}
    }
}