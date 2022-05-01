using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Responsible for spawning instruments into the scene.
/// </summary>
public class InstrumentSpawner : MonoBehaviour
{
    // 
    
    [SerializeField] private List<AssetReference> addressables;
    [SerializeField] private Camera StageCamera;

    private Dictionary<GameObject, AssetReference> valid_instruments;
    private IEnumerator placement_process;

    void Start()
    {
        valid_instruments = new Dictionary<GameObject, AssetReference>();
        if(StageCamera == null) StageCamera = Camera.main;
    }


    public void SpawnInstrument(int index)
    {
    
        
        if(index < 0 || index >= addressables.Count) return;
        AssetReference assetReference = addressables[index];
        if(assetReference.RuntimeKeyIsValid() == false) return;

        Load(assetReference);
    }

    public void SpawnInstrument(AssetReference addressable)
    {
        if(!addressables.Contains(addressable)) return;

        AssetReference assetReference = addressables[addressables.IndexOf(addressable)];
        Load(assetReference);
    }
    private void Load(AssetReference af)
    {
        Debug.Log("loading");
        var operation = Addressables.LoadAssetAsync<GameObject>(af);
        operation.Completed += (operation) => {
            
            GameObject result = operation.Result;
            valid_instruments[result] = af;
            placement_process = PlaceInEnvironment(StageCamera, result);
            StartCoroutine(placement_process);
        
        
            
        };
        
    }

    private IEnumerator PlaceInEnvironment(Camera stage, GameObject instrument)
    {
        bool successful_placement = false;
        GameObject player = Instantiate(instrument, Vector3.zero, Quaternion.identity);

        while(!Input.GetMouseButtonUp(0)){
            Ray stage_point = stage.ScreenPointToRay(Input.mousePosition);
            RaycastHit environment_hit;
            bool hit_environment = Physics.Raycast(stage_point, out environment_hit, 100f, LayerMask.GetMask("Terrain"), QueryTriggerInteraction.Ignore);
            
            if(hit_environment){
                successful_placement = true;
                player.transform.position = environment_hit.point + (environment_hit.normal * 0.5f);
            }
            else{
                successful_placement = false;
                player.transform.position = stage_point.origin + (stage_point.direction * 3);

            }
            
            
            yield return null;
        }

        if(!successful_placement) CancelLoad(valid_instruments[gameObject]);

        yield return null;
    }

    public void CancelLoad(AssetReference af)
    {
        if(placement_process == null) return;
        
        StopCoroutine(placement_process);
        placement_process = null;
        //Addressables.ReleaseInstance();
    }
}
