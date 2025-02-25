using UnityEngine;

public class FoveatedRenderingComputeShaderScript : MonoBehaviour
{
    public ComputeShader foveated_compute_shader;
	public RenderTexture _mip1_input_texture; // Workarounds for unity sucking
	public RenderTexture _mip2_input_texture; 

	public RenderTexture _output_render_texture;

    [Range(0.0f, 1.0f)] public float foveal_radius = 0.1f;
    [Range(0.0f, 1.0f)] public float mid_periphery_radius = 0.3f;

    private int foveated_kernel_handle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foveated_kernel_handle = foveated_compute_shader.FindKernel("CSMain");
        _output_render_texture = create_render_texture(Screen.width, Screen.height);
		_mip1_input_texture = create_render_texture(Screen.width / 2, Screen.height / 2);
		_mip2_input_texture = create_render_texture(Screen.width / 4, Screen.height / 4);
	}

	// Update is called once per frame
	void Update()
    {
        
    }


	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (_output_render_texture.width != source.width || _output_render_texture.height != source.height)
		{
			_output_render_texture.Release();
			_mip1_input_texture.Release();
			_mip2_input_texture.Release();
			_output_render_texture = create_render_texture(Screen.width, Screen.height);
			_mip1_input_texture = create_render_texture(Screen.width / 2, Screen.height / 2);
			_mip2_input_texture = create_render_texture(Screen.width / 4, Screen.height / 4);
		}

		// Screen space image can't gen mipmaps, so we have to do some bullshit workaround
		Graphics.Blit(source, _mip1_input_texture);
		Graphics.Blit(_mip1_input_texture, _mip2_input_texture);

		// This would be improved with eye tracking!
		Vector2 foveation_center = new Vector2(0.5f, 0.5f);
		foveated_compute_shader.SetVector("_foveation_center", foveation_center);
		foveated_compute_shader.SetFloat("_radius_fovea", foveal_radius);
		foveated_compute_shader.SetFloat("_radius_periphery", mid_periphery_radius);

		foveated_compute_shader.SetTexture(foveated_kernel_handle, "_input_frame_texture", source);
		foveated_compute_shader.SetTexture(foveated_kernel_handle, "_mip1_texture", _mip1_input_texture);
		foveated_compute_shader.SetTexture(foveated_kernel_handle, "_mip2_texture", _mip2_input_texture);
		foveated_compute_shader.SetTexture(foveated_kernel_handle, "_foveated_output_texture", _output_render_texture);

		foveated_compute_shader.Dispatch(foveated_kernel_handle, source.width / 8, source.height / 8, 1);

		Graphics.Blit(_output_render_texture, destination);
	}


	private RenderTexture create_render_texture(int w, int h)
	{
		RenderTexture render_texture = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBFloat);
		render_texture.enableRandomWrite = true;
		//render_texture.useMipMap = true;
		//render_texture.autoGenerateMips = true;
		render_texture.Create();
		return render_texture;
	}
}
