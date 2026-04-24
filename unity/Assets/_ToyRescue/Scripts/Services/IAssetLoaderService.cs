using System.Threading.Tasks;
using UnityEngine;

namespace ToyRescue.Services
{
    public interface IAssetLoaderService
    {
        Task<TAsset> LoadAsync<TAsset>(string address) where TAsset : Object;
        void Release(Object asset);
    }
}
