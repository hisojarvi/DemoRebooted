#version 330

in vec3 Position;
in vec2 Texcoord;
in vec3 Normal;
uniform sampler2D texture;
uniform vec4 color;
uniform mat4 view;


/*
vec3 lightPosition = vec3(-0.5, -0.5, 3.5);
vec3 ambientLightColor = vec3(0.8, 0.8, 1.0);
vec3 lightColor = vec3(1.0, 1.0, 0.9);
float lightAmbientIntensity = 0.4;
float lightDiffuseIntensity = 0.8;
*/
uniform vec3 lightPosition;
uniform vec3 lightColor;
uniform float lightDiffuseIntensity;

vec3 ambientLightColor = vec3(0.8, 0.8, 1.0);
float lightAmbientIntensity = 0.4;



vec3 material_ambient = color.rgb;
vec3 material_diffuse = color.rgb;
vec3 material_specular = color.rgb;
float material_specExponent = 1.1;

out vec4 outColor;

void main()
{
	outColor = color * vec4(Texcoord.x, Texcoord.y, 0.0, 1.0);
	
	vec2 flipped_texcoord = vec2(Texcoord.x, 1.0 - Texcoord.y);
	vec3 n = normalize(Normal);

	 // Colors
	 vec4 texcolor = vec4(1.0, 1.0, 1.0, 1.0); //texture2D(texture, flipped_texcoord.xy);
	 vec4 light_ambient = lightAmbientIntensity * vec4(ambientLightColor, 1.0);
	 vec4 light_diffuse = lightDiffuseIntensity * vec4(lightColor, 1.0);
	 
	 // Ambient lighting
	 outColor = light_ambient * vec4(texcolor.rgb, 1.0) * vec4(material_ambient, 1.0);

	 
	 // Diffuse lighting
	 vec3 lightvec = normalize(lightPosition - Position);
	 float lambertmaterial_diffuse = max(dot(n, lightvec), 0.0);
	 outColor = outColor + (light_diffuse * texcolor * vec4(material_diffuse, 1.0)) * lambertmaterial_diffuse;

	 
	 // Specular lighting
	 vec3 reflectionvec = normalize(reflect(-lightvec, Normal));
	 vec3 viewvec = normalize(vec3(inverse(view) * vec4(0,0,0,1)) - Position); 
	 float material_specularreflection = max(dot(Normal, lightvec), 0.0) * pow(max(dot(reflectionvec, viewvec), 0.0), material_specExponent);
	 outColor = outColor + vec4(material_specular * lightColor, 1.0) * material_specularreflection;
	 outColor.a = color.a;
	 
}


/*
void
main()
{
 vec2 flipped_texcoord = vec2(f_texcoord.x, 1.0 - f_texcoord.y);
 vec3 n = normalize(v_norm);

 // Colors
 vec4 texcolor = texture2D(maintexture, flipped_texcoord.xy);
 vec4 light_ambient = light_ambientIntensity * vec4(light_color, 0.0);
 vec4 light_diffuse = light_diffuseIntensity * vec4(light_color, 0.0);

 // Ambient lighting
 outputColor = texcolor * light_ambient * vec4(material_ambient, 0.0);

 // Diffuse lighting
 vec3 lightvec = normalize(light_position - v_pos);
 float lambertmaterial_diffuse = max(dot(n, lightvec), 0.0);
 outputColor = outputColor + (light_diffuse * texcolor * vec4(material_diffuse, 0.0)) * lambertmaterial_diffuse;

 // Specular lighting
 vec3 reflectionvec = normalize(reflect(-lightvec, v_norm));
 vec3 viewvec = normalize(vec3(inverse(view) * vec4(0,0,0,1)) - v_pos); 
 float material_specularreflection = max(dot(v_norm, lightvec), 0.0) * pow(max(dot(reflectionvec, viewvec), 0.0), material_specExponent);
 outputColor = outputColor + vec4(material_specular * light_color, 0.0) * material_specularreflection;
}
*/