using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadSpriteManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private AsyncOperationHandle<Sprite> spriteOperation;

    public void loadNewSprite(string spriteName)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteOperation = Addressables.LoadAssetAsync<Sprite>(spriteName);
        spriteOperation.Completed += SpriteLoaded;
    }


    private void SpriteLoaded(AsyncOperationHandle<Sprite> obj)
    {
        switch (obj.Status)
        {
            case AsyncOperationStatus.Succeeded:
            spriteRenderer.sprite = obj.Result;
            break;

            case AsyncOperationStatus.Failed:
            Debug.LogError("Sprite load failed.");
            break;
            default: break;
        }
    }

    public void OnDestroy()
    {
        if (spriteOperation.IsValid())
        {
            Addressables.Release(spriteOperation);
            Debug.Log("Success");
        }
    }
}
