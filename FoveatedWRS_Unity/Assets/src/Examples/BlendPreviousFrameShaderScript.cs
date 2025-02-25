using UnityEngine;

public class BlendPreviousFrameShaderScript : MonoBehaviour
{
    public ComputeShader blend_previous_frame_compute_shader;

    public RenderTexture _previous_frame_texture;
	
    public RenderTexture _blended_frame_output_texture; // Source is the input, so this is the "
	private int blend_previous_frame_kernel_handle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        blend_previous_frame_kernel_handle = blend_previous_frame_compute_shader.FindKernel("CSMain");

        _previous_frame_texture = create_render_texture(Screen.width, Screen.height);
		_blended_frame_output_texture = create_render_texture(Screen.width, Screen.height);
	}

	// Update is called once per frame
	void Update()
    {
        
    }


    private RenderTexture create_render_texture(int w, int h)
    {
        RenderTexture render_texture = new RenderTexture(w, h, 0, RenderTextureFormat.ARGBFloat);
        render_texture.enableRandomWrite = true;
        render_texture.Create();
        return render_texture;
    }


	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if(_blended_frame_output_texture.width != source.width || _blended_frame_output_texture.height != source.height ||
            _previous_frame_texture.width != source.width || _previous_frame_texture.height != source.height)
        {
			_blended_frame_output_texture.Release();
            _previous_frame_texture.Release();

			_blended_frame_output_texture = create_render_texture(Screen.width, Screen.height);
			_previous_frame_texture = create_render_texture(Screen.width, Screen.height);
		}

        blend_previous_frame_compute_shader.SetTexture(blend_previous_frame_kernel_handle, "_current_frame_texture", source);
        blend_previous_frame_compute_shader.SetTexture(blend_previous_frame_kernel_handle, "_previous_frame_texture", _previous_frame_texture);
        blend_previous_frame_compute_shader.SetTexture(blend_previous_frame_kernel_handle, "_blended_frame_output_texture", _blended_frame_output_texture);

        blend_previous_frame_compute_shader.Dispatch(blend_previous_frame_kernel_handle, source.width / 8, source.height / 8, 1);

        Graphics.Blit(_blended_frame_output_texture, destination);
	}


    // This will get more complicated in real use cases
    private void update_render_textures_after_render()
    {
        _previous_frame_texture = _blended_frame_output_texture;
    }
}
