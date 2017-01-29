#version 330

in vec2 Texcoord;
uniform sampler2D texFire;
uniform sampler1D texPalette8Bit;
uniform sampler1D texPalette16Bit;

uniform float blend;

out vec4 outColor;

vec4 frag8Bit()
{	
	int sampleX = int(Texcoord.x*320);
	if(sampleX % 2 == 1)
	{
		sampleX--;
	}
	float x = float(sampleX)/320.0;

	vec4 intensity = texture(texFire, vec2(x, Texcoord.y));	
	
	vec4 color = texture(texPalette8Bit, intensity.r);
	// rasterize two colors
	if((sampleX/2 + int(Texcoord.y*200)) % 2 == 0)
	{
		color = texture(texPalette8Bit, intensity.r+0.06);
	}
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
	//outColor += 0.5 * vec4(Texcoord.x, Texcoord.y, 1.0, 1.0);
}