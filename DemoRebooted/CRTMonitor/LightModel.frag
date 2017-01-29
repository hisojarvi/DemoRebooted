#version 330

in vec2 Texcoord;
in vec4 Normal;
uniform sampler2D texture;
uniform vec4 color;

out vec4 outColor;

void main()
{
		//outColor = color * vec4(Texcoord.x, Texcoord.y, 0.0, 1.0);
		outColor = color;
}

