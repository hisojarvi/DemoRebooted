#version 330

in vec3 texCoord;
uniform samplerCube skyboxTexture;
uniform float brightness;

out vec4 outColor;

vec3 rotatedTexCoord(float rotation)
{
	float hypotenuse = sqrt(texCoord.z*texCoord.z + texCoord.x*texCoord.x);
	float rotY = atan(texCoord.z / texCoord.x);
	float tex2rotY = rotY + rotation;
	vec3 texCoord2 = vec3(cos(tex2rotY)*hypotenuse, texCoord.y, sin(tex2rotY)*hypotenuse);
	return texCoord2;
}

void main (void) {
	//outColor = vec4(1,1,1,1); 
	outColor = texture(skyboxTexture, texCoord);

	vec3 texCoord2 = rotatedTexCoord(1.68);
	outColor += 0.5 * texture(skyboxTexture, texCoord2);

	vec3 texCoord3 = rotatedTexCoord(0.68);
	vec4 frag3 = texture(skyboxTexture, texCoord3);
	outColor += frag3*frag3;
	outColor *= brightness;
}