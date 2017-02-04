#version 330

in vec3 Position;
in vec2 Texcoord;
in vec3 Normal;
uniform sampler2D texture;
uniform vec4 color;
uniform mat4 view;
uniform sampler2D texContents;

vec3 lightPosition = vec3(-0.5, -0.5, 3.5);
vec3 ambientLightColor = vec3(0.8, 0.8, 1.0);
vec3 lightColor = vec3(1.0, 1.0, 0.9);
float lightAmbientIntensity = 0.4;
float lightDiffuseIntensity = 0.8;

vec3 material_ambient = color.rgb;
vec3 material_diffuse = color.rgb;
vec3 material_specular = vec3(1,1,1);
float material_specExponent = 1.5;

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

vec4 CRT()
{
		vec4 sampleColor = texture2D(texContents, Texcoord);
		sampleColor.r = texture2D(texContents, vec2(Texcoord.x - CRTDISPLACEMENT, Texcoord.y)).r;
		sampleColor.b = texture2D(texContents, vec2(Texcoord.x + CRTDISPLACEMENT, Texcoord.y)).b;

		float phase = scanlinePhase(Texcoord.y);
		if(abs(phase-0.5) < SCANLINERADIUS/2)
		{
			float distance = distanceToScanlineCenter(phase);
			float distanceNormalized = distance / (SCANLINERADIUS/2.0);
			float intensity = MININTENSITY + distanceNormalized*(1.0-MININTENSITY);
			sampleColor = vec4(intensity * sampleColor.rgb, 1.0);
		}
		return sampleColor;
}



void main()
{
		vec2 flipped_texcoord = vec2(Texcoord.x, 1.0 - Texcoord.y);
		vec3 n = normalize(Normal);

		 // Colors
		 vec4 texcolor = CRT();		 
		 vec4 light_ambient = lightAmbientIntensity * vec4(ambientLightColor, 1.0);
		 vec4 light_diffuse = lightDiffuseIntensity * vec4(lightColor, 1.0);
	 
		 // Ambient lighting
		 outColor = light_ambient * vec4(texcolor.rgb, 1.0);

	 
		 // Diffuse lighting
		 vec3 lightvec = normalize(lightPosition - Position);
		 float lambertmaterial_diffuse = max(dot(n, lightvec), 0.0);
		 outColor = outColor + (light_diffuse * texcolor) * lambertmaterial_diffuse;

	 
		 // Specular lighting
		 vec3 reflectionvec = normalize(reflect(-lightvec, Normal));
		 vec3 viewvec = normalize(vec3(inverse(view) * vec4(0,0,0,1)) - Position); 
		 float material_specularreflection = max(dot(Normal, lightvec), 0.0) * pow(max(dot(reflectionvec, viewvec), 0.0), material_specExponent);
		 outColor = outColor + vec4(texcolor.rgb * lightColor, 1.0) * material_specularreflection;
		 outColor.a = color.a;
}

