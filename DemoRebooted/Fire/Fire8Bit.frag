#version 330

in vec2 Texcoord;
uniform sampler2D texFire;
uniform sampler1D texPalette8Bit;
uniform sampler1D texPalette16Bit;

uniform float blend;

out vec4 outColor;

vec4 frag8Bit()
{	
	int sampleX = int(Texcoord.x*160);
	if(sampleX % 2 == 1)
	{
		sampleX--;
	}
	float x = float(sampleX)/160.0;

	//vec4 intensity = texture(texFire, vec2(x, Texcoord.y));
	vec4 intensity = texture(texFire, Texcoord);
	vec4 color = texture(texPalette8Bit, intensity.r);
	return color;
}

vec4 frag16Bit()
{	
	vec4 intensity = texture(texFire, Texcoord);
	vec4 color = texture(texPalette16Bit, intensity.r);
	return color;
}


void main()
{	
	outColor = mix(frag8Bit(), frag16Bit(), blend);
}