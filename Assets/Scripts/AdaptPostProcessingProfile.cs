using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(PostProcessingBehaviour))]
public class AdaptPostProcessingProfile : MonoBehaviour 
{

    #region Variable Declarations
    [SerializeField] PostProcessingBehaviour postProBehaviour = null;
	#endregion
	
	
	
	#region Unity Event Functions
	private void OnEnable () 
	{
        postProBehaviour = GetComponent<PostProcessingBehaviour>();

        if (QualitySettings.GetQualityLevel() >= 2)
            postProBehaviour.profile.ambientOcclusion.enabled = true;
        else
            postProBehaviour.profile.ambientOcclusion.enabled = false;
    }
	
	private void OnDisable () 
	{
        postProBehaviour.profile.ambientOcclusion.enabled = true;
    }
	#endregion
	
	
	
	#region Public Functions
	
	#endregion
	
	
	
	#region Private Functions
	
	#endregion
}
