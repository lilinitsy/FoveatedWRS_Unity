using TMPro;
using UnityEngine;

public class RedShiftComputeShaderScript : MonoBehaviour
{
    public ComputeShader red_shift_compute_shader;
    public RenderTexture _output_texture;

    private int kernel_entrance_function_idx;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Camera.main.targetTexture = new RenderTexture(Screen.width, Screen.height, 32, RenderTextureFormat.ARGB32);
        kernel_entrance_function_idx = red_shift_compute_shader.FindKernel("CSMain"); // Move this to OnRenderImage if the kernel might change.
		_output_texture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBFloat);
		_output_texture.enableRandomWrite = true; // Necessary
        _output_texture.Create();


        Debug.Log("kernel entrance function idx: " + kernel_entrance_function_idx);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if(_output_texture.width != source.width || _output_texture.height != source.height)
        {
            _output_texture.Release(); // Release hardware resources used by the render texture. Does not destroy 
            _output_texture.width = source.width;
            _output_texture.height = source.height;
            _output_texture.Create();
        }

        red_shift_compute_shader.SetTexture(kernel_entrance_function_idx, "_src_texture", source);
        red_shift_compute_shader.SetTexture(kernel_entrance_function_idx, "_output_texture", _output_texture);
        int thread_groups_x = Mathf.CeilToInt(source.width / 8.0f);
        int thread_groups_y = Mathf.CeilToInt(source.height / 8.0f);
        red_shift_compute_shader.Dispatch(kernel_entrance_function_idx, thread_groups_x, thread_groups_y, 1);

        // Copy to result
        Graphics.Blit(_output_texture, destination);
	}
}
