using System;
using System.Collections.Generic;
public class FloatOrList
{
	private float value;
	private List<float> arr_value;

    public FloatOrList(float f)
    {
		value = f;
		arr_value = null;
    }

	public FloatOrList(List<float> f)
	{
		value = -123f;
		arr_value = f;
	}

    public Object getValue()
	{
		if (value != -123f)
			return value;
		else
			return arr_value;
	}
}
