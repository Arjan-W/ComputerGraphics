using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    private GameObject gain;
    private GameObject lacunarity;
    private GameObject octaves;
    private GameObject scale;
    private GameObject shift;
    private GameObject state;
    private GameObject resolution;
    private GameObject length_val;
    private GameObject height_val;
    private Script01 script;

    // Start is called before the first frame update
    void Awake()
    {
        // Find script
        script = GameObject.Find("Landscape").GetComponent<Script01>();

        // Find settings
        gain = gameObject.transform.Find("Gain").gameObject;
        lacunarity = gameObject.transform.Find("Lacunarity").gameObject;
        octaves = gameObject.transform.Find("Octaves").gameObject;
        scale = gameObject.transform.Find("Scale").gameObject;
        shift = gameObject.transform.Find("Shift").gameObject;
        state = gameObject.transform.Find("State").gameObject;
        resolution = gameObject.transform.Find("Resolution").gameObject;
        length_val = gameObject.transform.Find("Length").gameObject;
        height_val = gameObject.transform.Find("Height").gameObject;
    }

    void Start() {
        // Update settings
        set_gain(script.gain);
        set_lacunarity(script.lacunarity);
        set_octaves(script.octaves);
        set_scale(script.scale);
        set_shift(script.shift);
        set_state(script.state);
        set_resolution(script.resolution);
        set_length(script.length);
        set_height(script.height);

        // Set OnChangeValues
        gain.transform.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener( delegate {gain_from_slider();});
        gain.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {gain_from_field();});
        lacunarity.transform.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener( delegate {lacunarity_from_slider();});
        lacunarity.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {lacunarity_from_field();});
        octaves.transform.Find("Slider").GetComponent<Slider>().onValueChanged.AddListener( delegate {octaves_from_slider();});
        octaves.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {octaves_from_field();});
        scale.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {scale_from_field();});
        shift.transform.Find("X").Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {shift_from_field();});
        shift.transform.Find("Y").Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {shift_from_field();});
        state.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {state_from_field();});
        resolution.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {resolution_from_field();});
        length_val.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {length_from_field();});
        height_val.transform.Find("InputField").GetComponent<InputField>().onValueChanged.AddListener( delegate {height_from_field();});
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void set_gain(float gain_val) {
        // Update slider value
        Slider slider = gain.transform.Find("Slider").GetComponent<Slider>();
        slider.value = gain_val;
        // Update inputfield
        InputField field =  gain.transform.Find("InputField").GetComponent<InputField>();
        field.text = gain_val.ToString();
    }

    public void gain_from_slider() {
        // Grab slider value
        Slider slider = gain.transform.Find("Slider").GetComponent<Slider>();
        float gain_val = slider.value;
        // Update inputfield
        InputField field =  gain.transform.Find("InputField").GetComponent<InputField>();
        field.text = gain_val.ToString();
        // Update script
        script.UpdateGain(gain_val);
    }

    public void gain_from_field() {
        // Grab inputfield value
        InputField field =  gain.transform.Find("InputField").GetComponent<InputField>();
        float gain_val = float.Parse(field.text);
        // Grab slider value
        Slider slider = gain.transform.Find("Slider").GetComponent<Slider>();
        slider.value = gain_val;
        // Update script
        script.UpdateGain(gain_val);
    }

    public void set_lacunarity(float l_val) {
        // Update slider value
        Slider slider = lacunarity.transform.Find("Slider").GetComponent<Slider>();
        slider.value = l_val;
        // Update inputfield
        InputField field =  lacunarity.transform.Find("InputField").GetComponent<InputField>();
        field.text = l_val.ToString();
    }

    public void lacunarity_from_slider() {
        // Grab slider value
        Slider slider = lacunarity.transform.Find("Slider").GetComponent<Slider>();
        float lacunarity_val = slider.value;
        // Update inputfield
        InputField field =  lacunarity.transform.Find("InputField").GetComponent<InputField>();
        field.text = lacunarity_val.ToString();
        // Update script
        script.UpdateLacunarity(lacunarity_val);
    }

    public void lacunarity_from_field() {
        // Grab inputfield value
        InputField field =  lacunarity.transform.Find("InputField").GetComponent<InputField>();
        float lacunarity_val = float.Parse(field.text);
        // Grab slider value
        Slider slider = lacunarity.transform.Find("Slider").GetComponent<Slider>();
        slider.value = lacunarity_val;
        // Update script
        script.UpdateLacunarity(lacunarity_val);
    }


    public void set_octaves(int oct) {
        // Update slider value
        Slider slider = octaves.transform.Find("Slider").GetComponent<Slider>();
        slider.value = oct;
        // Update inputfield
        InputField field =  octaves.transform.Find("InputField").GetComponent<InputField>();
        field.text = oct.ToString();
    }

    public void octaves_from_slider() {
        // Grab slider value
        Slider slider = octaves.transform.Find("Slider").GetComponent<Slider>();
        int oct = (int) slider.value;
        // Update inputfield
        InputField field =  octaves.transform.Find("InputField").GetComponent<InputField>();
        field.text = oct.ToString();
        // Update script
        script.UpdateOctaves(oct);
    }

    public void octaves_from_field() {
        // Grab inputfield value
        InputField field =  octaves.transform.Find("InputField").GetComponent<InputField>();
        int oct = int.Parse(field.text);
        // Grab slider value
        Slider slider = octaves.transform.Find("Slider").GetComponent<Slider>();
        slider.value = oct;
        // Update script
        script.UpdateOctaves(oct);
    }

    public void set_scale(float scl) {
        // Update inputfield
        InputField field =  scale.transform.Find("InputField").GetComponent<InputField>();
        field.text = scl.ToString();
    }

    public void scale_from_field() {
        // Grab inputfield value
        InputField field =  scale.transform.Find("InputField").GetComponent<InputField>();
        float scl = float.Parse(field.text);
        // Update script
        script.UpdateScale(scl);
    }

    public void set_shift(Vector2 shft) {
        // Update inputfields
        InputField x = shift.transform.Find("X").Find("InputField").GetComponent<InputField>();
        x.text = shft.x.ToString();
        InputField y = shift.transform.Find("Y").Find("InputField").GetComponent<InputField>();
        y.text = shft.y.ToString();
    }

    public void shift_from_field() {
        // Grab inputfield value
        InputField x = shift.transform.Find("X").Find("InputField").GetComponent<InputField>();
        float x_val = float.Parse(x.text);
        InputField y = shift.transform.Find("Y").Find("InputField").GetComponent<InputField>();
        float y_val = float.Parse(y.text);
        Vector2 shft = new Vector2(float.Parse(x.text), float.Parse(y.text));
        // Update script
        script.UpdateShift(shft);
    }

    public void set_state(int st) {
        // Update inputfield
        InputField field =  state.transform.Find("InputField").GetComponent<InputField>();
        field.text = st.ToString();
    }

    public void state_from_field() {
        // Grab inputfield value
        InputField field =  state.transform.Find("InputField").GetComponent<InputField>();
        int st = int.Parse(field.text);
        // Update script
        script.UpdateScale(st);
    }

    public void set_resolution(int res) {
        // Update inputfield
        InputField field =  resolution.transform.Find("InputField").GetComponent<InputField>();
        field.text = res.ToString();
    }

    public void resolution_from_field() {
        // Grab inputfield value
        InputField field =  resolution.transform.Find("InputField").GetComponent<InputField>();
        int res = int.Parse(field.text);
        // Update script
        script.UpdateScale(res);
    }

    public void set_length(float len) {
        // Update inputfield
        InputField field =  length_val.transform.Find("InputField").GetComponent<InputField>();
        field.text = len.ToString();
    }

    public void length_from_field() {
        // Grab inputfield value
        InputField field =  length_val.transform.Find("InputField").GetComponent<InputField>();
        float len = float.Parse(field.text);
        // Update script
        script.UpdateScale(len);
    }

    public void set_height(float h) {
        // Update inputfield
        InputField field =  height_val.transform.Find("InputField").GetComponent<InputField>();
        field.text = h.ToString();
    }

    public void height_from_field() {
        // Grab inputfield value
        InputField field =  height_val.transform.Find("InputField").GetComponent<InputField>();
        float h = float.Parse(field.text);
        // Update script
        script.UpdateScale(h);
    }

}
