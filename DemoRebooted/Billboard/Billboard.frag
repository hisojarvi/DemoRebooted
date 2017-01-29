#version 330

in vec2 Texcoord;
uniform sampler2D billboardTex;

out vec4 outColor;

void main()
{	
	outColor = texture(billboardTex, Texcoord);	
}