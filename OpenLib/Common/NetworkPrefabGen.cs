using BepInEx.Configuration;
using System.Collections.Generic;
using Unity.Netcode;
using GameObject = UnityEngine.GameObject;

namespace OpenLib.Common;

public abstract class NetworkPrefabGenBase
{
    internal static List<NetworkPrefabGenBase> PrefabGenBases = [];
    internal static void Register<T>(NetworkPrefabGen<T> prefabGen) where T : NetworkBehaviour
    {
        PrefabGenBases.Add(prefabGen);
    }
    internal abstract void NetworkInit();
    public abstract GameObject GetPrefabObject();
    //Can only be called after the object has finished spawning
    public abstract bool TryGetNetObject(out NetworkObject result);
    internal abstract bool TrySpawnNetworkHandler();

    internal static void RegisterNetworkPrefabs()
    {
        if (PrefabGenBases.Count == 0)
            return;

        foreach (var item in PrefabGenBases)
            item.NetworkInit();
    }

    internal static void SpawnNetworkPrefabs()
    {
        if (PrefabGenBases.Count == 0)
            return;

        foreach (var item in PrefabGenBases)
        {
            if (item.TrySpawnNetworkHandler())
                Loggers.LogMessage($"Network Prefab: {item} has been spawned! (You are the host client)");
        }
    }

    internal static bool IsPrefab(GameObject query)
    {
        foreach (var item in PrefabGenBases)
        {
            if (item.GetPrefabObject() == query)
                return true;
        }

        return false;
    }
}
public class NetworkPrefabGen<T> : NetworkPrefabGenBase where T : NetworkBehaviour
{
    internal GameObject NetworkPrefab = null!;
    internal NetworkObject NetObj = null!;
    internal string ObjectName;
    internal ConfigEntry<bool> Toggle;

    public NetworkPrefabGen(string objectName, ConfigEntry<bool> toggle = null!)
    {
        ObjectName = objectName;
        Toggle = toggle;

        // Explicit registration instead of automatic base constructor
        Register(this);
    }
    public override string ToString()
    {
        return ObjectName;
    }

    internal bool ShouldSpawn()
    {
        if (Toggle == null)
            return true;

        return Toggle.Value;
    }

    internal override void NetworkInit()
    {
        NetworkPrefab = new GameObject(ObjectName);
        GameObject.DontDestroyOnLoad(NetworkPrefab);
        NetworkPrefab.AddComponent<NetworkObject>();
        NetworkPrefab.AddComponent<NetworkSpawnModifier>();
        NetworkPrefab.AddComponent<T>();

        NetworkManager.Singleton.AddNetworkPrefab(NetworkPrefab);
        Loggers.LogMessage($"Network Prefab: {ObjectName} has been initialized!");
    }

    internal override bool TrySpawnNetworkHandler()
    {
        if (!ShouldSpawn())
            return false;

        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
        {
            NetObj = NetworkObject.InstantiateAndSpawn(NetworkPrefab, NetworkManager.Singleton);
            //OnNetworkSpawnComplete.Invoke();
            return true;
        }

        Loggers.LogDebug($"Non-host client will not spawn NetworkPrefab - {ObjectName}");
        return false;
    }

    public override GameObject GetPrefabObject()
    {
        return NetworkPrefab;
    }

    public override bool TryGetNetObject(out NetworkObject result)
    {
        result = NetObj;
        return NetObj != null;
    }
}

internal class NetworkSpawnModifier : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (NetworkPrefabGenBase.IsPrefab(gameObject))
            return;

        Loggers.LogDebug($"{gameObject.name} is NOT a prefab");
        base.OnNetworkSpawn();
    }
}
