#version 410

in vec3 Position1;
out vec4 outColor;

void main()
{	
	outColor = vec4(Position1, 1.0);	
}