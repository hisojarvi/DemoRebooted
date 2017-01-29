#version 330

in vec3 position;
in vec2 texcoord;
in vec3 normal;

uniform mat4 projection;
uniform mat4 view;
uniform mat4 model;

out vec2 Texcoord;

void main()
{
	Texcoord = texcoord; 
    gl_Position = vec4(position, 1.0);
}