using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ToyRescue.Services
{
    public sealed class AddressablesAssetLoaderService : IAssetLoaderService
    {
        public async Task<TAsset> LoadAsync<TAsset>(string address) where TAsset : Object
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                Debug.LogWarning($"Addressables load skipped for {typeof(TAsset).Name}: address is empty.");
                return null;
            }

            AsyncOperationHandle<TAsset> handle = Addressables.LoadAssetAsync<TAsset>(address);
            try
            {
                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    return handle.Result;
                }

                Debug.LogWarning($"Addressables failed to load '{address}' as {typeof(TAsset).Name}. Check Addressables groups and labels.");
                Addressables.Release(handle);
                return null;
            }
            catch (System.Exception exception)
            {
                Debug.LogWarning($"Addressables exception while loading '{address}' as {typeof(TAsset).Name}: {exception.Message}\n {exception.StackTrace}");
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }

                return null;
            }
        }

        public void Release(Object asset)
        {
            if (asset == null)
            {
                return;
            }

            Addressables.Release(asset);
        }
    }
}
