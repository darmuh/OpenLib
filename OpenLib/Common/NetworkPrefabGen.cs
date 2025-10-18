using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using GameObject = UnityEngine.GameObject;

namespace OpenLib.Common;

public abstract class NetworkPrefabGenBase
{
    internal static GameObject Prefab { get; set; } = null!;
    internal static GameObject NetObject = null!;
    internal static List<NetworkPrefabGenBase> PrefabGenBases = [];
    internal static void Register<T>(NetworkPrefabGen<T> prefabGen) where T : NetworkBehaviour
    {
        PrefabGenBases.Add(prefabGen);
    }
    internal abstract void NetworkInit();
    public virtual ConfigEntry<bool>? Toggle { get; set; }
    //Can only be called after the object has finished spawning

    internal static void RegisterNetworkPrefabs()
    {
        if (Prefab == null)
        {
            Loggers.WARNING("Unable to RegisterNetworkPrefabs! Openlib Networker asset has not been loaded!");
            return;
        }

        if (PrefabGenBases.Count == 0)
            return;

        foreach (var item in PrefabGenBases)
            item.NetworkInit();

        NetworkManager.Singleton.AddNetworkPrefab(Prefab);
    }

    internal static void SpawnNetworkPrefab()
    {
        if (!ShouldSpawn())
            return;

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            NetObject = Object.Instantiate(Prefab, Vector3.zero, Quaternion.identity);
            NetObject.GetComponent<NetworkObject>().Spawn();
            Loggers.LogMessage($"Host client has spawned Openlib's Network Object");
            return;
        }

        Loggers.LogDebug($"Non-host client will not spawn Network Object");
    }

    //determine whether to spawn the network object for the host
    internal static bool ShouldSpawn()
    {
        if (PrefabGenBases.Count == 0)
            return false;

        foreach (var item in PrefabGenBases)
        {
            if(item.Toggle == null)
            {
                Loggers.LogMessage($"{item} does not have a networking toggle. Openlib networker will be spawned for host!");
                return true;
            }

            if(item.Toggle.Value)
            {
                Loggers.LogMessage($"{item}'s network toggle is enabled. Openlib networker will be spawned for host!");
                return true;
            }
        }

        Loggers.LogMessage("Openlib networker will not be spawned. Networking is disabled.");
        return false;
    }
}
public class NetworkPrefabGen<T> : NetworkPrefabGenBase where T : NetworkBehaviour
{
    internal string Name;

    public NetworkPrefabGen(string name, ConfigEntry<bool> toggle = null!)
    {
        Toggle = toggle;
        Name = name;
        // Explicit registration instead of automatic base constructor
        Register(this);
    }
    public override string ToString()
    {
        return Name;
    }

    internal override void NetworkInit()
    {
        //rather than created a new game object for each mod that adds networking
        //simply re-use the same object but allow for adding more components to it (that have Rpcs)
        Prefab.AddComponent<T>();
        Loggers.LogMessage($"Network Class: {Name} has been initialized on Openlib's Networker!");
    }
}
