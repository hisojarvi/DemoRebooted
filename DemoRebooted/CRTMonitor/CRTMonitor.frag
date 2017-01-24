#version 330

in vec2 Texcoord;
uniform sampler2D texContents;

out vec4 outColor;

float SCANLINES = 200;
float scanlinePhase(float y)
{
	float phase = y*SCANLINES - floor(y*SCANLINES);
	return phase;
}

float SCANLINERADIUS=0.8;
float MININTENSITY=0.7;
float CRTDISPLACEMENT = 1.0 / 400;

float distanceToScanlineCenter(float phase)
{
	return abs(phase - 0.5);
}

void main()
{
		vec4 sampleColor = texture(texContents, Texcoord);
		sampleColor.r = texture(texContents, vec2(Texcoord.x - CRTDISPLACEMENT, Texcoord.y)).r;
		sampleColor.b = texture(texContents, vec2(Texcoord.x + CRTDISPLACEMENT, Texcoord.y)).b;

		float phase = scanlinePhase(Texcoord.y);
		if(abs(phase-0.5) < SCANLINERADIUS/2)
		{
			float distance = distanceToScanlineCenter(phase);
			float distanceNormalized = distance / (SCANLINERADIUS/2.0);
			float intensity = MININTENSITY + distanceNormalized*(1.0-MININTENSITY);
			sampleColor = vec4(intensity * sampleColor.rgb, 1.0);
		}
		sampleColor.a = 0.7;
		outColor = sampleColor;
}

